using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance
    private int overallScore; // Encapsulated overall score
    //private int targetScore;  // Target score loaded from PlayerDataManager

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // טוען את הניקוד השמור מתוך `GameProgressManager`
            overallScore = GameProgressManager.Instance.GetPlayerProgress().totalScore;

            Debug.Log($"Loaded overallScore: {overallScore}");
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
        GameProgressManager.Instance.GetPlayerProgress().totalScore = overallScore;
        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Added {score} to OverallScore. New OverallScore: {overallScore}. Progress saved.");
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