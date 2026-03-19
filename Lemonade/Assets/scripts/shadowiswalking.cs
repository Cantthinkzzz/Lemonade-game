using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowiswalking : MonoBehaviour
{

    public bool isWalking;

void Update()
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
