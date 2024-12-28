using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private int goalScore = 100;
    // Public property to expose goalScore
    public int GoalScore => goalScore;
    [SerializeField] private SceneSwitcher sceneSwitcher;

    private void Update()
    {
        if (OverallScoreManager.Instance != null && OverallScoreManager.Instance.OverallScore >= goalScore)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        if (sceneSwitcher != null)
        {
            OverallScoreManager.Instance.ResetScore();
            sceneSwitcher.LoadSceneByName(SceneNames.GameOver);
        }
        else
        {
            Debug.LogError("SceneSwitcher not assigned!");
        }
    }
}
