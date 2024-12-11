using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Constants
    private const string SCORE_LABEL = "Score: ";

    public TextMeshProUGUI scoreText;  // Reference to the UI Text element to display the score
    private int score = 0;  // The player's score

    private void Start()
    {
        UpdateScoreText();
    }

    // Method to add points to the score
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    // Update the score display
    private void UpdateScoreText()
    {
        scoreText.text = SCORE_LABEL + score.ToString();
    }
}
