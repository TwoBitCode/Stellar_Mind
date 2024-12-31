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
        yield return new WaitForSeconds(delay);
        LoadSceneImmediately(sceneName);
    }

    /// <summary>
    /// Load the scene immediately without delay.
    /// </summary>
    private void LoadSceneImmediately(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not provided for transition.");
        }
    }
}
