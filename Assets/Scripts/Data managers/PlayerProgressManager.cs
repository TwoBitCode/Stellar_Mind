using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SerializableGameProgress
{
    public int gameIndex;
    public GameProgress progress;
}

[System.Serializable]
public class PlayerProgress
{
    public string playerName;
    public string selectedCharacter;
    public int totalScore;
    public int lastPlayedGame;
    public int lastPlayedStage;
    public List<SerializableGameProgress> gamesProgressList = new List<SerializableGameProgress>();
    public Dictionary<int, GameProgress> gamesProgress = new Dictionary<int, GameProgress>();

    public PlayerProgress(string name, string character)
    {
        playerName = name;
        selectedCharacter = character;
        totalScore = 100;
        lastPlayedGame = -1;
        lastPlayedStage = -1;
        gamesProgress = new Dictionary<int, GameProgress>();

        for (int i = 0; i < 4; i++) // Assuming 4 different games
        {
            if (i == 3) // Asteroid Game uses `AsteroidStageProgress`
                gamesProgress[i] = new GameProgress(3);
            else
                gamesProgress[i] = new GameProgress(i);
        }

    }

    public void ConvertListToDictionary()
    {
        if (gamesProgressList == null || gamesProgressList.Count == 0)
        {
            Debug.LogError("ConvertListToDictionary: gamesProgressList is EMPTY! Initializing.");
            gamesProgressList = new List<SerializableGameProgress>();

            for (int i = 0; i < 4; i++) // Assuming 4 different games
            {
                gamesProgressList.Add(new SerializableGameProgress { gameIndex = i, progress = new GameProgress(i) });
            }
        }

        gamesProgress.Clear();
        foreach (var item in gamesProgressList)
        {
            if (item == null)
            {
                Debug.LogError("ConvertListToDictionary: Found a NULL item in gamesProgressList!");
                continue;
            }

            if (item.progress == null)
            {
                Debug.LogError($"ConvertListToDictionary: Game {item.gameIndex} has NULL progress! Creating new GameProgress.");
                item.progress = new GameProgress(item.gameIndex);
            }

            item.progress.ConvertListToDictionary();
            gamesProgress[item.gameIndex] = item.progress;
        }

        Debug.Log($"ConvertListToDictionary: gamesProgress now contains {gamesProgress.Count} games.");
    }



    public void ConvertDictionaryToList()
    {
        if (gamesProgress == null || gamesProgress.Count == 0)
        {
            Debug.LogError("ConvertDictionaryToList: gamesProgress is EMPTY or NULL! Creating default data.");
            gamesProgress = new Dictionary<int, GameProgress>();

            for (int i = 0; i < 4; i++) // Assuming 4 different games
            {
                if (i == 3) // Asteroid Game uses `AsteroidStageProgress`
                    gamesProgress[i] = new GameProgress(3);
                else
                    gamesProgress[i] = new GameProgress(i);
            }
        }

        gamesProgressList.Clear();
        foreach (var pair in gamesProgress)
        {
            pair.Value.ConvertDictionaryToList();
            gamesProgressList.Add(new SerializableGameProgress { gameIndex = pair.Key, progress = pair.Value });
        }
    }



}

[System.Serializable]
public class GameProgress
{
    public bool isCompleted;
    public Dictionary<int, StageProgress> stages;
    public List<SerializableStageProgress> stagesList;
    public bool hasStarted; // NEW: Track if this mini-game has started

    public GameProgress(int gameIndex)
    {
        isCompleted = false;
        stages = new Dictionary<int, StageProgress>();
        stagesList = new List<SerializableStageProgress>();

        for (int i = 0; i < 3; i++) // Assuming 3 stages per game
        {
            if (gameIndex == 3) // Asteroid Game uses `AsteroidStageProgress`
                stages[i] = new AsteroidStageProgress();
            else if (gameIndex == 2) // Equipment Recovery Game
                stages[i] = new EquipmentRecoveryStageProgress();
            else if (gameIndex == 1) // Cable Connection Game
                stages[i] = new CableConnectionStageProgress();
            else // Default game
                stages[i] = new StageProgress();
        }

        ConvertDictionaryToList();
    }



    public void ConvertDictionaryToList()
    {
        if (stages != null)
        {
            stagesList.Clear();
            foreach (var pair in stages)
            {
                stagesList.Add(new SerializableStageProgress { stageIndex = pair.Key, progress = pair.Value });
            }
        }
    }

    public void ConvertListToDictionary()
    {
        if (stagesList == null || stagesList.Count == 0)
        {
            Debug.LogWarning("ConvertListToDictionary: stagesList was EMPTY! Creating default stages.");
            stagesList = new List<SerializableStageProgress>();

            for (int i = 0; i < 3; i++)
            {
                stagesList.Add(new SerializableStageProgress { stageIndex = i, progress = new StageProgress() });
            }
        }

        if (stages != null)
        {
            stages.Clear();
            foreach (var item in stagesList)
            {
                stages[item.stageIndex] = item.progress;
            }
        }


    }

    public bool CheckIfCompleted()
    {
        if (stages != null)
        {
            foreach (var stage in stages.Values)
            {
                if (!stage.isCompleted)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    [System.Serializable]
    public class StageProgress
    {
        public bool isCompleted;
        public int score;
        public float timeTaken;
        public int mistakes;

        public StageProgress()
        {
            isCompleted = false;
            score = 0;
            timeTaken = 0f;
            mistakes = 0;
        }
    }

    [System.Serializable]
    public class SerializableStageProgress
    {
        public int stageIndex;
        public StageProgress progress;
    }
    [System.Serializable]
    public class AsteroidStageProgress : StageProgress
    {
        public int incorrectAsteroids;
        public int bonusAsteroids;
    }
    public class EquipmentRecoveryStageProgress : StageProgress
    {
        public EquipmentRecoveryStageProgress()
        {
            mistakes = 0; // Initialize mistakes using the inherited field
        }
    }
    [System.Serializable]
    public class CableConnectionStageProgress : StageProgress
    {


        public CableConnectionStageProgress()
        {
            mistakes = 0; // Initialize mistakes to zero
        }
    }


}

