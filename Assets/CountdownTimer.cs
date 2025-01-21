using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign the text component to display the timer
    public float countdownTime = 5f; // Default countdown time

    public Action OnTimerStart; // Event triggered when the timer starts
    public Action<float> OnTimerUpdate; // Event triggered on timer updates
    public Action OnTimerEnd; // Event triggered when the timer ends

    private bool isRunning = false;

    // Start the countdown timer
    public void StartCountdown(float time = -1f)
    {
        if (time > 0)
        {
            countdownTime = time;
        }

        if (!isRunning)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    // Countdown coroutine
    public AudioSource tickingSound; // Sound for each second
    public AudioSource endSound; // Sound for timer completion

    private IEnumerator CountdownCoroutine()
    {
        isRunning = true;
        float timeRemaining = countdownTime;
        int lastSecond = Mathf.CeilToInt(timeRemaining); // Track the last whole second

        OnTimerStart?.Invoke();

        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                timerText.text = Mathf.Ceil(timeRemaining).ToString();
            }

            // Play ticking sound only when the second changes
            int currentSecond = Mathf.CeilToInt(timeRemaining);
            if (currentSecond != lastSecond)
            {
                if (tickingSound != null)
                {
                    tickingSound.Play();
                }
                lastSecond = currentSecond;
            }

            OnTimerUpdate?.Invoke(timeRemaining);

            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        if (timerText != null)
        {
            timerText.text = "0";
        }

        // Play end sound
        if (endSound != null)
        {
            endSound.Play();
        }

        OnTimerEnd?.Invoke();
        isRunning = false;
    }


    // Stop the countdown
    public void StopCountdown()
    {
        StopAllCoroutines();
        isRunning = false;
    }
}
