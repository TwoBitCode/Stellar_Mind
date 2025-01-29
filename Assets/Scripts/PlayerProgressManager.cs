using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerProgress
{
    public string playerName;
    public string selectedCharacter; 
    public int totalScore;
    public Dictionary<int, GameProgress> gamesProgress; 

    public int lastPlayedGame; // NEW: Tracks the last game the player played
    public int lastPlayedStage; // NEW: Tracks the last stage the player played

    public PlayerProgress(string name, string character)
    {
        playerName = name;
        selectedCharacter = character;
        totalScore = 0;
        lastPlayedGame = -1; // Default: No game played yet
        lastPlayedStage = -1; // Default: No stage played yet
        gamesProgress = new Dictionary<int, GameProgress>();

        for (int i = 0; i < 4; i++) // 4 משחקים
        {
            gamesProgress[i] = new GameProgress();
        }
    }
    public void EnsureGameProgressInitialized()
    {
        if (gamesProgress == null)
        {
            Debug.LogWarning("gamesProgress dictionary was null! Reinitializing.");
            gamesProgress = new Dictionary<int, GameProgress>();
        }

        for (int i = 0; i < 4; i++) // Ensure all 4 games are initialized
        {
            if (!gamesProgress.ContainsKey(i))
            {
                Debug.LogWarning($"Game index {i} was missing. Initializing now.");
                gamesProgress[i] = new GameProgress();
            }
        }
    }

}


[System.Serializable]
public class GameProgress
{
    public bool isCompleted; // True if all stages are done
    public Dictionary<int, StageProgress> stages; // Tracks individual stages

    public GameProgress()
    {
        isCompleted = false;
        stages = new Dictionary<int, StageProgress>();

        for (int i = 0; i < 3; i++) // Assuming 3 stages per mini-game
        {
            stages[i] = new StageProgress();
        }
    }

    public bool CheckIfCompleted()
    {
        foreach (var stage in stages.Values)
        {
            if (!stage.isCompleted) return false; // If any stage is incomplete, game is not complete
        }
        isCompleted = true;
        return true;
    }
}


[System.Serializable]
public class StageProgress
{
    public bool isCompleted; // האם השלב הושלם
    public int score; // ניקוד של השלב

    public StageProgress()
    {
        isCompleted = false;
        score = 0;
    }
}
