using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
    public Button reportButton; // Reference to the report button

    private string lastScene;
    private void Start()
    {
        PlayerPrefs.SetString("LastSceneBeforeReport", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();


    }
    public void OpenGameReport()
    {
        Debug.Log("Opening Game Report");
        SceneManager.LoadScene("Player report");
    }
    private void DestroyAllPersistentObjects()
    {
        // **Find all objects marked as DontDestroyOnLoad**
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.buildIndex == -1) // Objects in DontDestroyOnLoad
            {
                Debug.Log($"Destroying persistent object: {obj.name}");
                Destroy(obj);
            }
        }
    }
    public void RestartGame()
    {
        Debug.Log("Restart button clicked! Resetting all data...");

        // סימון שהמשחק צריך להתאפס
        PlayerPrefs.SetInt("reset", 1);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

#if UNITY_WEBGL
        // מחיקת localStorage בדפדפן דרך JavaScript
        ResetLocalStorage();
#else

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // מחיקת קובץ ההתקדמות
        string saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Player progress file deleted.");
        }
        else
        {
            Debug.Log("No player progress file found.");
        }
#endif


        GameObject progressManager = GameObject.Find("GameProgressManager");
        if (progressManager != null)
        {
            Debug.Log("Found and destroying GameProgressManager manually.");
            Destroy(progressManager);
        }
        else if (GameProgressManager.Instance != null)
        {
            Debug.Log("Destroying GameProgressManager instance...");
            Destroy(GameProgressManager.Instance.gameObject);
        }
        // **Destroy ALL DontDestroyOnLoad Objects**
        DestroyAllPersistentObjects();

        // **Force Full Scene Reload**
        Debug.Log("Reloading first scene: WelcomeScene-vivi");
        SceneManager.LoadScene("WelcomeScene-vivi");

        // טעינת סצנת ההתחלה מחדש
        Debug.Log("Loading first scene: WelcomeScene-vivi");
        SceneManager.LoadScene("WelcomeScene-vivi");
    }

#if UNITY_WEBGL
    private void ResetLocalStorage()
    {
        Debug.Log("Resetting localStorage via JavaScript...");
        Application.ExternalEval("localStorage.clear(); location.reload();");
    }
#endif
}
