using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    // Constants for timer durations and formatting
    private const string GAME_OVER_SCENE_NAME = "GameOverScene"; // Name of the Game Over scene
    private const string TIMER_FORMAT = "{0:00}:{1:00}"; // Timer format string

    [Header("Game Settings")]
    [Tooltip("Total game time in seconds")]
    [SerializeField] private float gameDuration = 60f; // Total game time in seconds
    public TextMeshProUGUI timerText; // Reference to a UI Text element to display the timer

    private float timeRemaining;
    private bool isGameOver = false;
    private bool gameStarted = false; // Flag to track if the game has started

    public SceneSwitcher sceneSwitcher;

    private void Start()
    {
        timeRemaining = gameDuration;
        UpdateTimerText();
    }

    private void Update()
    {
        if (isGameOver || !gameStarted) return; // If the game is over or hasn't started, don't count down

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
        if (timerText != null)
        {
            // Format the time as MM:SS
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format(TIMER_FORMAT, minutes, seconds);
        }
    }

    private void EndGame()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
        // OnGameOver?.Invoke(); // Trigger the Game Over event
        if (sceneSwitcher != null)
        {
            sceneSwitcher.LoadSceneByName(GAME_OVER_SCENE_NAME);  // Load the Game Over scene
        }
        else
        {
            Debug.LogError("SceneSwitcher not found on this GameObject!");
        }
    }

    public void StartTimer()
    {
        gameStarted = true; // Set the game as started
        isGameOver = false; // Ensure the game is not over when starting
        timeRemaining = gameDuration; // Reset the timer to full value

        // The timer is already running in the Update() method, so no need to add extra code here.
    }

    public void ResetTimer()
    {
        // Optionally, if you want to reset the timer at any point during the game:
        timeRemaining = gameDuration;
        UpdateTimerText();
    }
}