using UnityEngine;

using globalVariables = GlobalVariables; // Alias to avoid confusion with the class name

public class AudioControl1 : MonoBehaviour

{
    public AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to the GameObject
        audioSource = GetComponent<AudioSource>();

        StopAudio(); // Ensure the audio is stopped at the start
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

    void update()
    {
        if (!globalVariables.TimmyPlaneGiven)
        {
            StopAudio();
        }
        
        if (globalVariables.TimmyPlaneGiven)
        {
            PlayAudio();
        }
}}