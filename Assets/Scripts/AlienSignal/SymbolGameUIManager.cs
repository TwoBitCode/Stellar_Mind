using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SymbolGameUIManager : MonoBehaviour
{
    public static SymbolGameUIManager Instance; // Singleton for global access

    [Header("Canvases")]
    public Canvas learningCanvas; // Learning UI Canvas
    public Canvas practiceCanvas; // Practice UI Canvas

    [Header("UI Text Elements")]
    public TMP_Text instructionText; // Text for instructions
    public TMP_Text feedbackText; // Text for feedback
    public TMP_Text strategyText; // Text for strategies
    public TMP_Text learningScoreText; // Score for learning canvas
    public TMP_Text practiceScoreText; // Score for practice canvas

    [Header("Buttons")]
    public Button[] answerButtons; // Buttons for practice answers

    [Header("Audio")]
    public Sprite soundIcon; // Assign a generic sound icon in the Unity Inspector
    public AudioSource feedbackAudioSource; // Audio source for feedback sounds
    public AudioClip correctSound; // Sound played for correct answers
    public AudioClip incorrectSound; // Sound played for incorrect answers

    [Header("Visual Effects")]
    public ParticleSystem correctAnswerEffect; // Particle effect for correct answers

    [Header("Progress Bar")]
    public Slider progressBar; // Progress bar for level tracking
    private float progressIncrement = 0.1f; // Amount to increment per correct answer

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevent duplicate managers
    }

    void Update()
    {
        UpdateScoreDisplay();
    }

    public void ShowLearningUI()
    {
        learningCanvas.gameObject.SetActive(true);
        practiceCanvas.gameObject.SetActive(false);
        ResetUI();

        // Adjust instructions for the current mode
        if (SymbolPracticeManager.Instance.isVoiceMode)
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

        // Play feedback sound
        if (feedbackAudioSource != null)
        {
            feedbackAudioSource.clip = isCorrect ? correctSound : incorrectSound;
            feedbackAudioSource.Play();
        }

        // Play particle effect for correct answers
        if (isCorrect && correctAnswerEffect != null)
        {
            correctAnswerEffect.Play();
        }

        // Update progress bar on correct answers
        if (isCorrect && progressBar != null)
        {
            progressBar.value += progressIncrement;
        }
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

        // Reset progress bar
        if (progressBar != null)
        {
            progressBar.value = 0;
        }
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
