using UnityEngine;
using globalVariables = GlobalVariables;       

public class TeleportObject : MonoBehaviour
{
    public Transform target; // The object to teleport to

    private bool hasTeleported = false; // To ensure teleportation happens only once

    void Update()
    {
        if (globalVariables.TimmyReturned && !hasTeleported) // Check the global variable
        {
            Teleport();
            hasTeleported = true; // Mark as having teleported
        }
    }

    void Teleport()
    {
        if (target != null)
        {
            transform.position = target.position; // Teleport to target's position
            transform.rotation = target.rotation; // Optional: match rotation
        }
    }
}