using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public event Action OnTimerEnd; // Event triggered when the timer ends
    public event Action<float> OnTimerUpdate; // Event triggered on timer updates

    [Header("Game Settings")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool gameStarted;

    private void Update()
    {
        if (!gameStarted) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            gameStarted = false;
            OnTimerEnd?.Invoke(); // Notify listeners that the timer has ended
        }

        OnTimerUpdate?.Invoke(timeRemaining); // Notify listeners of time updates
        UpdateTimerText();
    }

    // Starts the timer with the previously set duration
    public void StartTimer()
    {
        if (timeRemaining <= 0)
        {
            Debug.LogWarning("Timer duration is zero or negative. Did you forget to set it?");
            return;
        }

        gameStarted = true;
        UpdateTimerText();
    }

    // Stops the timer and resets it to the set duration
    public void ResetTimer()
    {
        gameStarted = false;
        UpdateTimerText();
    }

    // Sets the duration for the timer
    public void SetDuration(float duration)
    {
        if (duration <= 0)
        {
            Debug.LogWarning("Timer duration must be greater than zero.");
            return;
        }

        timeRemaining = duration; // Update remaining time immediately
        UpdateTimerText();

        Debug.Log($"Timer duration set to {duration} seconds.");
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
