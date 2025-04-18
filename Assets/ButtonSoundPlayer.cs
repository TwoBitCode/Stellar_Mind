using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource targetAudioSource; // << Explicit audio source to use!

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (targetAudioSource == null)
        {
            Debug.LogError("ButtonSoundPlayer: No AudioSource assigned! Please assign one manually in the Inspector.");
            return;
        }

        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (targetAudioSource == null) return;

        AsteroidChallengeManager challengeManager = FindAnyObjectByType<AsteroidChallengeManager>();
        if (challengeManager == null)
        {
            Debug.LogError("No AsteroidChallengeManager found in the scene!");
            return;
        }

        var currentChallenge = challengeManager.CurrentChallenge;
        if (currentChallenge != null && currentChallenge.instructionAudioClip != null)
        {
            targetAudioSource.Stop();
            targetAudioSource.clip = currentChallenge.instructionAudioClip;
            targetAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No instruction audio clip found for this challenge.");
        }
    }
}
