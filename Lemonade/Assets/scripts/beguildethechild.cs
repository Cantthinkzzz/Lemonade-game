using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using globalVariables = GlobalVariables; 

[RequireComponent(typeof(Collider))]


public class beguilethechild : MonoBehaviour
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

public Transform target;
    private Material _originalMaterial;
    private Renderer _renderer;
    private bool _hasTriggered;

    public Animator timmyAnimator;
    public Animator timmyshadowanimator;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

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

    Debug.Log($"GiveMeTheBox: player used '{requiredItemId}' on '{name}'", this);

    // Update the global variable
    GlobalVariables.TimmyPlaneGiven = true;
        {
            // Move the agent to the target's position
            navMeshAgent.SetDestination(target.position);
            
            
        

        if (Vector3.Distance(transform.position, target.position) < 1.0f)
        {
            navMeshAgent.isStopped = true; // Stop moving
            timmyAnimator.SetBool("timmywalk", false);
            timmyshadowanimator.SetBool("timmywalk", false);
            
            
        }
        else
        {
            navMeshAgent.isStopped = false; // Continue moving
            timmyAnimator.SetBool("timmywalk", true);
            timmyshadowanimator.SetBool("timmywalk", true);
        }
    }


    // TODO: add additional behavior here (open door, spawn something, etc.)
}

    private void OnDisable()
    {
        // Restore original material so editor/play mode doesn't keep it changed.
        if (_renderer != null && _originalMaterial != null)
            _renderer.material = _originalMaterial;
    }
}