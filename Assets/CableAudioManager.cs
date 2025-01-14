using UnityEngine;

public class CableAudioManager : MonoBehaviour
{
    public static CableAudioManager Instance; // Singleton instance

    [Header("Audio Clips")]
    public AudioClip dragSound; // Sound during drag
    public AudioClip snapSound; // Sound on correct connection
    public AudioClip resetSound; // Sound when resetting to start

    private AudioSource audioSource;

    void Awake()
    {
        // Ensure only one instance of the manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
