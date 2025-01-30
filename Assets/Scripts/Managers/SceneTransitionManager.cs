//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class SceneTransitionManager : MonoBehaviour
//{
//    [Header("Scene Configuration")]
//    [SerializeField] private string nextSceneName = "GameSelectionScene"; // Name of the next scene to load

//    // Loads the next scene specified in the inspector
//    public void LoadNextScene()
//    {
//        string playerName = GameProgressManager.Instance.GetPlayerProgress().playerName;
//        if (string.IsNullOrEmpty(playerName) || playerName == "DefaultPlayer")
//        {
//            Debug.LogWarning("Player name not set! Blocking transition.");
//            return;
//        }

//        LoadSceneImmediately(nextSceneName);
//    }

//    // Loads a specified scene by name
//    public void LoadScene(string sceneName)
//    {
//        LoadSceneImmediately(sceneName);
//    }

//    // Loads the scene immediately without any delay
//    private void LoadSceneImmediately(string sceneName)
//    {
//        if (!string.IsNullOrEmpty(sceneName))
//        {
//            ResetSceneState(); // Clean up current scene states before loading the new one
//            SceneManager.LoadScene(sceneName, LoadSceneMode.Single); // Ensure only the new scene is loaded
//        }
//        else
//        {
//            Debug.LogError("Scene name is not provided for transition.");
//        }
//    }

//    // Resets any lingering states or objects in the current scene
//    private void ResetSceneState()
//    {
//        Debug.Log("Resetting scene state before transitioning.");

//        // Ensure EventSystem is active (avoids UI issues)
//        var eventSystem = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
//        if (eventSystem != null)
//        {
//            eventSystem.gameObject.SetActive(false);
//            eventSystem.gameObject.SetActive(true);
//        }
//    }
//}
