using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public event Action OnTimerEnd; // Event triggered when the timer ends
    public event Action<float> OnTimerUpdate; // Event triggered on timer updates

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
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

    public void StartTimer()
    {
        gameStarted = true;
        timeRemaining = gameDuration;
        UpdateTimerText();
    }

    public void ResetTimer()
    {
        gameStarted = false;
        timeRemaining = gameDuration;
        UpdateTimerText();
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
