using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;
using Newtonsoft.Json;

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



    public void AdvanceToNextCycle()
    {
        if (playerProgress == null)
        {
            Debug.LogError("AdvanceToNextCycle: Player progress is null!");
            return;
        }

        playerProgress.ConvertDictionaryToList(); // make sure list is up-to-date
        List<SerializableGameProgress> snapshotCopy = new List<SerializableGameProgress>();

        foreach (var game in playerProgress.gamesProgressList)
        {
            try
            {
                string json = JsonConvert.SerializeObject(
                    game.progress,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Converters = new List<JsonConverter> { new StageProgressConverter() }
                    });

                var progressCopy = JsonConvert.DeserializeObject<GameProgress>(
                    json,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Converters = new List<JsonConverter> { new StageProgressConverter() }
                    });

                var copiedGame = new SerializableGameProgress
                {
                    gameIndex = game.gameIndex,
                    progress = progressCopy
                };

                snapshotCopy.Add(copiedGame);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to serialize/deserialize game {game.gameIndex}: {ex.Message}");
            }
        }


        if (playerProgress.hasStartedCurrentCycle)
        {
            playerProgress.cycleHistory.Add(new CycleSummary
            {
                cycleNumber = playerProgress.currentCycle,
                totalScore = playerProgress.totalScore,
                startDate = playerProgress.currentCycleStartDate,
                endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                gamesSnapshot = snapshotCopy
            });
        }
        else
        {
            Debug.Log("Skipped saving cycle history — player did not start any games.");
        }


        playerProgress.currentCycle++;
        playerProgress.currentCycleStartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        playerProgress.totalScore = 100;
        playerProgress.lastPlayedGame = -1;
        playerProgress.lastPlayedStage = -1;
        playerProgress.hasStartedCurrentCycle = false; // איפוס הדגל

        playerProgress.gamesProgress.Clear();
        for (int i = 0; i < 4; i++)
        {
            playerProgress.gamesProgress[i] = new GameProgress(i);
        }

        playerProgress.ConvertDictionaryToList();
        SaveProgress();

        Debug.Log($"New cycle {playerProgress.currentCycle} started.");
    }




    public async void SaveProgress()
    {
        if (playerProgress == null)
        {
            Debug.LogError("Cannot save progress because playerProgress is null!");
            return;
        }

        playerProgress.ConvertDictionaryToList();

        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };

        string json = JsonConvert.SerializeObject(playerProgress, settings);

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
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    };

                    var loadedProgress = JsonConvert.DeserializeObject<PlayerProgress>(json, settings);

                    if (loadedProgress == null)
                    {
                        Debug.LogWarning("Deserialization returned null. Cloud data might be corrupted.");
                    }
                    else
                    {
                        playerProgress = loadedProgress;

                        try
                        {
                            playerProgress.ConvertListToDictionary();
                            Debug.Log("Progress loaded and converted from Cloud Save.");
                            return;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("Error during ConvertListToDictionary(): " + ex.Message);
                        }
                    }
                }

                Debug.Log("No valid progress found. Creating new player progress.");
                playerProgress = new PlayerProgress(GetDefaultPlayerName(), "");
                //SaveProgress();
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
    public void SaveStageProgress(int gameIndex, int stageIndex, float timeSpent, int mistakes = 0, int incorrectAsteroids = 0, int bonusAsteroids = 0, int score = 0)
    {
        if (playerProgress == null)
        {


            Debug.LogError("SaveStageProgress: playerProgress is null!");
            return;
        }
        playerProgress.hasStartedCurrentCycle = true;
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
            asteroidStage.selectedTime = GameObject.FindAnyObjectByType<AsteroidGameUIManager>()?.SelectedDuration ?? 0f;
            stage.score = score;

            Debug.Log($"Saved Asteroid Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Incorrect {incorrectAsteroids}, Bonus {bonusAsteroids}");
        }
        // Handle Equipment Recovery Game
        else if (stage is GameProgress.EquipmentRecoveryStageProgress equipStage)
        {
            equipStage.timeTaken = timeSpent;
            equipStage.mistakes = mistakes;
            equipStage.selectedTime = EquipmentRecoveryGameManager.Instance?.SelectedTimeForCurrentStage ?? 0f;
            equipStage.score = score;


            Debug.Log($"Saved Equipment Recovery Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Mistakes {mistakes}, Selected Time {equipStage.selectedTime}");
        }

        else if (stage is GameProgress.CableConnectionStageProgress cableStage)
        {
            cableStage.timeTaken = timeSpent;
            cableStage.mistakes = mistakes;
            cableStage.selectedTime = GameObject.FindAnyObjectByType<CableConnectionManager>()?.SelectedMemoryTime ?? 0f;

            Debug.Log($"Saved Cable Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Mistakes {mistakes}, Selected Time {cableStage.selectedTime}s");
        }

        // Handle Tubes Game (no mistakes, only time)
        else if (stage is GameProgress.StageProgress tubesStage)
        {
            tubesStage.timeTaken = timeSpent;
            tubesStage.selectedTime = GameObject.FindAnyObjectByType<GameManager>()?.SelectedMemoryTime ?? 0f;

            Debug.Log($"Saved Tubes Game Stage {stageIndex} for Game {gameIndex}: Time {timeSpent:F2}s, Selected Time {tubesStage.selectedTime}s");
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
