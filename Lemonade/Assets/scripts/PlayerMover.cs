using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CharacterControl : MonoBehaviour
{

    public Camera cam;
    public NavMeshAgent player;
    public Animator playerAnimator;
    public Animator shadowAnimator;
    public GameObject targetDest;

    public bool isWalking;
   


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPoint;

            if(Physics.Raycast(ray, out hitPoint))
            {
                targetDest.transform.position = hitPoint.point;
                player.SetDestination(hitPoint.point);

               
            }
        }
               
            
  
        if (player.velocity != Vector3.zero)
        {
            playerAnimator.SetBool("isWalking", true);
            shadowAnimator.SetBool("isWalking", true);

            
        }
        else if (player.velocity == Vector3.zero)
        {
            playerAnimator.SetBool("isWalking", false);
            shadowAnimator.SetBool("isWalking", false);
            
        }

    }

           
        }