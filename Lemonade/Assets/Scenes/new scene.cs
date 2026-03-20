using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Method to load a scene by its name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}