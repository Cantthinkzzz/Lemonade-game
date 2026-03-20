using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Call this method to quit the game
    public void Quit()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }
}