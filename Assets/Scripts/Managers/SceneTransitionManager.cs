using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string nextSceneName = "GameSelectionScene";
    [SerializeField] private AudioSource audioSource; // Audio Source for click sound
    [SerializeField] private AudioClip clickSound;    // Click sound

    /// <summary>
    /// Load the next scene specified in the inspector.
    /// </summary>
    public void LoadNextScene()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(WaitAndLoadScene(nextSceneName, clickSound.length)); // Wait for sound to finish
        }
        else
        {
            LoadSceneImmediately(nextSceneName);
        }
    }

    /// <summary>
    /// Load a specified scene by name.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(WaitAndLoadScene(sceneName, clickSound.length)); // Wait for sound to finish
        }
        else
        {
            LoadSceneImmediately(sceneName);
        }
    }

    /// <summary>
    /// Coroutine to wait for the click sound to finish before loading the scene.
    /// </summary>
    private IEnumerator WaitAndLoadScene(string sceneName, float delay)
    {
        // Optional: Add fade-out animation or other transitions here
        yield return new WaitForSeconds(delay);

        ResetSceneState(); // Clean up current scene states before loading the new one
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Load the scene immediately without delay.
    /// </summary>
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

    /// <summary>
    /// Resets any lingering states or objects in the current scene.
    /// </summary>
    private void ResetSceneState()
    {
        // Reset UI or other scene-specific states here
        Debug.Log("Resetting scene state before transitioning.");

        // Ensure EventSystem is active (avoids UI issues)
        var eventSystem = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(true);
        }

        // Optional: Clear or reset other persistent objects if needed
    }
}
