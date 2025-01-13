using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public static AudioFeedbackManager Instance;

    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip dragStartSound; // Add this for drag sounds

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of this manager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure there is an AudioSource attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayCorrectSound()
    {
        PlaySound(correctSound);
    }

    public void PlayIncorrectSound()
    {
        PlaySound(incorrectSound);
    }

    public void PlayDragStartSound()
    {
        PlaySound(dragStartSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip is not assigned in AudioFeedbackManager.");
        }
    }
}
