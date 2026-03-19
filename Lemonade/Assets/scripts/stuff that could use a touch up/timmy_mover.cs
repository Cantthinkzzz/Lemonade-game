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

    // Optional cached renderers used to flip sprites when moving toward B.
    private SpriteRenderer _spriteRenderer;

    [Tooltip("The shadow's SpriteRenderer; if not assigned it will be searched for in children.")]
    public SpriteRenderer shadowSpriteRenderer;

    [Tooltip("If true, Timmy will stop moving forever the first time he reaches target B.")]
    public bool stopAtBWhenReached = true;

    private bool _timmyStoppedAtB = false;

    public bool Timmy_Distracted_one = false;
    public Animator timmyAnimator;
    public Animator timmyshadowanimator;

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
        if (_timmyStoppedAtB)
        {
            if (timmyAnimator != null)
                timmyAnimator.SetBool("timmywalk", false);
            if (timmyshadowanimator != null)
                timmyshadowanimator.SetBool("timmywalk", false);
            return;
        }

        if (Timmy_Distracted_one == false)
        {
            if (timmyAnimator != null)
                timmyAnimator.SetBool("timmywalk", false);
            if (timmyshadowanimator != null)
                timmyshadowanimator.SetBool("timmywalk", false);
        }


        if (Timmy_Distracted_one == false) return;

        if (timmyAnimator != null)
            timmyAnimator.SetBool("timmywalk", true);
        if (timmyshadowanimator != null)
            timmyshadowanimator.SetBool("timmywalk", true);



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
            if (_movingTowardB && stopAtBWhenReached)
            {
                _timmyStoppedAtB = true;
                Timmy_Distracted_one = false;
                if (timmyAnimator != null)
                    timmyAnimator.SetBool("timmywalk", false);
                if (timmyshadowanimator != null)
                    timmyshadowanimator.SetBool("timmywalk", false);
                Debug.Log("timmy_mover: reached B and stopped (stopAtBWhenReached=true).", this);
                return;
            }
            
            _movingTowardB = !_movingTowardB;
            UpdateSpriteFlip();
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
}
