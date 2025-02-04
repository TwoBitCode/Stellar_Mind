using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EndScreenManager : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restart button clicked! Resetting all data...");

        // מחיקת כל הנתונים שנשמרו ב-PlayerPrefs (רלוונטי ל-WebGL ולנתונים כלליים)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // לוודא שהשינויים נשמרים

#if !UNITY_WEBGL
        // מחיקת קובץ ההתקדמות אם לא ב-WebGL
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

        // איפוס ה-GameProgressManager כדי לוודא שהמשחק מתחיל מאפס
        if (GameProgressManager.Instance != null)
        {
            Debug.Log("Destroying GameProgressManager instance...");
            Destroy(GameProgressManager.Instance.gameObject); // מוחק את האובייקט מהזיכרון
        }

        // טעינת סצנת ההתחלה מחדש
        Debug.Log("Loading first scene: WelcomeScene-vivi");
        SceneManager.LoadScene("WelcomeScene-vivi");
    }
}
