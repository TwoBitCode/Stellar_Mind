using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [Tooltip("Reference to UI Text element")]
    [SerializeField] private TextMeshProUGUI overallScoreText;

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
}
