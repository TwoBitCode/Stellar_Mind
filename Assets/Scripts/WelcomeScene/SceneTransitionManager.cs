using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string nextSceneName = "GameSelectionScene";
    [SerializeField] private AudioSource audioSource; // Audio Source for click sound
    [SerializeField] private AudioClip clickSound;    // Click sound

    public void LoadNextScene()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            Invoke(nameof(TransitionToScene), clickSound.length); // Wait for sound to finish
        }
        else
        {
            TransitionToScene(); // Immediate transition if no sound
        }
    }

    private void TransitionToScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set in SceneTransitionManager.");
        }
    }
}
