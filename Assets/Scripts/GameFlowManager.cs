using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] public int goalScore = 100;
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [SerializeField] private string GAME_OVER_SCENE_NAME = "OverallGameOver"; // Name of the Game Over scene


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OverallScoreManager.Instance != null && OverallScoreManager.Instance.OverallScore >= goalScore)
        {
            if (sceneSwitcher != null)
            {
                OverallScoreManager.Instance.ResetScore();
                sceneSwitcher.LoadSceneByName(GAME_OVER_SCENE_NAME);  // Load the Game Over scene
            }
            else
            {
                Debug.LogError("SceneSwitcher not found on this GameObject!");
            }
        }
    }
}
