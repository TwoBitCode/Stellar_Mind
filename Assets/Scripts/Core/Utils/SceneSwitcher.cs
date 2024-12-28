using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames
{
    public const string MainMenu = "MainMenu";
    public const string MemoryGame = "MemoryGame";
    public const string SortingAsteroids = "sortingAsteroids";
    public const string GameOver = "OverallGameOver";
}

public class SceneSwitcher : MonoBehaviour
{
    public void LoadSceneByName(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} does not exist.");
        }
    }

    private bool SceneExists(string sceneName)
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        return sceneIndex >= 0;
    }
}
