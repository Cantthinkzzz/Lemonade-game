using UnityEngine;
using UnityEngine.AI;
using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public class MoveToTarget : MonoBehaviour
{
    public Transform target; // The target object to move towards
    private NavMeshAgent navMeshAgent;

    public Animator timmyAnimator;
    public Animator timmyshadowanimator;

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
           
        if (GlobalVariables.TimmyReturned)
        {
           
        
        // Check if the target is assigned
        if (target != null)
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
}

}
}
