using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EquipmentRecoveryUIManager : MonoBehaviour
{
    public static EquipmentRecoveryUIManager Instance; // Singleton for global access

    [Header("UI Elements")]
    public GameObject dialoguePanel; // Panel for the character and text
    //public TextMeshProUGUI dialogueText; // Text component for the character's instructions
    public GameObject workspaceStartButton; // Button to start the stage

    [Header("Feedback Elements")]
    public TextMeshProUGUI feedbackText;

    [Header("Reward UI")]
    public GameObject rewardPanel;
    public TextMeshProUGUI rewardBaseScoreText;
    public TextMeshProUGUI rewardBonusScoreText;
    public UnityEngine.UI.Button continueButton;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Button restartButton;
    public UnityEngine.UI.Button returnToMapButton;
    public UnityEngine.UI.Button strategyButton;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteBaseScoreText;
    public TextMeshProUGUI levelCompleteBonusScoreText;
    public UnityEngine.UI.Button levelCompleteButton;
    public UnityEngine.UI.Button levelCompleteReturnToMapButton; // NEW: Return to Map Button in Level Complete
    public UnityEngine.UI.Button levelCompleteStrategyButton;    // NEW: Strategy Button in Level Complete


    [Header("Strategy Panel")]
    public StrategyManager strategyManager;

    public AudioSource instructionAudioSource;


    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rewardPanel != null) rewardPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                EquipmentRecoveryGameManager.Instance?.RestartStage();
            });
        }

        if (returnToMapButton != null)
        {
            returnToMapButton.onClick.AddListener(() =>
            {
                EquipmentRecoveryGameManager.Instance?.ReturnToMap();
            });
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() => ProceedToNextStage());
        }

        if (levelCompleteButton != null)
        {
            levelCompleteButton.onClick.AddListener(() =>
            {
                EquipmentRecoveryGameManager.Instance?.ReturnToMap();
            });
        }

        // Ensure workspace start button is always visible
        if (workspaceStartButton != null)
        {
            workspaceStartButton.SetActive(true);
            workspaceStartButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnWorkspaceStartClicked());
        }

        // Ensure dialogue panel is shown immediately
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        if (strategyButton != null)
        {
            strategyButton.onClick.AddListener(() =>
            {
                if (strategyManager != null)
                {
                    strategyManager.ShowNextStrategy();
                }
            });
        }
        if (levelCompleteReturnToMapButton != null)
        {
            levelCompleteReturnToMapButton.onClick.AddListener(() =>
            {
                EquipmentRecoveryGameManager.Instance?.ReturnToMap();
            });
        }

        if (levelCompleteStrategyButton != null)
        {
            levelCompleteStrategyButton.onClick.AddListener(() =>
            {
                if (strategyManager != null)
                {
                    strategyManager.ShowNextStrategy();
                }
            });
        }

    }

    private void Awake()
    {
        Instance = this;
        Debug.Log("EquipmentRecoveryUIManager started!");
    }

    public void OnWorkspaceStartClicked()
    {
        dialoguePanel.SetActive(false); // Hide the dialogue panel
       // workspaceStartButton.SetActive(false); // Hide the start button

        // Stop intro dialogue audio
        var intro = FindAnyObjectByType<EquipmentRecoveryIntro>();
        if (intro != null && intro.dialogueAudioSource != null && intro.dialogueAudioSource.isPlaying)
        {
            intro.dialogueAudioSource.Stop();
            Debug.Log("Stopped intro dialogue audio on workspace start.");
        }

        // Stop instruction audio (direct reference)
        if (instructionAudioSource != null && instructionAudioSource.isPlaying)
        {
            instructionAudioSource.Stop();
            Debug.Log("Stopped instruction audio from assigned AudioSource.");
        }

        Debug.Log("Player can now interact with the robot parts!");

        if (EquipmentRecoveryGameManager.Instance != null)
        {
            //EquipmentRecoveryGameManager.Instance.StartStage();
        }
        else
        {
            Debug.LogError("EquipmentRecoveryGameManager.Instance is null! Cannot start the stage.");
        }
    }



    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;

            CancelInvoke(nameof(HideFeedback));
            Invoke(nameof(HideFeedback), 1f);
        }
        else
        {
            Debug.LogWarning("FeedbackText is not assigned in EquipmentRecoveryUIManager!");
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    public void ShowRewardPanel(int stagePoints, int bonusPoints)
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(true);
            rewardBaseScoreText.text = $"{stagePoints}";
            rewardBonusScoreText.text = $"{bonusPoints}";
        }
    }

    public void HideRewardPanel()
    {
        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }
    }

    public void ProceedToNextStage()
    {
        Debug.Log("Continue button clicked - proceeding to next stage.");

        if (EquipmentRecoveryGameManager.Instance != null)
        {
            EquipmentRecoveryGameManager.Instance.NextStage();
        }
        else
        {
            Debug.LogError("EquipmentRecoveryGameManager.Instance is NULL!");
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowLevelCompletePanel(int stagePoints, int bonusPoints)
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            levelCompleteBaseScoreText.text = $"{stagePoints}";
            levelCompleteBonusScoreText.text = $"{bonusPoints}";
        }
    }
}
