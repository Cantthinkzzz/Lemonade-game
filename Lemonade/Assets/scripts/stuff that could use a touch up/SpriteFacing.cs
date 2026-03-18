using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpriteFacing : MonoBehaviour
{
    [Tooltip("Optional. If not set, Camera.main is used.")]
    public Camera targetCamera;

    [Tooltip("How fast the object must be moving horizontally before flipping occurs.")]
    public float flipThreshold = 0.05f;

    [Tooltip("Optional. If set, uses this NavMeshAgent for movement direction. Otherwise tries to find one on this object or its parents.")]
    public NavMeshAgent agent;

    [Tooltip("Optional. If set, uses this Rigidbody for movement direction. Otherwise tries to find one on this object or its parents.")]
    public Rigidbody rb;

    [Tooltip("Optional. If set, uses this Rigidbody2D for movement direction. Otherwise tries to find one on this object or its parents.")]
    public Rigidbody2D rb2D;

    private Vector3 _baseLocalScale;

    void Awake()
    {
        _baseLocalScale = transform.localScale;

        // Ensure we can still find a movement component even if not assigned.
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody>();

        if (rb2D == null)
            rb2D = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 cameraPosition = cam.transform.position;
        transform.LookAt(cameraPosition);
        transform.Rotate(0, 180, 0);

        // Flip sprite based on movement direction (left/right) relative to the camera.
        float moveX = GetHorizontalMovement(cam);
        if (Mathf.Abs(moveX) > flipThreshold)
        {
            float sign = Mathf.Sign(moveX);
            transform.localScale = new Vector3(Mathf.Abs(_baseLocalScale.x) * sign, _baseLocalScale.y, _baseLocalScale.z);
        }
    }

    private float GetHorizontalMovement(Camera cam)
    {
        Vector3 velocity = Vector3.zero;

        // Prefer explicitly assigned components, then fall back to components on this object or parent.
        if (rb2D != null)
            velocity = rb2D.velocity;
        else if (rb != null)
            velocity = rb.velocity;
        else if (agent != null)
            velocity = agent.velocity;

        // Convert to camera-relative coordinates so "right"/"left" matches screen direction.
        if (cam != null)
            velocity = cam.transform.InverseTransformDirection(velocity);

        return velocity.x;
    }
}