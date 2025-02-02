using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TubesUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI sortingTimerText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Button checkAnswerButton;

    [Header("Failure Panel")]
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TextMeshProUGUI failureText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button strategyButton;

    [Header("Completion Panel")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private Button completionReturnToMapButton; // New button for completion panel
    [SerializeField] private TextMeshProUGUI completionBaseScoreText;
    [SerializeField] private TextMeshProUGUI completionBonusScoreText;
    [Header("Timer Background")]
    [SerializeField] private Image timerBackground;

    [Header("Stage Success Panel")]
    [SerializeField] private GameObject stageSuccessPanel;
    [SerializeField] private TextMeshProUGUI baseScoreText;
    [SerializeField] private TextMeshProUGUI bonusScoreText;
    [SerializeField] private Button nextStageButton;

    [Header("Strategy Panel")]
    [SerializeField] private StrategyManager strategyManager; 


    private int gameIndex = 0; // Tubes Game index (should match DoorManager setup)

    private void Start()
    {
        mainMenuButton.onClick.RemoveAllListeners();
        strategyButton.onClick.RemoveAllListeners();

        strategyButton.onClick.AddListener(() =>
        {
            if (strategyManager != null)
            {
                strategyManager.ShowNextStrategy();
            }
        });
    }
    public void ShowCompletionPanel(int baseScore, int bonusScore)
    {
        completionPanel.SetActive(true);
        completionBaseScoreText.text = $"{baseScore}";


        completionBonusScoreText.text = bonusScore.ToString();

        var playerProgress = GameProgressManager.Instance.playerProgress;

        if (playerProgress != null && playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            GameProgress gameProgress = playerProgress.gamesProgress[gameIndex];
            gameProgress.isCompleted = true;

            // Save the final overall score when game is completed
            playerProgress.totalScore = OverallScoreManager.Instance.OverallScore;
            GameProgressManager.Instance.SaveProgress();

            Debug.Log($"Game {gameIndex} is now marked as completed! Final Total Score: {playerProgress.totalScore}");
        }
        completionReturnToMapButton.onClick.RemoveAllListeners();
        completionReturnToMapButton.onClick.AddListener(() =>
        {
            Debug.Log("Returning to map from Completion Panel...");
            GameProgressManager.Instance.SaveProgress();
            ReturnToMap();
        });

        completionReturnToMapButton.gameObject.SetActive(true);
    }





    public void HideCompletionPanel()
    {
        completionPanel.SetActive(false);
    }

    public void ResetUI()
    {
        resultText.text = ""; // Clear any feedback
        countdownText.text = "";
        sortingTimerText.text = ""; // Clear the sorting timer
        ShowCheckButton(); // Always reset the "Check Result" button visibility
    }

    public void UpdateCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void UpdateSortingTimer(string text)
    {
        if (sortingTimerText != null)
        {
            sortingTimerText.text = text;
        }
    }

    public void UpdateResultText(string text)
    {
        resultText.text = text;
    }

    public void ShowInstructionPanel(string text)
    {
        instructionPanel.SetActive(true);
        instructionText.text = text;
    }

    public void HideInstructionPanel()
    {
        instructionPanel.SetActive(false);
    }

    public void ShowCheckButton()
    {
        checkAnswerButton.gameObject.SetActive(true);
    }

    public void HideCheckButton()
    {
        checkAnswerButton.gameObject.SetActive(false);
    }

    public void HideFailurePanel()
    {
        if (failurePanel != null)
        {
            failurePanel.SetActive(false);
        }
    }
    public void UpdateFailureText(string text)
    {
        if (failureText != null)
        {
            failureText.text = text; // עדכון טקסט השגיאה
        }
        else
        {
            Debug.LogWarning("Failure text UI element is not assigned in the TubesUIManager.");
        }
    }
    public void ChangeTimerBackgroundColor(Color newColor)
    {
        if (timerBackground != null)
        {
            timerBackground.color = newColor;
        }
        else
        {
            Debug.LogWarning("Timer background image is not assigned in the TubesUIManager.");
        }
    }
    public void ReturnToMap()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.SaveProgress();
        }

        Debug.Log("Returning to game selection map.");

        // Reset the game state completely to ensure fresh load
        GameManager.Instance.RestartGame();

        SceneManager.LoadScene("GameMapScene-V");
    }


    public void ShowFailurePanel(string text, System.Action retryAction)
    {
        if (failurePanel != null)
        {
            failurePanel.SetActive(true);
            failureText.text = text;
            retryButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.RemoveAllListeners();

            retryButton.onClick.AddListener(() => retryAction.Invoke());
            mainMenuButton.onClick.AddListener(() =>
            {
                GameProgressManager.Instance.SaveProgress();
                ReturnToMap();
            });
        }
    }
    public void ShowStageSuccessPanel(int baseScore, int bonusScore, System.Action onNextStage)
    {
        if (stageSuccessPanel != null)
        {
            stageSuccessPanel.SetActive(true);
            baseScoreText.text = $"{baseScore}";
            bonusScoreText.text = bonusScore.ToString();

            nextStageButton.onClick.RemoveAllListeners();
            nextStageButton.onClick.AddListener(() =>
            {
                stageSuccessPanel.SetActive(false);
                onNextStage.Invoke();
            });
        }
    }
    public int GetCurrentSortingTime()
    {
        int time = 0;
        int.TryParse(sortingTimerText.text.Replace("s", ""), out time);
        return time;
    }



}

