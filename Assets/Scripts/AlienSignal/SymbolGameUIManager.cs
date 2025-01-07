using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SymbolGameUIManager : MonoBehaviour
{
    public static SymbolGameUIManager Instance; // Singleton for global access

    [Header("UI Canvases")]
    public Canvas learningCanvas; // Learning UI Canvas
    public Canvas practiceCanvas; // Practice UI Canvas

    [Header("Text Elements")]
    public TMP_Text instructionText; // Text for instructions
    public TMP_Text feedbackText; // Text for feedback
    public TMP_Text strategyText; // Text for strategies
    public TMP_Text learningScoreText; // Score for learning canvas
    public TMP_Text practiceScoreText; // Score for practice canvas

    [Header("Answer Buttons")]
    public Button[] answerButtons; // Buttons for practice answers

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate managers
        }
    }

    void Update()
    {
        UpdateScoreDisplay(); // Continuously update score display
    }

    public void ShowLearningUI()
    {
        learningCanvas.gameObject.SetActive(true);
        practiceCanvas.gameObject.SetActive(false);
        ResetUI();

        // Adjust instructions for the current mode
        if (SymbolPracticeManager.Instance != null && SymbolPracticeManager.Instance.isVoiceMode)
        {
            instructionText.text = "Listen carefully to learn the voice meanings!";
        }
        else
        {
            instructionText.text = "Observe carefully to learn the symbol meanings!";
        }
    }

    public void ShowPracticeUI()
    {
        learningCanvas.gameObject.SetActive(false);
        practiceCanvas.gameObject.SetActive(true);
        ResetUI();
    }

    public void DisplayQuestion(string meaning)
    {
        instructionText.text = $"What symbol represents: {meaning}?";
        feedbackText.text = "";
        strategyText.text = "";
    }

    public void DisplayFeedback(bool isCorrect)
    {
        feedbackText.text = isCorrect ? "Correct! Great job!" : "Oops! Try again.";
        feedbackText.color = isCorrect ? Color.green : Color.red;
    }

    public void DisplayStrategy(string strategy)
    {
        strategyText.text = $"Robot's Tip: {strategy}";
    }

    public void DisplayCompletion()
    {
        instructionText.text = "Great job! You've completed the practice session!";
        feedbackText.text = "Level Complete!";
        feedbackText.color = Color.green;
    }

    public void DisableAnswerButtons()
    {
        foreach (Button button in answerButtons)
        {
            button.interactable = false; // Disable interaction
        }
    }

    public void ResetUI()
    {
        instructionText.text = "";
        feedbackText.text = "";
        strategyText.text = "";
    }

    private void UpdateScoreDisplay()
    {
        if (OverallScoreManager.Instance != null)
        {
            string scoreText = "Score: " + OverallScoreManager.Instance.OverallScore.ToString();

            if (learningScoreText != null)
            {
                learningScoreText.text = scoreText;
            }

            if (practiceScoreText != null)
            {
                practiceScoreText.text = scoreText;
            }
        }
    }
}
