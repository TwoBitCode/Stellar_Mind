using UnityEngine;
using TMPro;

public class EquipmentRecoveryUIManager : MonoBehaviour
{
    public static EquipmentRecoveryUIManager Instance; // Singleton for global access

    [Header("UI Elements")]
    public GameObject dialoguePanel; // Panel for the character and text
    public TextMeshProUGUI dialogueText; // Text component for the character's instructions
    public GameObject workspaceStartButton; // Button to start the interaction in the workspace

    [Header("Feedback Elements")]
    public TextMeshProUGUI feedbackText; // Reference to the feedback text

    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public string[] workspaceInstructions; // Instructions for the workspace
    public float textDisplayDuration = 2f; // Duration for each instruction to display
    [Header("Reward UI")]
    public GameObject rewardPanel; // Panel for stage completion rewards
    public TextMeshProUGUI rewardText; // Text to display points and bonus
    public UnityEngine.UI.Button continueButton; // Button to proceed to the next stage
    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Panel that appears when time runs out
    public UnityEngine.UI.Button restartButton;
    public UnityEngine.UI.Button returnToMapButton;
    [Header("Level Complete UI")]
    public GameObject levelCompletePanel; // The panel that appears when the final stage is completed
    public TextMeshProUGUI levelCompleteText; // Text to display final stage points and bonus
    public UnityEngine.UI.Button levelCompleteButton; // Button to exit or return to the map

    private int currentInstructionIndex = 0;
    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Hide at start
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                if (EquipmentRecoveryGameManager.Instance != null)
                {
                    EquipmentRecoveryGameManager.Instance.RestartStage();
                }
            });
        }

        if (returnToMapButton != null)
        {
            returnToMapButton.onClick.AddListener(() =>
            {
                if (EquipmentRecoveryGameManager.Instance != null)
                {
                    EquipmentRecoveryGameManager.Instance.ReturnToMap();
                }
            });
        }
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(() => ProceedToNextStage());
        }
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false); // Ensure it starts hidden
        }

        if (levelCompleteButton != null)
        {
            levelCompleteButton.onClick.AddListener(() =>
            {
                if (EquipmentRecoveryGameManager.Instance != null)
                {
                    EquipmentRecoveryGameManager.Instance.ReturnToMap();
                }
                else
                {
                    Debug.LogError("EquipmentRecoveryGameManager.Instance is NULL!");
                }
            });
        }



    }


    private void Awake()
    {
        // Ensure only one instance of EquipmentRecoveryUIManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this manager across scenes if needed
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Displays feedback text with the specified message and color
    public void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message; // Set the text message
            feedbackText.color = color; // Set the color of the text

            // Hide the feedback after a delay (default is 2 seconds)
            CancelInvoke(nameof(HideFeedback));
            Invoke(nameof(HideFeedback), 2f); // Adjust the delay if needed
        }
        else
        {
            Debug.LogWarning("FeedbackText is not assigned in EquipmentRecoveryUIManager!");
        }
    }

    // Clears and hides the feedback text
    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = ""; // Clear the text
        }
    }

    // Starts the workspace instructions dialogue
    public void StartWorkspaceInstructions()
    {
        Debug.Log("Starting workspace instructions...");

        currentInstructionIndex = 0; // Reset the instruction index
        dialoguePanel.SetActive(true); // Show the dialogue panel
        workspaceStartButton.SetActive(false); // Hide the start button initially

        ShowNextInstruction();
    }

    private void ShowNextInstruction()
    {
        if (currentInstructionIndex < workspaceInstructions.Length)
        {
            Debug.Log($"Showing instruction {currentInstructionIndex + 1}: {workspaceInstructions[currentInstructionIndex]}");
            dialogueText.text = workspaceInstructions[currentInstructionIndex]; // Update text
            currentInstructionIndex++;

            // Schedule the next instruction
            if (currentInstructionIndex < workspaceInstructions.Length)
            {
                Invoke(nameof(ShowNextInstruction), textDisplayDuration);
            }
            else
            {
                // Show the start button after the last instruction
                Invoke(nameof(EnableWorkspaceStartButton), textDisplayDuration);
            }
        }
    }


    // Enables the workspace start button
    private void EnableWorkspaceStartButton()
    {
        workspaceStartButton.SetActive(true); // Show the start button
    }

    // Handles the start of the workspace interaction
    public void OnWorkspaceStartClicked()
    {
        dialoguePanel.SetActive(false); // Hide the dialogue panel
        workspaceStartButton.SetActive(false); // Hide the start button

        Debug.Log("Player can now interact with the robot parts!");

        // Start the first stage
        if (EquipmentRecoveryGameManager.Instance != null)
        {
            Debug.Log("Calling StartStage() from UIManager...");
            EquipmentRecoveryGameManager.Instance.StartStage();
        }
        else
        {
            Debug.LogError("EquipmentRecoveryGameManager.Instance is null! Cannot start the stage.");
        }
    }
    public void ShowRewardPanel(int stagePoints, int bonusPoints)
    {
        if (rewardPanel != null && rewardText != null)
        {
            rewardPanel.SetActive(true);
            rewardText.text = $"Stage Points: {stagePoints}\nBonus Points: {bonusPoints}\nTotal Earned: {stagePoints + bonusPoints}";
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
        if (levelCompletePanel != null && levelCompleteText != null)
        {
            levelCompletePanel.SetActive(true);
            levelCompleteText.text = $"Final Stage Points: {stagePoints}\nBonus Points: {bonusPoints}\nTotal Earned: {stagePoints + bonusPoints}";
        }
    }


}
