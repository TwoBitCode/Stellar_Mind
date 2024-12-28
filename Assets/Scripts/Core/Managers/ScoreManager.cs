using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 0; // Keep the field private

    public int Score => score; // Add a read-only property for access

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}
