using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance
    private int overallScore; // Encapsulated overall score


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load the overall score from PlayerProgress
            if (GameProgressManager.Instance != null && GameProgressManager.Instance.playerProgress != null)
            {
                overallScore = GameProgressManager.Instance.playerProgress.totalScore;
                Debug.Log($"Loaded Overall Score from GameProgress: {overallScore}");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Property to access the overall score
    public int OverallScore
    {
        get => overallScore;
        private set => overallScore = value;
    }

    // Property to access the target score

    // Method to add score
    public void AddScore(int score)
    {
        overallScore += score;

        if (GameProgressManager.Instance != null && GameProgressManager.Instance.playerProgress != null)
        {
            GameProgressManager.Instance.playerProgress.totalScore = overallScore;
            GameProgressManager.Instance.SaveProgress();
        }

        Debug.Log($"Added {score} to Overall Score. New Overall Score: {overallScore}. Progress saved.");

        // Notify ScoreDisplay to temporarily change the text color if score is negative
        ScoreDisplay.Instance?.UpdateScoreDisplay(score);
    }




    // Method to reset the overall score
    public void ResetScore()
    {
        OverallScore = 0;
        Debug.Log("OverallScore has been reset.");
    }

    // Initialize target score from PlayerDataManager

    public void AddScoreFromStage(string stageName, int score)
    {
        overallScore += score; // Update the overall score
        Debug.Log($"Stage '{stageName}' added {score} points. New OverallScore: {overallScore}");
    }

}