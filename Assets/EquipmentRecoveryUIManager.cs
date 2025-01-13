using UnityEngine;
using TMPro;

public class EquipmentRecoveryUIManager : MonoBehaviour
{
    public static EquipmentRecoveryUIManager Instance; // Singleton for global access
    public TextMeshProUGUI feedbackText; // Reference to the feedback text

    private void Awake()
    {
        // Ensure only one instance of the EquipmentRecoveryUIManager exists
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
}
