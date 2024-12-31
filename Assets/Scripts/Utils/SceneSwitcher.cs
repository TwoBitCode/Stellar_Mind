using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private const string SceneLoadingError = "Error: Invalid scene name or index.";

    /// <summary>
    /// Loads a scene by its name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty. Cannot load scene.");
            return;
        }

        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"{SceneLoadingError} Scene Name: {sceneName}");
        }
    }

    /// <summary>
    /// Loads a scene by its build index.
    /// </summary>
    /// <param name="sceneIndex">The index of the scene to load.</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"{SceneLoadingError} Scene Index: {sceneIndex}");
        }
    }

    /// <summary>
    /// Checks if a scene exists in the build settings.
    /// </summary>
    /// <param name="sceneName">The name of the scene to check.</param>
    /// <returns>True if the scene exists, otherwise false.</returns>
    private bool SceneExists(string sceneName)
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        return sceneIndex >= 0;
    }

    /// <summary>
    /// Reloads the current active scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
