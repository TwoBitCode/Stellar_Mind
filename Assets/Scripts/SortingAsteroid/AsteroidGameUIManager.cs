using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidGameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] private GameObject stageSuccessPanel;
    [SerializeField] private TextMeshProUGUI baseScoreText;
    [SerializeField] private TextMeshProUGUI bonusScoreText;
    [SerializeField] private Button nextStageButton;

    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI completionBaseScoreText;
    [SerializeField] private TextMeshProUGUI completionBonusScoreText;
    [SerializeField] private Button returnToMapButton;

    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TextMeshProUGUI failureMessageText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button strategyButton;

    [Header("Instruction Sound Button")]
    [SerializeField] private Button playInstructionsAudioButton;
    [SerializeField] private AudioSource instructionsAudioSource;
    [SerializeField] private List<AudioClip> instructionAudioClips; // One per challenge

    [SerializeField] private StrategyManager strategyManager;
    [SerializeField] private Button startChallengeButton;

    public float SelectedDuration { get; private set; } = 0f;
    public void ShowInstructions(string instructions, System.Action onStartGame)
    {
        if (instructionsPanel == null || instructionsText == null)
        {
            Debug.LogError("Instructions panel or text is not assigned in the UI Manager!");
            return;
        }

        instructionsPanel.SetActive(true);
        instructionsText.text = instructions;

        Button startButton = instructionsPanel.GetComponentInChildren<Button>();
        if (startButton == null)
        {
            Debug.LogError("Start button not found in the instructions panel!");
            return;
        }

        startButton.interactable = false;

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            if (instructionsAudioSource != null && instructionsAudioSource.isPlaying)
            {
                instructionsAudioSource.Stop();
            }

            instructionsPanel.SetActive(false); // הסתרת פאנל ההוראות
            onStartGame?.Invoke();              // התחלת המשחק
        });

        if (playInstructionsAudioButton != null)
        {
            playInstructionsAudioButton.onClick.RemoveAllListeners();
            playInstructionsAudioButton.onClick.AddListener(PlayInstructionsAudio);
        }
    }



    public void UpdateTimer(string timeText)
    {
        if (timerText != null)
        {
            timerText.text = timeText;
        }
        else
        {
            Debug.LogWarning("Timer text is not assigned in the UI Manager!");
        }
    }

    public void ShowStageSuccessPanel(int baseScore, int bonusScore, System.Action onNextStage)
    {
        if (stageSuccessPanel != null)
        {
            stageSuccessPanel.SetActive(true);
            baseScoreText.text = $"{baseScore}";
            bonusScoreText.text = bonusScore > 0 ? $"{bonusScore}" : "0";

            nextStageButton.onClick.RemoveAllListeners();
            nextStageButton.onClick.AddListener(() =>
            {
                StartCoroutine(TransitionToNextStage(onNextStage));
            });
        }
    }

    private IEnumerator TransitionToNextStage(System.Action onNextStage)
    {
        // Show instructions immediately to prevent blank screen
        onNextStage.Invoke();

        // Wait briefly before hiding the success panel to avoid overlap
        yield return new WaitForSeconds(0.3f);

        stageSuccessPanel.SetActive(false);
    }


    public void ShowCompletionPanel(int baseScore, int bonusScore, System.Action returnToMap)
    {
        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            completionBaseScoreText.text = $"{baseScore}";
            completionBonusScoreText.text = bonusScore > 0 ? $"{bonusScore}" : "0";

            returnToMapButton.onClick.RemoveAllListeners();
            returnToMapButton.onClick.AddListener(() =>
            {
                completionPanel.SetActive(false);
                returnToMap.Invoke();
            });
        }
    }


    public void HideSuccessPanel()
    {
        if (stageSuccessPanel != null)
        {
            stageSuccessPanel.SetActive(false);
        }
    }

    public void ShowFailurePanel(string message, System.Action retryAction, System.Action returnToMenu)
    {
        HideAllUI(); // Hide other panels

        if (failurePanel != null)
        {
            failurePanel.SetActive(true);

            TextMeshProUGUI failureText = failurePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (failureText != null)
            {
                failureText.text = message;
            }

            Button[] buttons = failurePanel.GetComponentsInChildren<Button>();
            if (buttons.Length >= 3) // וידוא שיש מספיק כפתורים בפאנל
            {
                // Retry button
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() =>
                {
                    failurePanel.SetActive(false); // Hide the failure panel
                    retryAction?.Invoke(); // Call the retry action
                });

                // Return to menu button
                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(() =>
                {
                    failurePanel.SetActive(false); // Hide the failure panel
                    returnToMenu?.Invoke(); // Call the return-to-menu action
                });

                // Strategy button - כפתור להצגת פאנל האסטרטגיות
                buttons[2].onClick.RemoveAllListeners();
                buttons[2].onClick.AddListener(() =>
                {
                    if (strategyManager != null)
                    {
                        strategyManager.ShowNextStrategy(); // מציג את פאנל האסטרטגיות
                    }
                });
            }
            else
            {
                Debug.LogWarning("Not enough buttons assigned in the failure panel!");
            }
        }
        else
        {
            Debug.LogWarning("Failure panel is not assigned in the UI Manager!");
        }
    }


    public void HideFailurePanel()
    {
        if (failurePanel != null)
        {
            failurePanel.SetActive(false);
        }
    }

    public void HideAllUI()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (stageSuccessPanel != null) stageSuccessPanel.SetActive(false);
        if (completionPanel != null) completionPanel.SetActive(false);
        if (failurePanel != null) failurePanel.SetActive(false);
    }

    public void ShowTimerText()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }
    public void HideTimerText()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }
    private void PlayInstructionsAudio()
    {
        if (instructionsAudioSource == null) return;

        AsteroidChallengeManager challengeManager = FindAnyObjectByType<AsteroidChallengeManager>();
        if (challengeManager == null)
        {
            Debug.LogError("No AsteroidChallengeManager found in the scene!");
            return;
        }

        var currentChallenge = challengeManager.CurrentChallenge;
        if (currentChallenge != null && currentChallenge.instructionAudioClip != null)
        {
            instructionsAudioSource.Stop();
            instructionsAudioSource.clip = currentChallenge.instructionAudioClip;
            instructionsAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No instruction audio clip found for this challenge.");
        }
    }
    public void SelectDuration(float duration)
    {
        SelectedDuration = duration;
        Debug.Log($"Selected duration set to: {duration}");

        if (startChallengeButton != null)
            startChallengeButton.interactable = true;
    }



}
