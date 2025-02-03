using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EndScreenManager : MonoBehaviour
{
    public void RestartGame()
    {
        // Delete Player Progress File
        string saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Player progress reset!");
        }

        // Reload the first scene (Main Menu or Name Entry)
        SceneManager.LoadScene("WelcomeScene-vivi"); // Change this to your first scene
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Works in the Editor
#elif UNITY_WEBGL
            Application.OpenURL("about:blank"); // Close tab in WebGL
#else
            Application.Quit(); // Works in standalone builds
#endif
    }
}
