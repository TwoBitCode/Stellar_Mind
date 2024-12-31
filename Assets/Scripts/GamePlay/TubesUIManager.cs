using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI countdownText;
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
            checkAnswerButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
        }
    }

    public void ShowRestartButton()
    {
        restartButton.gameObject.SetActive(true);
    }
}
