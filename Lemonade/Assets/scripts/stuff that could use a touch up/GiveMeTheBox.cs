using System.Collections;
using System.Linq;
using UnityEngine;
using globalVariables = GlobalVariables;

[RequireComponent(typeof(Collider))]
public class GiveMeTheBox : MonoBehaviour
{
    [Header("Requirements")]
    public string requiredItemId = "box";
    public float requiredDistance = 2f;
    public PickupScript playerPickupScript;

    [Header("Feedback")]
    public Material successMaterial;

    private Material _originalMaterial;
    private Renderer _renderer;
    private bool _hasTriggered;

    [Header("Dialogue")]
    public RedstoneinventeGameStudio.NPCManager targetNpc;

    public bool DogRanAway;
    private bool _previousDogRanAway;

    public timmy_mover timmyMover;
    public signchanger signMover;
    public dog_mover dogMover;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null) _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
            Debug.LogWarning("GiveMeTheBox: playerPickupScript is not assigned.", this);
    }

    private void OnMouseDown()
    {
        if (_hasTriggered) return;
        if (playerPickupScript == null) return;
        if (globalVariables.istalking) return;

        StartCoroutine(WaitForPlayerAndUse());
    }

    private IEnumerator WaitForPlayerAndUse()
    {
        while (true)
        {
            if (playerPickupScript == null) yield break;

            float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
            if (distance <= requiredDistance) break;

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
            targetNpc.EnableSpecial(); // <-- updated method

            if (RedstoneinventeGameStudio.DialogueManager.instance != null)
            {
                globalVariables.istalking = false;
                RedstoneinventeGameStudio.DialogueManager.instance.ShowDialogue(targetNpc);
                // No more isShowing check required, coroutine handles single-line display per click
            }
            else
            {
                Debug.LogWarning("GiveMeTheBox: DialogueManager.instance is null.", this);
            }
        }

        if (dogMover != null) dogMover.DogRanAway = true;
        if (timmyMover != null) timmyMover.StartDistraction();
    }

    private void ApplyDogRanAwayEffects()
    {
        if (_hasTriggered) return;
        _hasTriggered = true;

        if (_renderer != null && successMaterial != null) _renderer.material = successMaterial;

        GlobalVariables.DogRanAway = true;

        if (dogMover != null) dogMover.DogRanAway = true;
        if (timmyMover != null) timmyMover.StartDistraction();
    }

    private void Update()
    {
        if (!_previousDogRanAway && DogRanAway)
        {
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