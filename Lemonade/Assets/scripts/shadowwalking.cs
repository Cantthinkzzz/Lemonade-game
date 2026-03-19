using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowwalking : MonoBehaviour
{

    public bool isWalking;

void update()
    {
        if (isWalking==true)
        {
            GetComponent<Animator>().SetBool("isWalking", true);
        }
        else if (isWalking==false)
        {
            GetComponent<Animator>().SetBool("isWalking", false);
        }
    }
   
}
