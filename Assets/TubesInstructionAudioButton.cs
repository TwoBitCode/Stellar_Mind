using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TubesInstructionAudioButton : MonoBehaviour
{
    private AudioSource audioSource;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false;
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
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No instruction audio assigned for this stage.");
        }
    }
}
