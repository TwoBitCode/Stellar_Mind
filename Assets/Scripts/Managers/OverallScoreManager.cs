using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; } // Singleton instance
    private int overallScore; // Encapsulated overall score
    private int targetScore;  // Target score loaded from PlayerDataManager

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            InitializeTargetScore(); // Load the target score from PlayerDataManager
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

}
