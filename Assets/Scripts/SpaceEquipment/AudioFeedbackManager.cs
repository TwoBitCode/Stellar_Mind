using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public static AudioFeedbackManager Instance; // Restore Instance

    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip dragStartSound;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this; // Assign Instance
        Debug.Log("AudioFeedbackManager started!");

        // Ensure AudioSource is assigned
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
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip is not assigned or AudioSource is missing in AudioFeedbackManager.");
        }
    }
}
