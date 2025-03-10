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


#if !UNITY_WEBGL
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Deleted progress file: " + saveFilePath);
            }
#endif


            playerProgress = new PlayerProgress("", "");
            SaveProgress();
            PlayerPrefs.SetInt("reset", 0);
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
                playerProgress.playerName = "שחקן";  // Set a default name if empty
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
                    // Ensure Asteroid Game uses `GameProgress(3)`
                    playerProgress.gamesProgressList.Add(new SerializableGameProgress
                    {
                        gameIndex = i,
                        progress = new GameProgress(i) // Pass game index 
                    });
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
    public void SaveStageProgress(int gameIndex, int stageIndex, float timeSpent, int mistakes = 0, int incorrectAsteroids = 0, int bonusAsteroids = 0)
    {
        if (playerProgress == null)
        {
            Debug.LogError("SaveStageProgress: playerProgress is null!");
            return;
        }

        if (!playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError($"SaveStageProgress: Invalid game index {gameIndex}");
            return;
        }

        var gameProgress = playerProgress.gamesProgress[gameIndex];

        if (!gameProgress.stages.ContainsKey(stageIndex))
        {
            Debug.LogError($"SaveStageProgress: Invalid stage index {stageIndex} for game {gameIndex}");
            return;
        }

        var stage = gameProgress.stages[stageIndex];

        // Handle Asteroid Game
        if (stage is GameProgress.AsteroidStageProgress asteroidStage)
        {
            asteroidStage.timeTaken = timeSpent;
            asteroidStage.incorrectAsteroids = incorrectAsteroids;
            asteroidStage.bonusAsteroids = bonusAsteroids;

            Debug.Log($"Saved Asteroid Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Incorrect {incorrectAsteroids}, Bonus {bonusAsteroids}");
        }
        // Handle Equipment Recovery Game
        else if (stage is GameProgress.EquipmentRecoveryStageProgress equipStage)
        {
            equipStage.timeTaken = timeSpent;
            equipStage.mistakes = mistakes; // Store mistakes separately

            Debug.Log($"Saved Equipment Recovery Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Mistakes {mistakes}");
        }
        else if (stage is GameProgress.CableConnectionStageProgress cableStage)
        {
            cableStage.timeTaken = timeSpent;
            cableStage.mistakes = mistakes; // Save mistakes

            Debug.Log($"Saved Cable Connection Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Mistakes {mistakes}");
        }
        // Handle Tubes Game (no mistakes, only time)
        else if (stage is GameProgress.StageProgress tubesStage)
        {
            tubesStage.timeTaken = timeSpent;

            Debug.Log($"Saved Tubes Game Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s");
        }


        else
        {
            Debug.LogError("Stage is not recognized! Data was not saved correctly.");
        }

        SaveProgress();
    }



    public int GetLastPlayedStage()
    {
        return playerProgress.lastPlayedStage;
    }


}
