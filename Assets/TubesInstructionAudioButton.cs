using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TubesInstructionAudioButton : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource instructionAudioSource; // <-- Specific AudioSource you assign manually

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (instructionAudioSource == null)
        {
            Debug.LogWarning("Instruction AudioSource not assigned. Trying to find one on the GameObject.");
            instructionAudioSource = GetComponent<AudioSource>();

            if (instructionAudioSource == null)
            {
                instructionAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        instructionAudioSource.loop = false;
        button.onClick.AddListener(PlayInstructionAudio);
    }

    private void PlayInstructionAudio()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance not found.");
            return;
        }

        var gameManager = GameManager.Instance;
        var stages = gameManager.GetStages();
        int index = gameManager.GetCurrentStageIndex();

        if (index < 0 || index >= stages.Count)
        {
            Debug.LogError("Invalid stage index.");
            return;
        }

        AudioClip clip = stages[index].instructionAudioClip;

        if (clip != null)
        {
            instructionAudioSource.Stop();
            instructionAudioSource.clip = clip;
            instructionAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No instruction audio assigned for this stage.");
        }
    }
}
