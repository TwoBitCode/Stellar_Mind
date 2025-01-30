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

            StartCoroutine(LoadProgressCoroutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator LoadProgressCoroutine()
    {
        yield return null; // מחכים פריים אחד כדי שכל האובייקטים יטענו

        LoadProgress();

        yield return new WaitForSeconds(0.1f); // מחכים 0.1 שניות
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

            // ודא שהמילון gamesProgress מאותחל
            if (playerProgress.gamesProgress == null)
            {
                Debug.LogWarning("gamesProgress was null after loading! Initializing.");
                playerProgress.gamesProgress = new Dictionary<int, GameProgress>();
            }

            // ודא שהרשימה gamesProgressList לא ריקה
            if (playerProgress.gamesProgressList == null || playerProgress.gamesProgressList.Count == 0)
            {
                Debug.LogWarning("gamesProgressList is empty! Initializing.");
                playerProgress.gamesProgressList = new List<SerializableGameProgress>();

                for (int i = 0; i < 4; i++)
                {
                    playerProgress.gamesProgressList.Add(new SerializableGameProgress { gameIndex = i, progress = new GameProgress() });
                }
            }

            // המרת הרשימה חזרה למילון כדי שהמשחק יוכל לעבוד עם הנתונים
            playerProgress.ConvertListToDictionary();

            Debug.Log("Game progress loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No save file found. Creating new progress.");
            playerProgress = new PlayerProgress("", "");
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
