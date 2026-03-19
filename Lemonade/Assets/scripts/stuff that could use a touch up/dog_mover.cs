using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dog_mover : MonoBehaviour
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

    // Optional cached renderers used to flip sprites when moving toward B.
    private SpriteRenderer _spriteRenderer;

    [Tooltip("The shadow's SpriteRenderer; if not assigned it will be searched for in children.")]
    public SpriteRenderer shadowSpriteRenderer;

    public bool DogRanAway;
    public bool DogReturned;
    public bool DogWaitingAtB;

    public Animator dogAnimator;
    public Animator dogshadowanimator;

    public bool Timmy_Distracted_one = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (shadowSpriteRenderer == null)
        {
            shadowSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (shadowSpriteRenderer == _spriteRenderer)
                shadowSpriteRenderer = null;
        }

        _movingTowardB = !startAtA;
        UpdateSpriteFlip();
    }

    void Update()
    {
        bool shouldGoToB = DogRanAway;
        bool shouldReturnToA = DogReturned && DogWaitingAtB;

        if (!shouldGoToB && !shouldReturnToA)
        {
            if (dogAnimator != null)
                dogAnimator.SetBool("dogwalk", false);
            if (dogshadowanimator != null)
                dogshadowanimator.SetBool("dogwalk", false);
            return;
        }

        if (dogAnimator != null)
            dogAnimator.SetBool("dogwalk", true);
        if (dogshadowanimator != null)
            dogshadowanimator.SetBool("dogwalk", true);

        Transform currentTarget = shouldGoToB ? targetB : targetA;
        _movingTowardB = shouldGoToB;
        if (currentTarget == null) return;
        UpdateSpriteFlip();

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentTarget.position;

        // Move towards the target using a constant speed.
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);

        // Check arrival condition and update states.
        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            if (DogRanAway)
            {
                DogRanAway = false;
                DogWaitingAtB = true;
                DogReturned = false;
                Debug.Log("dog_mover: reached B and is now waiting for return trigger.", this);
            }
            else if (DogReturned && DogWaitingAtB)
            {
                DogReturned = false;
                DogWaitingAtB = false;
                Debug.Log("dog_mover: returned to A.", this);
            }
        }
    }

    private void UpdateSpriteFlip()
    {
        if (!flipWhenMovingToB) return;

        if (_spriteRenderer != null)
            _spriteRenderer.flipX = _movingTowardB;

        if (shadowSpriteRenderer != null)
            shadowSpriteRenderer.flipX = _movingTowardB;
    }

    private Transform GetCurrentTarget()
    {
        return _movingTowardB ? targetB : targetA;
    }

    public void TriggerDogReturn()
    {
        if (!DogWaitingAtB)
        {
            Debug.LogWarning("dog_mover: dog is not waiting at B; cannot return yet.", this);
            return;
        }

        DogReturned = true;
        DogRanAway = false;
        Debug.Log("dog_mover: return trigger received, heading back to A.", this);
    }
}