using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public static ScoreDisplay Instance { get; private set; }

    [Tooltip("Reference to UI Text element")]
    [SerializeField] private TextMeshProUGUI overallScoreText;

    private Color defaultColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        defaultColor = overallScoreText.color; // Store default color
    }

    private void Update()
    {
        if (OverallScoreManager.Instance != null)
        {
            overallScoreText.text = OverallScoreManager.Instance.OverallScore.ToString();
        }
        else
        {
            overallScoreText.text = "Overall Score: 0";
            Debug.LogWarning("OverallScoreManager instance not found!");
        }
    }

    // Function to temporarily change color if score is reduced
    public void UpdateScoreDisplay(int scoreChange)
    {
        if (scoreChange < 0)
        {
            overallScoreText.color = Color.red;
            Invoke(nameof(ResetColor), 0.5f); // Reset after 0.5 seconds
        }
    }

    private void ResetColor()
    {
        overallScoreText.color = defaultColor;
    }
}
