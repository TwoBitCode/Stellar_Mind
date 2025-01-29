using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string nextSceneName = "GameSelectionScene"; // Name of the next scene to load
    [SerializeField] private AudioSource audioSource; // Audio source for playing sound effects
    [SerializeField] private AudioClip clickSound;    // Click sound to play during transitions

    // Loads the next scene specified in the inspector
    public void LoadNextScene()
    {
        string playerName = GameProgressManager.Instance.GetPlayerProgress().playerName;
        if (string.IsNullOrEmpty(playerName) || playerName == "DefaultPlayer")
        {
            Debug.LogWarning("Player name not set! Blocking transition.");
            return;
        }

        // המשך טעינת הסצנה
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(WaitAndLoadScene(nextSceneName, clickSound.length));
        }
        else
        {
            LoadSceneImmediately(nextSceneName);
        }
    }


    // Loads a specified scene by name
    public void LoadScene(string sceneName)
    {
        if (audioSource != null && clickSound != null)
        {
            // Play the click sound and wait for it to finish before transitioning
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(WaitAndLoadScene(sceneName, clickSound.length));
        }
        else
        {
            // Immediately load the specified scene if no sound is assigned
            LoadSceneImmediately(sceneName);
        }
    }

    // Coroutine to wait for the click sound to finish before loading the scene
    private IEnumerator WaitAndLoadScene(string sceneName, float delay)
    {
        // Optional: Add fade-out animation or other transitions here
        yield return new WaitForSeconds(delay);

        ResetSceneState(); // Clean up current scene states before loading the new one
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // Loads the scene immediately without any delay
    private void LoadSceneImmediately(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            ResetSceneState(); // Clean up current scene states before loading the new one
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single); // Ensure only the new scene is loaded
        }
        else
        {
            Debug.LogError("Scene name is not provided for transition.");
        }
    }

    // Resets any lingering states or objects in the current scene
    private void ResetSceneState()
    {
        Debug.Log("Resetting scene state before transitioning.");

        // Ensure EventSystem is active (avoids UI issues)
        var eventSystem = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(true);
        }
    }
}
