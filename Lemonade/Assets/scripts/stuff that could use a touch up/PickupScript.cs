using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public class PickupScript : MonoBehaviour
{


    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private NavMeshAgent agent;

    [Header("Pickup Settings")]
    [Tooltip("Objects must have this tag to be pickable.")]
    [SerializeField] private string pickupTag = "Pickup";
    [Tooltip("How close the player must get before the object is picked up")]
    [SerializeField] private float pickUpDistance = 1.2f;

    [Tooltip("Becomes true once at least one item has been picked up.")]
    public bool hasItem { get; private set; }

    /// <summary>
    /// A simple list of identifiers for items the player has picked up.
    /// This can be used to allow the player to "use" items later.
    /// </summary>
    [SerializeField]
    public List<string> collectedItemIds = new List<string>();

    /// <summary>
    /// Read-only access for other scripts.
    /// </summary>
    public IReadOnlyList<string> CollectedItemIds => collectedItemIds;

    [ContextMenu("Print collected items")]
    private void PrintCollectedItems()
    {
        Debug.Log("Collected items: " + string.Join(", ", collectedItemIds));
    }

    private Transform currentTarget;

public Animator playerAnimator;
    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {


if (globalVariables.istalking == true)
            {
                
                return;
            }

            TrySetTargetFromClick();
        }

        if (currentTarget != null)
        {
            CheckIfReachedTarget();
        }
    }

    private void TrySetTargetFromClick()
    {
        if (cam == null || agent == null)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.CompareTag(pickupTag))
            {
                currentTarget = hit.collider.transform;
                agent.SetDestination(currentTarget.position);
            }
        }
    }

    private void CheckIfReachedTarget()
    {
        if (agent.pathPending)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget <= pickUpDistance || agent.remainingDistance <= agent.stoppingDistance)
        {


if (currentTarget.name == "plane" && !collectedItemIds.Contains("stick")) return;
            

            PickupCurrentTarget();
        }
    }

    private void PickupCurrentTarget()
    {
        if (currentTarget == null)
            return;

        // Prefer a PickupItem component for a stable identifier.
        var pickupItem = currentTarget.GetComponent<PickupItem>();
        string itemId = pickupItem != null && !string.IsNullOrWhiteSpace(pickupItem.ItemId)
            ? pickupItem.ItemId
            : currentTarget.gameObject.name;

        collectedItemIds.Add(itemId);

        Destroy(currentTarget.gameObject);
        currentTarget = null;

        hasItem = collectedItemIds.Count > 0;

    }
}


