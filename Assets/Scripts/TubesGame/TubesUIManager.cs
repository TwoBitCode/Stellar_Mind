using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TubesUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Button checkAnswerButton;
    [Header("Completion Panel")]
    [SerializeField] private GameObject completionPanel; // Add reference for the completion panel

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
        checkAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Check Result";
        checkAnswerButton.gameObject.SetActive(true);
    }

    public void UpdateCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void UpdateResultText(string text, bool isCorrect)
    {
        resultText.text = text;
        if (isCorrect)
        {
            checkAnswerButton.gameObject.SetActive(false);
        }
        else
        {
            checkAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Try Again";
        }
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
}
