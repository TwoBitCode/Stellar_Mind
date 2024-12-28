using UnityEngine;

public class OverallScoreManager : MonoBehaviour
{
    public static OverallScoreManager Instance { get; private set; }
    public int OverallScore { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        OverallScore += score;
        Debug.Log($"New Overall Score: {OverallScore}");
    }

    public void ResetScore()
    {
        OverallScore = 0;
    }
}
