using UnityEngine;

using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public class AudioControl : MonoBehaviour

{
    public AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Method to play the audio
    public void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Method to stop the audio
    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void Update()
    {
        if (globalVariables.TimmyPlaneGiven)
        {
            StopAudio();
        }
    }
}