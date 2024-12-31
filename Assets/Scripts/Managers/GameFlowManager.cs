using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("Game Configuration")]
    [SerializeField] private int goalScore = 100; // Target score for Game Over

    [Header("Scene Configuration")]
    [SerializeField] private string mapSceneName = "MapScene";
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    [Header("Dependencies")]
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    // Public property to access the goalScore
    public int GoalScore => goalScore;

    public void HandleSceneTransition()
    {
        if (OverallScoreManager.Instance != null)
        {
            string targetScene = OverallScoreManager.Instance.OverallScore >= goalScore
                ? gameOverSceneName
                : mapSceneName;

            sceneTransitionManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }
    }
}
