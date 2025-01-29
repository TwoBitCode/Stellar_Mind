using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("Completion Panel")]
    [SerializeField] private GameObject completionPanel;
    [Header("Timer Background")]
    [SerializeField] private Image timerBackground;

    public void ShowCompletionPanel()
    {
        completionPanel.SetActive(true);
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

    public void ShowFailurePanel(string text, System.Action retryAction, System.Action mainMenuAction)
    {
        if (failurePanel != null)
        {
            failurePanel.SetActive(true);
            failureText.text = text;
            retryButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.RemoveAllListeners();

            retryButton.onClick.AddListener(() => retryAction.Invoke());
            mainMenuButton.onClick.AddListener(() => mainMenuAction.Invoke());
        }
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
}

