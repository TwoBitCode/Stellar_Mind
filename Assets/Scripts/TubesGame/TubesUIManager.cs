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
    [SerializeField] private GameObject completionPanel;

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
        ShowCheckButton(); // Always reset the "Check Result" button visibility
    }

    public void UpdateCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void UpdateResultText(string text)
    {
        // Only updates the feedback text; does not touch the button
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
}
