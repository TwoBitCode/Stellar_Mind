using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class OverallScoreManager : MonoBehaviour
{
    [SerializeField] public static OverallScoreManager Instance; // Singleton instance
    [SerializeField] public int OverallScore  = 0; // Encapsulated overall score

    private void Awake()
    {
        // Ensure there is only one instance of the ScoreManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    // Method to add score
    public void AddScore(int score)
    {
        OverallScore += score;
        Debug.Log($"Added {score} to OverallScore. New OverallScore: {OverallScore}");
    }

    // Optional: Reset score (e.g., when restarting the game)
    public void ResetScore()
    {
        OverallScore = 0;
    }
}
