using UnityEngine;

public class GlobalAsteroidSoundManager : MonoBehaviour
{
    public static GlobalAsteroidSoundManager Instance;

    [Header("Sound Effects")]
    public AudioClip grabSound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayGrabSound()
    {
        PlaySound(grabSound);
    }

    public void PlayCorrectSound()
    {
        PlaySound(correctSound);
    }

    public void PlayIncorrectSound()
    {
        PlaySound(incorrectSound);
    }
}
