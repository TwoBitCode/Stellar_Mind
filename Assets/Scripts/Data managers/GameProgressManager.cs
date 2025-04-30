using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;

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

            Debug.Log("GameProgressManager initialized.");
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

    //private IEnumerator LoadProgressCoroutine()
    //{
    //    yield return null;

    //    LoadProgress();

    //    yield return new WaitForSeconds(0.1f);
    //    Debug.Log("Finished loading progress, now ready to use data.");
    //}



    public async void SaveProgress()
    {
        if (playerProgress == null)
        {
            Debug.LogError("Cannot save progress because playerProgress is null!");
            return;
        }

        playerProgress.ConvertDictionaryToList();
        string json = JsonUtility.ToJson(playerProgress);

        try
        {
            var data = new Dictionary<string, object> { { "playerProgress", json } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log("Progress saved to Cloud Save.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Cloud Save failed: " + ex.Message);
        }
    }

    public async Task LoadProgress()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "playerProgress" });

            if (data.TryGetValue("playerProgress", out var item) && item?.Value != null)
            {
                string json = item.Value.GetAs<string>();

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var loadedProgress = JsonUtility.FromJson<PlayerProgress>(json);

                    if (loadedProgress == null)
                    {
                        Debug.LogWarning("FromJson returned null. Cloud data might be corrupted.");
                    }
                    else
                    {
                        playerProgress = loadedProgress;

                        try
                        {
                            playerProgress.ConvertListToDictionary(); // safely guarded now
                            Debug.Log("Progress loaded and converted from Cloud Save.");
                            return;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(" Error during ConvertListToDictionary(): " + ex.Message);
                        }
                    }
                }

                // If we reach here, either no data was found or JSON was invalid
                Debug.Log("No valid progress found. Creating new player progress.");
                playerProgress = new PlayerProgress(GetDefaultPlayerName(), "");
                SaveProgress();
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Failed to load progress from cloud: " + ex.Message);
            // Do not overwrite playerProgress on cloud error
        }
    }


    private string GetDefaultPlayerName()
    {
        // Try to use the signed-in username if possible
        return AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerName : "Unknown";
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
