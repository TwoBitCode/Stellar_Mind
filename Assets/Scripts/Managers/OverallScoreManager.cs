using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance

    [Header("Score Settings")]
    [SerializeField]
    private int defaultTargetScore = 100; // Default target score if PlayerDataManager is unavailable

    private int overallScore; // Encapsulated overall score
    private int targetScore; // Target score loaded from PlayerDataManager

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            InitializeScores(); // Initialize overall and target scores
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    // Property to access the overall score
    public int OverallScore
    {
        get => overallScore;
        private set
        {
            overallScore = value;
            PlayerPrefs.SetInt("OverallScore", overallScore); // Save score persistently
        }
    }

    // Property to access the target score
    public int TargetScore => targetScore;

    // Adds points to the overall score
    public void AddScore(int score)
    {
        OverallScore += score;
        Debug.Log($"Added {score} points to OverallScore. New OverallScore: {OverallScore}");

        // Check if the target score is reached
        if (OverallScore >= TargetScore)
        {
            Debug.Log("Target score reached! Triggering game over or next phase.");
            HandleTargetScoreReached();
        }
    }

    // Adds score from a specific stage
    public void AddScoreFromStage(string stageName, int score)
    {
        AddScore(score); // Use the existing AddScore method
        Debug.Log($"Stage '{stageName}' added {score} points. New OverallScore: {OverallScore}");
    }

    // Resets the overall score
    public void ResetScore()
    {
        OverallScore = 0; // Reset score
        Debug.Log("OverallScore has been reset.");
    }

    // Initializes the overall and target scores
    private void InitializeScores()
    {
        // Initialize overall score from PlayerPrefs
        if (!PlayerPrefs.HasKey("OverallScore"))
        {
            PlayerPrefs.SetInt("OverallScore", 0); // Default to 0
        }
        overallScore = PlayerPrefs.GetInt("OverallScore");

        // Initialize target score from PlayerDataManager or use default
        if (PlayerDataManager.Instance != null)
        {
            targetScore = PlayerDataManager.Instance.LoadTargetScore();
            Debug.Log($"TargetScore initialized to {targetScore} from PlayerDataManager.");
        }
        else
        {
            Debug.LogWarning("PlayerDataManager not found! Using default TargetScore.");
            targetScore = defaultTargetScore;
        }
    }

    // Handles logic when the target score is reached
    private void HandleTargetScoreReached()
    {
        Debug.Log("Game Over! You have reached the target score.");
    }
}