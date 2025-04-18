using UnityEngine;
using TMPro;

public class StrategyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI strategyText;
    public GameObject strategyPanel;

    [Header("Strategies")]
    [TextArea(2, 5)]
    public string[] strategies;
    public AudioClip[] strategyAudioClips; // <-- NEW: Audio clips for each strategy

    [Header("Audio")]
    public AudioSource audioSource; // <-- NEW: AudioSource to play strategy audios

    private int currentStrategyIndex = 0;

    private void Start()
    {
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(false);
        }

        if (strategies.Length == 0)
        {
            Debug.LogWarning("No strategies found! Please add strategies in the Inspector.");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void ShowNextStrategy()
    {
        if (strategies.Length == 0)
        {
            Debug.LogWarning("No strategies available to show.");
            return;
        }

        if (strategyPanel != null)
        {
            strategyPanel.SetActive(true);
        }

        strategyText.text = strategies[currentStrategyIndex];


        currentStrategyIndex = (currentStrategyIndex + 1) % strategies.Length;
    }

    public void PlayCurrentStrategyAudio()
    {
        PlayStrategyAudio(currentStrategyIndex == 0 ? strategies.Length - 1 : currentStrategyIndex - 1);
    }

    private void PlayStrategyAudio(int index)
    {
        if (strategyAudioClips != null && index >= 0 && index < strategyAudioClips.Length)
        {
            audioSource.clip = strategyAudioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"No audio clip found for strategy index {index}.");
        }
    }

    public void HideStrategyPanel()
    {
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(false);
        }
    }

    public void ShowStrategyPanel()
    {
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(true);
        }
    }
}
