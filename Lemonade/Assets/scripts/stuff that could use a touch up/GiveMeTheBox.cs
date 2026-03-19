using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    [Tooltip("Assign the timmy mover component here so GiveMeTheBox can trigger Timmy distraction.")]
    public timmy_mover timmyMover;

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
        if (_hasTriggered)
            return;

        if (playerPickupScript == null)
            return;

        // Check distance from player to this object
        float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
        if (distance > requiredDistance)
            return;

        // Check if player has the required item
        if (!playerPickupScript.CollectedItemIds.Contains(requiredItemId))
            return;

        TriggerSuccess();
    }

    private void TriggerSuccess()
    {
        _hasTriggered = true;

        if (_renderer != null && successMaterial != null)
            _renderer.material = successMaterial;

        Debug.Log($"GiveMeTheBox: player used '{requiredItemId}' on '{name}'", this);

        DogRanAway = true;
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
            timmyMover.Timmy_Distracted_one = true;
            Debug.Log($"GiveMeTheBox: timmyMover.Timmy_Distracted_one set to {timmyMover.Timmy_Distracted_one}", this);
        }
        else
        {
            Debug.LogWarning("GiveMeTheBox: timmyMover reference is not assigned, Timmy won't start moving.", this);
        }

        // TODO: add additional behavior here (open door, spawn something, etc.)
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
    }

    private void OnDisable()
    {
        // Restore original material so editor/play mode doesn't keep it changed.
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
