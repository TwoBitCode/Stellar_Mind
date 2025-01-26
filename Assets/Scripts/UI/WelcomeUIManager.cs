using UnityEngine;
using TMPro;

public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField playerNameInput;
    //[SerializeField] private TMP_InputField targetScoreInput;
    [SerializeField] private TextMeshProUGUI welcomeMessage;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        InitializeWelcomeMessage();
        startButton.SetActive(false); // Hide start button by default
    }

    private void InitializeWelcomeMessage()
    {
        //welcomeMessage.isRightToLeftText = true;
        //welcomeMessage.alignment = TextAlignmentOptions.Right;
        //welcomeMessage.text = "ברוך הבא למשחק! הזן את שמך וניקוד המטרה.";
    }

    public void UpdateStartButtonVisibility(bool isEnabled)
    {
        startButton.SetActive(isEnabled);
    }
}
