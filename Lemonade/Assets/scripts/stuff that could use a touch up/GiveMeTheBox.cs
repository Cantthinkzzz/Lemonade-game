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

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
            _originalMaterial = _renderer.material;

        if (playerPickupScript == null)
            Debug.LogWarning("GiveMeTheBox: playerPickupScript is not assigned.", this);
    }

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

        // TODO: add additional behavior here (open door, spawn something, etc.)
    }

    private void OnDisable()
    {
        // Restore original material so editor/play mode doesn't keep it changed.
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}
