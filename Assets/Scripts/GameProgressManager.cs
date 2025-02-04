using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;

public class GameProgressManager : MonoBehaviour
{
    private string saveFilePath;
    public PlayerProgress playerProgress;
    public static GameProgressManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress.json");

            // בדיקה אם יש צורך לאפס את הנתונים
            CheckForReset();

            StartCoroutine(LoadProgressCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void CheckForReset()
    {
        if (PlayerPrefs.HasKey("reset") && PlayerPrefs.GetInt("reset") == 1)
        {
            Debug.Log("Reset detected! Clearing all saved progress...");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

#if UNITY_WEBGL
            Debug.Log("Clearing localStorage for WebGL...");
            Application.ExternalEval("localStorage.clear(); location.reload();");
#endif

            // מחיקת קובץ השמירה אם קיים (רק בפלטפורמות תומכות)
#if !UNITY_WEBGL
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Deleted progress file: " + saveFilePath);
            }
#endif

            // יצירת פרוגרס חדש
            playerProgress = new PlayerProgress("", "");
            SaveProgress();
            PlayerPrefs.SetInt("reset", 0); // מניעת איפוס מחזורי
        }
    }

    private IEnumerator LoadProgressCoroutine()
    {
        yield return null;

        LoadProgress();

        yield return new WaitForSeconds(0.1f); 
        Debug.Log("Finished loading progress, now ready to use data.");
    }



    public void SaveProgress()
    {
        if (playerProgress == null)
        {
            Debug.LogError("Cannot save progress because playerProgress is null!");
            return;
        }

        playerProgress.ConvertDictionaryToList();

        string json = JsonUtility.ToJson(playerProgress, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game progress successfully saved: " + json);
    }
    public void LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerProgress = JsonUtility.FromJson<PlayerProgress>(json);

            if (playerProgress == null)
            {
                Debug.LogError("Failed to load progress! Creating new save.");
                playerProgress = new PlayerProgress("", "");
                SaveProgress();
                return;
            }

            // Ensure playerName is initialized if missing
            if (string.IsNullOrEmpty(playerProgress.playerName))
            {
                playerProgress.playerName = "Player";  // Set a default name if empty
                Debug.LogWarning("playerName was empty! Resetting to default.");
            }

            // Ensure gamesProgress is initialized
            if (playerProgress.gamesProgress == null)
            {
                Debug.LogWarning("gamesProgress was null after loading! Initializing.");
                playerProgress.gamesProgress = new Dictionary<int, GameProgress>();
            }

            // Ensure gamesProgressList is initialized
            if (playerProgress.gamesProgressList == null || playerProgress.gamesProgressList.Count == 0)
            {
                Debug.LogWarning("gamesProgressList is empty! Initializing.");
                playerProgress.gamesProgressList = new List<SerializableGameProgress>();

                for (int i = 0; i < 4; i++)
                {
                    playerProgress.gamesProgressList.Add(new SerializableGameProgress { gameIndex = i, progress = new GameProgress() });
                }
            }

            // Convert the list back into a dictionary for use in-game
            playerProgress.ConvertListToDictionary();

            Debug.Log($"Game progress loaded successfully. Player Name: {playerProgress.playerName}");
        }
        else
        {
            Debug.LogWarning("No save file found. Creating new progress.");
            playerProgress = new PlayerProgress("", "");  // Initialize with an empty name
            SaveProgress();
        }
    }


    public void SetLastPlayedGame(int gameIndex, int stageIndex)
    {
        playerProgress.lastPlayedGame = gameIndex;
        playerProgress.lastPlayedStage = stageIndex;
        SaveProgress();
        Debug.Log($"Last played game set to {gameIndex}, stage {stageIndex}");
    }

    public int GetLastPlayedGame()
    {
        return playerProgress.lastPlayedGame;
    }

    public int GetLastPlayedStage()
    {
        return playerProgress.lastPlayedStage;
    }


}
