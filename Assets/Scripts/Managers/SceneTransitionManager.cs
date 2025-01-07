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
        // Check if audioSource and clickSound are assigned
        if (audioSource != null && clickSound != null)
        {
            // Play the click sound and wait for it to finish before transitioning
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(WaitAndLoadScene(nextSceneName, clickSound.length));
        }
        else
        {
            // Immediately load the next scene if no sound is assigned
            LoadSceneImmediately(nextSceneName);
        }
    }

    // Loads a specified scene by name
    public void LoadScene(string sceneName)
    {
        // Check if audioSource and clickSound are assigned
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
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Load the scene
        LoadSceneImmediately(sceneName);
    }

    // Loads the scene immediately without any delay
    private void LoadSceneImmediately(string sceneName)
    {
        // Check if the scene name is valid
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Load the scene
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // Log an error if the scene name is not provided
            Debug.LogError("Scene name is not provided for transition.");
        }
    }
}
