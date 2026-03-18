using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timmy_mover : MonoBehaviour
{
    [Tooltip("The first target (A) for the GameObject to move towards.")]
    public Transform targetA;

    [Tooltip("The second target (B) for the GameObject to move towards.")]
    public Transform targetB;

    [Tooltip("Movement speed in units per second.")]
    public float speed = 5f;

    [Tooltip("Stop moving once this close to the current target.")]
    public float stopDistance = 0.1f;

    [Tooltip("If true, the object will flip horizontally while moving toward target B.")]
    public bool flipWhenMovingToB = true;

    [Tooltip("If true, the object will start moving toward target A (then alternate between A and B). If false, it will start toward B.")]
    public bool startAtA = true;

    // Tracks which target we're currently moving toward.
    private bool _movingTowardB;

    // Optional cached renderer used to flip sprites when moving toward B.
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _movingTowardB = !startAtA;
        UpdateSpriteFlip();
    }

    void Update()
    {
        Transform currentTarget = GetCurrentTarget();
        if (currentTarget == null) return;

        UpdateSpriteFlip();

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentTarget.position;

        // Move towards the target using a constant speed.
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);

        // Switch target once close enough to avoid jitter and make it loop between A and B.
        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            _movingTowardB = !_movingTowardB;
        }
    }

    private void UpdateSpriteFlip()
    {
        if (_spriteRenderer == null || !flipWhenMovingToB) return;
        _spriteRenderer.flipX = _movingTowardB;
    }

    private Transform GetCurrentTarget()
    {
        return _movingTowardB ? targetB : targetA;
    }
}
