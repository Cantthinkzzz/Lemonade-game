 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public interface Itimmy_mover
{
    void NotifyDogReturned();
    void OnDogReturned();
    void StartDistraction();
}

public class timmy_mover : MonoBehaviour, Itimmy_mover
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

    [Tooltip("If true, Timmy starts idle. If false, Timmy starts idle (distraction only triggers movement).")]
    public bool startAtA = true;

    [Tooltip("Set true externally when the dog has returned to allow Timmy to go back to A.")]
    public bool dogReturned = false;

    public Animator timmyAnimator;
    public Animator timmyshadowanimator;

    [Tooltip("Reference to the dog mover so Timmy can trigger dog return on collision.")]
    public dog_mover dogMover;

    private SpriteRenderer _spriteRenderer;
    [Tooltip("The shadow's SpriteRenderer; if not assigned it will be searched for in children.")]
    public SpriteRenderer shadowSpriteRenderer;

    public void StartDistraction()
    {
        if (_state == TimmyState.StoppedAtA)
        {
            Debug.Log("timmy_mover: StartDistraction called but Timmy is already stopped at A.", this);
            return;
        }

        if (_state == TimmyState.Idle)
        {
            _state = TimmyState.GoingToB;
            Debug.Log("timmy_mover: StartDistraction sets state to GoingToB.", this);
        }
        else
        {
            Debug.Log($"timmy_mover: StartDistraction called while in state {_state}, no state change.", this);
        }

        dogReturned = false;
        UpdateSpriteFlip();
    }

    public void NotifyDogReturned()
    {
        if (globalVariables.TimmyReturned == true)
        {
            Debug.Log("timmy_mover: NotifyDogReturned called but GlobalVariables.TimmyReturned is already true.", this);
            return;
        }

        if (_state == TimmyState.WaitingAtB)
        {
            dogReturned = true;
            Debug.Log("timmy_mover: NotifyDogReturned sets dogReturned=true and will return to A on next Update.", this);
        }
        else
        {
            Debug.Log($"timmy_mover: NotifyDogReturned called but state is {_state} (ignored).", this);
        }
    }

    public void OnDogReturned()
    {
        Debug.Log("timmy_mover: OnDogReturned invoked (dog returned signal).", this);
        NotifyDogReturned();
    }

    private enum TimmyState
    {
        Idle,
        GoingToB,
        WaitingAtB,
        ReturningToA,
        StoppedAtA
    }

    private TimmyState _state;

    private bool IsMovingState => _state == TimmyState.GoingToB || _state == TimmyState.ReturningToA;

    private bool _movingTowardB => _state == TimmyState.GoingToB || _state == TimmyState.WaitingAtB;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (shadowSpriteRenderer == null)
        {
            shadowSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (shadowSpriteRenderer == _spriteRenderer)
                shadowSpriteRenderer = null;
        }

        _state = startAtA ? TimmyState.GoingToB : TimmyState.Idle;
        Debug.Log($"timmy_mover: Start({_state}, startAtA={startAtA})", this);
        UpdateSpriteFlip();
    }

    void Update()
    {

        if (GlobalVariables.TimmyReturned)
        {
            Destroy(this);

            if (_state == TimmyState.WaitingAtB) _state = TimmyState.ReturningToA;
        

            NotifyDogReturned();

        }



        // Set walker animation based on active movement state
        bool isWalking = IsMovingState;

        if (timmyAnimator != null)
            timmyAnimator.SetBool("timmywalk", isWalking);
        if (timmyshadowanimator != null)
            timmyshadowanimator.SetBool("timmywalk", isWalking);

        if (_state == TimmyState.WaitingAtB)
        {
            if (dogReturned)
            {
                _state = TimmyState.ReturningToA;
                Debug.Log("timmy_mover: dogReturned true, changing state WaitingAtB -> ReturningToA", this);
                UpdateSpriteFlip();
            }
            else
            {
                Debug.Log("timmy_mover: WaitingAtB, waiting for dogReturned = true", this);
            }
            return;
        }

        if (_state == TimmyState.StoppedAtA)
        {
            Debug.Log("timmy_mover: StoppedAtA, no movement.", this);
            return;
        }
        
        if (GlobalVariables.TimmyReturned == true)
        { _state = TimmyState.ReturningToA;
        return;
        }

        Transform currentTarget = GetCurrentTarget();
        if (currentTarget == null)
            return;

        UpdateSpriteFlip();

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentTarget.position;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            switch (_state)
            {
                case TimmyState.GoingToB:
                    _state = TimmyState.WaitingAtB;
                    dogReturned = false;
                    Debug.Log("timmy_mover: reached B, switching state GoingToB -> WaitingAtB", this);
                    break;

                case TimmyState.ReturningToA:
                    _state = TimmyState.StoppedAtA;
                    Debug.Log("timmy_mover: reached A, switching state ReturningToA -> StoppedAtA", this);
                    break;
            }

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
        
        switch (_state)
        {
        case TimmyState.ReturningToA:
            return targetA;
        case TimmyState.GoingToB:
        case TimmyState.WaitingAtB:
            return targetB;
        default:
            return null;
        }
    }

    // If Timmy collides with the dog while waiting at B, trigger dog return.
    private void OnCollisionEnter(Collision collision)
    {
        TryTriggerDogReturnOnCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryTriggerDogReturnOnCollision(other.gameObject);
    }

    private void TryTriggerDogReturnOnCollision(GameObject other)
    {
        if (_state != TimmyState.WaitingAtB) return;

        var dog = other.GetComponent<dog_mover>();
        if (dog == null)
            return;

        if (dogMover == null)
            dogMover = dog;

        if (dogMover != null)
        {
            dogMover.TriggerDogReturn();
            Debug.Log("timmy_mover: collided with dog at B; triggered dog return.", this);
        }
    }
}
