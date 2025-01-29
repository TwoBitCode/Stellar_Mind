using UnityEngine;
using System.IO;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }
    private PlayerProgress playerProgress;
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            saveFilePath = Application.persistentDataPath + "/playerProgress.json";
            LoadProgress();
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            playerProgress = JsonUtility.FromJson<PlayerProgress>(json);

            if (playerProgress == null)
            {
                Debug.LogError("Failed to load player progress. Creating new progress.");
                playerProgress = new PlayerProgress("", "");
            }

            // Ensure game progress is initialized after loading
            playerProgress.EnsureGameProgressInitialized();

            Debug.Log($"Loaded player data: {json}");
        }
        else
        {
            Debug.Log("No save file found, creating new progress.");
            playerProgress = new PlayerProgress("", "");
            SaveProgress(); // Save a fresh progress file
        }
    }






    public PlayerProgress GetPlayerProgress()
    {
        return playerProgress;
    }
    public void InitializePlayer(string playerName, string character)
    {
        if (playerProgress == null)
        {
            playerProgress = new PlayerProgress(playerName, character);
        }
        else
        {
            playerProgress.playerName = playerName;
            playerProgress.selectedCharacter = character;
        }

        Debug.Log($"Saving player name: {playerProgress.playerName}, Character: {playerProgress.selectedCharacter}");
        SaveProgress();
    }

    public void SaveProgress()
    {
        if (playerProgress == null)
        {
            Debug.LogError("Trying to save progress, but playerProgress is null!");
            return;
        }

        // Ensure `gamesProgress` is initialized before saving
        playerProgress.EnsureGameProgressInitialized();

        string json = JsonUtility.ToJson(playerProgress);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game progress saved: {json}");
    }


    public void ResetProgress()
    {
        playerProgress = new PlayerProgress("", ""); 
        SaveProgress();
        Debug.Log("Game progress reset! Restarting selection.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProgress();
            Debug.Log("Progress reset! Press Play again to restart from the beginning.");
        }
    }
    public void CompleteStage(int gameIndex, int stageIndex, int score)
    {
        if (!playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError($"Game index {gameIndex} not found in progress tracking!");
            return;
        }

        // Mark the stage as completed and update the score
        playerProgress.gamesProgress[gameIndex].stages[stageIndex].isCompleted = true;
        playerProgress.gamesProgress[gameIndex].stages[stageIndex].score = score;

        Debug.Log($"Stage {stageIndex} in Game {gameIndex} completed with {score} points.");

        // Check if all stages are completed
        if (playerProgress.gamesProgress[gameIndex].CheckIfCompleted())
        {
            Debug.Log($"Game {gameIndex} is now fully completed!");
        }

        SaveProgress(); // Save the updated progress
    }





}