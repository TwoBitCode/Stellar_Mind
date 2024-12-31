using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the score UI text

    public int CurrentScore => currentScore;

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        Debug.Log($"Score updated: {currentScore}");
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        Debug.Log("Score reset to 0");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
        else
        {
            Debug.LogWarning("ScoreText is not assigned in ScoreManager!");
        }
    }
}
