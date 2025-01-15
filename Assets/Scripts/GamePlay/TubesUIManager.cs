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
    [SerializeField] private Button restartButton;
    [SerializeField] private Button checkAnswerButton;

    public void ResetUI()
    {
        resultText.text = "";
        countdownText.text = "";
        checkAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Check Result";
        checkAnswerButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
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

    public void ShowRestartButton()
    {
        restartButton.gameObject.SetActive(true);
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
