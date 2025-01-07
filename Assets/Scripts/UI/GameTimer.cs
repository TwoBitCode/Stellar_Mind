using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Total game time in seconds")]
    [SerializeField] private float gameDuration = 60f; // Total game time in seconds
    [SerializeField] private TextMeshProUGUI timerText; // Reference to a UI Text element to display the timer

    [Header("Timer Format Settings")]
    [SerializeField] private string timerFormat = "{0:00}:{1:00}"; // Timer format string

    [Header("Scene Management")]
    [SerializeField] private string gameOverSceneName = "GameOverScene"; // Name of the Game Over scene
    [SerializeField] private SceneTransitionManager sceneTransitionManager; // Reference to SceneTransitionManager

    [Header("Game Components")]
    [SerializeField] private ScoreManager scoreManager;  // Reference to the score manager
    [SerializeField] private SortingGameManager sortingGameManager; // Reference to the sorting game manager

    private float timeRemaining; // Time remaining in the game
    private bool isGameOver = false; // Flag to track if the game is over
    private bool gameStarted = false; // Flag to track if the game has started

    private void Start()
    {
        timeRemaining = gameDuration; // Initialize the remaining time
        UpdateTimerText(); // Update the timer display
    }

    private void Update()
    {
        if (isGameOver || !gameStarted) return; // Do nothing if the game is over or not started

        // Countdown logic
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame();
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        // Check if the timerText UI element is assigned
        if (timerText == null)
        {
            Debug.LogWarning("Timer text UI element is not assigned!");
            return; // Exit if there's no text component to update
        }

        // Calculate the number of full minutes remaining
        int minutes = Mathf.FloorToInt(timeRemaining / 60);

        // Calculate the remaining seconds after extracting minutes
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Format the time as MM:SS and update the UI text
        timerText.text = string.Format(timerFormat, minutes, seconds);
    }


    private void EndGame()
    {
        if (sortingGameManager != null)
        {
            sortingGameManager.StopGame(); // Stop the game
        }

        isGameOver = true;
        Debug.Log("Game Over!");

        if (sceneTransitionManager != null)
        {
            // Use the SceneTransitionManager to load the Game Over scene
            sceneTransitionManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned!");
        }
    }

    public void StartTimer()
    {
        gameStarted = true; // Mark the game as started
        isGameOver = false; // Reset game over flag
        timeRemaining = gameDuration; // Reset the timer
        UpdateTimerText(); // Ensure the timer text is updated immediately
    }

    public void ResetTimer()
    {
        // Reset the timer during gameplay if needed
        timeRemaining = gameDuration;
        UpdateTimerText();
    }
}
