using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance

    [SerializeField]
    private int defaultTargetScore = 100; // Default target score if PlayerDataManager is unavailable

    private int overallScore; // Encapsulated overall score
    private int targetScore;  // Target score loaded from PlayerDataManager

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            InitializeTargetScore(); // Load or set the target score
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
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

        // Check if the target score is reached
        if (OverallScore >= TargetScore)
        {
            Debug.Log("Target score reached! Triggering game over or next phase.");
            HandleTargetScoreReached();
        }
    }

    // Method to reset the overall score
    public void ResetScore()
    {
        OverallScore = 0;
        Debug.Log("OverallScore has been reset.");
    }

    // Initialize the target score, either from PlayerDataManager or default value
    private void InitializeTargetScore()
    {
        if (PlayerDataManager.Instance != null)
        {
            targetScore = PlayerDataManager.Instance.LoadTargetScore();
            Debug.Log($"TargetScore initialized to {targetScore} from PlayerDataManager.");
        }
        else
        {
            Debug.LogWarning("PlayerDataManager not found! Using default TargetScore.");
            targetScore = defaultTargetScore; // Use the default target score
        }
    }

    // Handle logic for when the target score is reached
    private void HandleTargetScoreReached()
    {
        Debug.Log("Game Over! You have reached the target score.");
    }
}
