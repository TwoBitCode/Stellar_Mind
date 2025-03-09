using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public TextMeshProUGUI gameTimesText;
    private void Start()
    {
        PrintAllGameTimes(); // Show game times when End Screen appears
    }
    private void PrintAllGameTimes()
    {
        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError("GameProgressManager instance or playerProgress is null!");
            return;
        }

        var gamesProgress = GameProgressManager.Instance.playerProgress.gamesProgress;
        if (gamesProgress == null || gamesProgress.Count == 0)
        {
            Debug.LogError("No game progress found!");
            return;
        }

        string timesText = "Game Completion Times:\n"; // Text to display on screen

        foreach (var gameEntry in gamesProgress)
        {
            int gameIndex = gameEntry.Key;
            GameProgress gameProgress = gameEntry.Value;

            if (gameProgress == null)
            {
                Debug.LogWarning($"Game {gameIndex} progress is null, skipping...");
                continue;
            }

            timesText += $"Game {gameIndex}:\n";

            foreach (var stageEntry in gameProgress.stages)
            {
                int stageIndex = stageEntry.Key;
                StageProgress stage = stageEntry.Value;

                if (stage == null)
                {
                    Debug.LogWarning($"Game {gameIndex}, Stage {stageIndex} progress is null, skipping...");
                    continue;
                }

                timesText += $"- Stage {stageIndex}: {stage.timeTaken:F2} sec\n";
            }
        }

        if (gameTimesText != null)
        {
            gameTimesText.text = timesText; // Update the UI text
        }
        else
        {
            Debug.LogError("GameTimesText UI element is not assigned in the Inspector!");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart button clicked! Resetting all data...");

        // סימון שהמשחק צריך להתאפס
        PlayerPrefs.SetInt("reset", 1);
        PlayerPrefs.Save();

#if UNITY_WEBGL
        // מחיקת localStorage בדפדפן דרך JavaScript
        ResetLocalStorage();
#else
        // מחיקת כל הנתונים שנשמרו ב-PlayerPrefs (בפלטפורמות שאינן WebGL)
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
