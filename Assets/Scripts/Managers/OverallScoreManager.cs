using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance
    private int overallScore; // Encapsulated overall score
    private int targetScore;  // Target score loaded from PlayerDataManager

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize score
            if (!PlayerPrefs.HasKey("OverallScore"))
            {
                PlayerPrefs.SetInt("OverallScore", 0); // Default score
            }
            overallScore = PlayerPrefs.GetInt("OverallScore");
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
    public int TargetScore => targetScore;

    // Method to add score
    public void AddScore(int score)
    {
        OverallScore += score;
        Debug.Log($"Added {score} to OverallScore. New OverallScore: {OverallScore}");
    }

    // Method to reset the overall score
    public void ResetScore()
    {
        OverallScore = 0;
        Debug.Log("OverallScore has been reset.");
    }

    // Initialize target score from PlayerDataManager
    private void InitializeTargetScore()
    {
        if (PlayerDataManager.Instance != null)
        {
            targetScore = PlayerDataManager.Instance.LoadTargetScore();
            Debug.Log($"TargetScore initialized to {targetScore} from PlayerDataManager.");
        }
        else
        {
            Debug.Log("PlayerDataManager not found! Using default TargetScore.");
            targetScore = 100; // Default target score
        }
    }
    public void AddScoreFromStage(string stageName, int score)
    {
        overallScore += score; // Update the overall score
        Debug.Log($"Stage '{stageName}' added {score} points. New OverallScore: {overallScore}");
    }

}