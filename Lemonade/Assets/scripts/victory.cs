using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using globalVariables = GlobalVariables; 
using UnityEngine.SceneManagement;
public class anim : MonoBehaviour
{
    // Start is called before the first frame update

     public float timeRemaining = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GlobalVariables.PoisionedLemonade == true)
        {
           


        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        
        else
        {
            // Time has run out, do something here (e.g. end the game, reset the level, etc.)
            Debug.Log("Time's up!");
            SceneManager.LoadScene("Gameover");
                
        }
    }
}
        }
        