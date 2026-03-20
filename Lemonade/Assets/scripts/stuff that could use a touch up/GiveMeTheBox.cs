using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool DogRanAway;
    private bool _previousDogRanAway;

    [Tooltip("Assign the timmy mover component here so GiveMeTheBox can trigger Timmy distraction.")]
    public timmy_mover timmyMover;
    
    
    public signchanger signMover;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
            Debug.LogWarning("GiveMeTheBox: playerPickupScript is not assigned.", this);
    }

    // Optional other entity that needs to know when dog ran away.
    [Tooltip("Assign the dog mover component here so GiveMeTheBox can trigger dog run behavior.")]
    public dog_mover dogMover;

    // Called when the player clicks on this GameObject (requires a Collider)
    private void OnMouseDown()
    {
        
        
        if (globalVariables.istalking == true)
            {
                
                return;
            }


        if (_hasTriggered)
            return;

        if (playerPickupScript == null)
            return;

        // Start a coroutine to wait until player is close enough
        StartCoroutine(WaitForPlayerAndUse());
    }

    private IEnumerator WaitForPlayerAndUse()
    {
        // Continuously check until player is within requiredDistance
        while (true)
        {
            if (playerPickupScript == null)
                yield break;

            float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
            if (distance <= requiredDistance)
                break;

            // Optional: debug log (comment out in production)
            Debug.Log($"GiveMeTheBox: player is {distance:F2} units away, needs to be within {requiredDistance} to use.", this);

            yield return null; // wait one frame and check again
        }

        // Check if player has the required item
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

    // Update the global variable
    GlobalVariables.DogRanAway = true; // Add this line

    GlobalVariables.specialEventActive = true; // Example: trigger special dialogues to start happening
    

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

    // TODO: add additional behavior here (open door, spawn something, etc.)
}

private void ApplyDogRanAwayEffects()
{
    if (_hasTriggered) // optional: only allow once
        return;

    _hasTriggered = true;

    if (_renderer != null && successMaterial != null)
        _renderer.material = successMaterial;

    // Update the global variable
    GlobalVariables.DogRanAway = true; // Add this line

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
            Debug.Log("GiveMeTheBox: timmyMover.NotifyDogReturned invoked", this);
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
        // Restore original material so editor/play mode doesn't keep it changed.
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
