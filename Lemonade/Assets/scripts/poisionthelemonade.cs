using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using globalVariables = GlobalVariables; 

[RequireComponent(typeof(Collider))]


public static class GlobalVariables
{
    public static bool DogRanAway = false;
    public static bool TimmyReturned = false;
    public static bool PoisionedLemonade = false;

    public static bool TimmyPlaneGiven = false;

    public static bool istalking = false;

    public static bool specialEventActive = false;
    public static bool afterEventTriggered = false;
}

public class poisionthelemonade : MonoBehaviour
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

    public Animator lemonadeanimator;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
            Debug.LogWarning("GiveMeTheBox: playerPickupScript is not assigned.", this);
            if(globalVariables.afterEventTriggered == true)globalVariables.afterEventTriggered = false;
            if(globalVariables.DogRanAway == true)globalVariables.DogRanAway = false;
            if(globalVariables.istalking == true)globalVariables.istalking = false;
            if(globalVariables.PoisionedLemonade == true)globalVariables.PoisionedLemonade = false;
            if(globalVariables.specialEventActive == true)globalVariables.specialEventActive = false;
            if(globalVariables.TimmyPlaneGiven == true)globalVariables.TimmyPlaneGiven = false;
            if(globalVariables.TimmyReturned == true)globalVariables.TimmyReturned = false;
    }



    // Called when the player clicks on this GameObject (requires a Collider)
    private void OnMouseDown()
    {
        if (_hasTriggered)
            return;

        if (playerPickupScript == null)
            return;

        float distance = Vector3.Distance(playerPickupScript.transform.position, transform.position);
        
        if (distance > requiredDistance)
            return;

Debug.LogWarning("Aaaaaaaa", this);
        // Check if player has the required item
        if (!playerPickupScript.CollectedItemIds.Contains(requiredItemId))
            return;

            if (!globalVariables.TimmyPlaneGiven) return;

        TriggerSuccess();
    }

    private void TriggerSuccess()
{
    _hasTriggered = true;

    lemonadeanimator.SetTrigger("Poisioned");

    Debug.Log($"GiveMeTheBox: player used '{requiredItemId}' on '{name}'", this);

    // Update the global variable
    GlobalVariables.PoisionedLemonade = true;

    

    // TODO: add additional behavior here (open door, spawn something, etc.)
}

    private void OnDisable()
    {
        // Restore original material so editor/play mode doesn't keep it changed.
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
