using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    // Call this method to load the main menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("title screen"); 
    }
}