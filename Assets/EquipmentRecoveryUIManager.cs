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

    private int currentInstructionIndex = 0;

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
        currentInstructionIndex = 0; // Reset the instruction index
        dialoguePanel.SetActive(true); // Show the dialogue panel
        workspaceStartButton.SetActive(false); // Hide the start button initially

        ShowNextInstruction();
    }

    // Displays the next instruction in the workspace
    private void ShowNextInstruction()
    {
        if (currentInstructionIndex < workspaceInstructions.Length)
        {
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
        // Additional logic to enable interactions can go here
    }
}
