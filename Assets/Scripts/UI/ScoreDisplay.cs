using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [Tooltip("Reference to UI Text element")]
    [SerializeField] private TextMeshProUGUI overallScoreText;
    [SerializeField] private GameFlowManager gameFlowManager;

    private void Update()
    {
        if (OverallScoreManager.Instance != null && gameFlowManager != null)
        {
            overallScoreText.text = "Overall Score: " + OverallScoreManager.Instance.OverallScore.ToString() + " / " + gameFlowManager.GoalScore;
        }
    }
}
