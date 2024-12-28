using UnityEngine;
using TMPro;

public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Text component to display the welcome message")]
    [SerializeField] private TextMeshProUGUI welcomeMessage;

    [Header("Welcome Text")]
    [Tooltip("Default welcome message")]
    [SerializeField] private string defaultWelcomeText = "ברוך הבא למשחק!";

    private void Start()
    {
        // Set the welcome message when the scene starts
        InitializeWelcomeMessage();
    }

    private void InitializeWelcomeMessage()
    {
        // Check if the TextMeshPro component is assigned
        if (welcomeMessage != null)
        {
            // Enable right-to-left mode for Hebrew text
            welcomeMessage.isRightToLeftText = true;

            // Align the text to the right
            welcomeMessage.alignment = TextAlignmentOptions.Right;

            // Set the welcome message text
            welcomeMessage.text = defaultWelcomeText;
        }
        else
        {
            Debug.LogWarning("Welcome message TextMeshPro is not assigned.");
        }
    }
}
