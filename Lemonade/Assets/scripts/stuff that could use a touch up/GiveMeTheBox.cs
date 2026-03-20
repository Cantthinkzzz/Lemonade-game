using System.Collections;
using System.Linq; // for Enumerable.Contains if CollectedItemIds is IReadOnlyList
using UnityEngine;
using globalVariables = GlobalVariables;

[RequireComponent(typeof(Collider))]
public class GiveMeTheBox : MonoBehaviour
{
    [Header("Requirements")]
    [Tooltip("Item ID the player must have collected (e.g. \"car_keys\").")]
    public string requiredItemId = "box";

    [Tooltip("How close the player must be to this object to trigger.")]
    public float requiredDistance = 2f;

    [Tooltip("Reference to the player's pickup script.")]
    public PickupScript playerPickupScript;

    [Header("Feedback")]
    [Tooltip("Optional material to apply when the condition is met.")]
    public Material successMaterial;

    private Material _originalMaterial;
    private Renderer _renderer;
    private bool _hasTriggered;

    [Header("Dialogue")]
    [Tooltip("The NPC to start special dialogue on immediately when success happens.")]
    public RedstoneinventeGameStudio.NPCManager targetNpc;

    public bool DogRanAway;
    private bool _previousDogRanAway;

    [Tooltip("Assign the timmy mover component here so GiveMeTheBox can trigger Timmy distraction.")]
    public timmy_mover timmyMover;

    public signchanger signMover;

    [Tooltip("Assign the dog mover component here so GiveMeTheBox can trigger dog run behavior.")]
    public dog_mover dogMover;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
            Debug.LogWarning("GiveMeTheBox: playerPickupScript is not assigned.", this);
    }

    private void OnMouseDown()
    {
        if (globalVariables.istalking == true)
            return;

        if (_hasTriggered)
            return;

        if (playerPickupScript == null)
            return;

        StartCoroutine(WaitForPlayerAndUse());
    }

    private IEnumerator WaitForPlayerAndUse()
    {
        while (true)
        {
            if (playerPickupScript == null)
                yield break;

            float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
            if (distance <= requiredDistance)
                break;

            yield return null;
        }

        if (playerPickupScript.CollectedItemIds == null || !playerPickupScript.CollectedItemIds.Contains(requiredItemId))
            yield break;

        TriggerSuccess();
    }

    private void TriggerSuccess()
    {
        _hasTriggered = true;

        if (_renderer != null && successMaterial != null)
            _renderer.material = successMaterial;

        Debug.Log($"GiveMeTheBox: player used '{requiredItemId}' on '{name}'", this);

        // Set globals
        GlobalVariables.DogRanAway = true;
        GlobalVariables.specialEventActive = true;

        // Enable special on NPC and start dialogue immediately
        if (targetNpc != null)
        {
            targetNpc.EnableSpecialForThisNPC();

            if (RedstoneinventeGameStudio.DialogueManager.instance != null)
            {
                // ensure talking flag is reset so dialogue manager can set it
                globalVariables.istalking = false;

                // Start dialogue and wait for its completion to clean up
                RedstoneinventeGameStudio.DialogueManager.instance.ShowDialogue(targetNpc);
                StartCoroutine(WaitForDialogueEndAndCleanup());
            }
            else
            {
                Debug.LogWarning("GiveMeTheBox: DialogueManager.instance is null; cannot start dialogue immediately.", this);
            }
        }
        else
        {
            Debug.Log("GiveMeTheBox: no targetNpc assigned. globalVariables.specialEventActive set for future clicks.", this);
        }

        if (dogMover != null)
        {
            dogMover.DogRanAway = true;
            Debug.Log($"GiveMeTheBox: dogMover.DogRanAway set to {dogMover.DogRanAway}", this);
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: dogMover reference is not assigned, dog won't move.", this);
        }

        if (timmyMover != null)
        {
            timmyMover.StartDistraction();
            Debug.Log("GiveMeTheBox: timmyMover.StartDistraction invoked", this);
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: timmyMover reference is not assigned, Timmy won't start moving.", this);
        }
    }

    private IEnumerator WaitForDialogueEndAndCleanup()
    {
        var dm = RedstoneinventeGameStudio.DialogueManager.instance;
        if (dm == null)
        {
            GlobalVariables.specialEventActive = false;
            yield break;
        }

        yield return new WaitUntil(() => dm.isShowing == false);

        // Dialogue finished
        GlobalVariables.specialEventActive = false;

        // Trigger sign change if available (calls ApplySuccessMaterial on your signchanger)
        if (signMover != null)
        {
            signMover.ApplySuccessMaterial();
        }

        // Optionally return dog after dialogue
        // ReturnDog();

        yield break;
    }

    private void ApplyDogRanAwayEffects()
    {
        if (_hasTriggered)
            return;

        _hasTriggered = true;

        if (_renderer != null && successMaterial != null)
            _renderer.material = successMaterial;

        GlobalVariables.DogRanAway = true;

        if (dogMover != null)
        {
            dogMover.DogRanAway = true;
            Debug.Log($"GiveMeTheBox: dogMover.DogRanAway set to {dogMover.DogRanAway}", this);
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: dogMover reference is not assigned, dog won't move.", this);
        }

        if (timmyMover != null)
        {
            timmyMover.StartDistraction();
            Debug.Log("GiveMeTheBox: timmyMover.StartDistraction invoked", this);
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: timmyMover is not assigned, Timmy won't start moving.", this);
        }
    }

    public void ReturnDog()
    {
        if (dogMover != null)
        {
            dogMover.TriggerDogReturn();
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: dogMover is not set, cannot return dog.", this);
        }

        if (timmyMover != null)
        {
            timmyMover.NotifyDogReturned();
            Debug.Log("GiveMeTheBox: timmy_mover.NotifyDogReturned invoked", this);
        }
    }

    private void Update()
    {
        if (!_previousDogRanAway && DogRanAway)
        {
            Debug.Log("GiveMeTheBox: DogRanAway changed to true, applying run behavior.", this);
            ApplyDogRanAwayEffects();
        }

        _previousDogRanAway = DogRanAway;
    }

    private void OnDisable()
    {
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
