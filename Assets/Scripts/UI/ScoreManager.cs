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

        // Update OverallScoreManager
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(points);
        }
        else
        {
            Debug.LogError("OverallScoreManager is not available in the scene!");
        }

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
