using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
[System.Serializable]
public class SerializableGameProgress
{
    public int gameIndex;
    public GameProgress progress;
}

[System.Serializable]
public class CycleSummary
{
    public int cycleNumber;
    public int totalScore;
    public string startDate;
    public string endDate;
    public List<SerializableGameProgress> gamesSnapshot;
}

[System.Serializable]
public class PlayerProgress
{
    public string playerName;
    public string selectedCharacter;
    public int totalScore;
    public int lastPlayedGame;
    public int lastPlayedStage;
    public int currentCycle = 1;
    public string currentCycleStartDate;
    public List<CycleSummary> cycleHistory = new List<CycleSummary>();

    public List<SerializableGameProgress> gamesProgressList = new List<SerializableGameProgress>();
    public Dictionary<int, GameProgress> gamesProgress = new Dictionary<int, GameProgress>();
    public bool hasStartedCurrentCycle = false;


    public PlayerProgress(string name, string character)
    {
        playerName = name;
        selectedCharacter = character;
        totalScore = 100;
        lastPlayedGame = -1;
        lastPlayedStage = -1;
        currentCycle = 1;
        currentCycleStartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        gamesProgress = new Dictionary<int, GameProgress>();

        for (int i = 0; i < 4; i++) // Assuming 4 different games
        {
            if (i == 3) // Asteroid Game uses `AsteroidStageProgress`
                gamesProgress[i] = new GameProgress(3);
            else
                gamesProgress[i] = new GameProgress(i);
        }
        cycleHistory = new List<CycleSummary>(); // Initialize empty history
    }

    public void ConvertListToDictionary()
    {
        if (gamesProgress == null)
            gamesProgress = new Dictionary<int, GameProgress>();

        gamesProgress.Clear();

        if (gamesProgressList == null || gamesProgressList.Count == 0)
        {
            Debug.LogWarning("ConvertListToDictionary: gamesProgressList is empty. Creating defaults.");
            for (int i = 0; i < 4; i++)
            {
                gamesProgress[i] = new GameProgress(i);
            }
            return;
        }

        foreach (var item in gamesProgressList)
        {
            if (item == null)
            {
                Debug.LogWarning("ConvertListToDictionary: found null game item. Skipping.");
                continue;
            }

            if (item.progress == null)
            {
                Debug.LogWarning($"ConvertListToDictionary: game {item.gameIndex} has null progress. Creating default.");
                item.progress = new GameProgress(item.gameIndex);
            }

            item.progress.ConvertListToDictionary();
            gamesProgress[item.gameIndex] = item.progress;
        }
    }



    public void ConvertDictionaryToList()
    {
        if (gamesProgressList == null)
            gamesProgressList = new List<SerializableGameProgress>();
        else
            gamesProgressList.Clear();

        foreach (var pair in gamesProgress)
        {
            pair.Value.ConvertDictionaryToList();
            gamesProgressList.Add(new SerializableGameProgress
            {
                gameIndex = pair.Key,
                progress = pair.Value
            });
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
        if (stagesList == null)
            stagesList = new List<SerializableStageProgress>();
        else
            stagesList.Clear();

        if (stages != null)
        {
            foreach (var pair in stages)
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new StageProgressConverter() }
                };

                string json = JsonConvert.SerializeObject(pair.Value, settings);
                var converted = (StageProgress)JsonConvert.DeserializeObject(json, typeof(StageProgress), settings);

                // Log for debug
                // Debug.Log($"[ConvertDictionaryToList] stageIndex {pair.Key}  {converted.GetType().Name}, json: {json}");

                stagesList.Add(new SerializableStageProgress
                {
                    stageIndex = pair.Key,
                    progress = converted
                });
            }
        }
    }



    public void ConvertListToDictionary()
    {
        if (stages == null)
            stages = new Dictionary<int, StageProgress>();

        stages.Clear();

        if (stagesList == null || stagesList.Count == 0)
        {
            Debug.LogWarning("ConvertListToDictionary: stagesList was null or empty. Creating defaults.");
            stagesList = new List<SerializableStageProgress>();
            for (int i = 0; i < 3; i++)
            {
                stages[i] = new StageProgress();
            }
            return;
        }

        foreach (var item in stagesList)
        {
            if (item == null || item.progress == null)
            {
                Debug.LogWarning("ConvertListToDictionary: found null stage item. Skipping.");
                continue;
            }

            // Force re-casting using JSON roundtrip
            string json = JsonConvert.SerializeObject(item.progress);
            var correctType = (StageProgress)JsonConvert.DeserializeObject(json, typeof(StageProgress), new StageProgressConverter());

            stages[item.stageIndex] = correctType;
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

    [JsonConverter(typeof(StageProgressConverter))]
    [System.Serializable]
    public class StageProgress
    {
        public bool isCompleted;
        public int score;
        public float timeTaken;
        public int mistakes;
        public float selectedTime;

        public StageProgress()
        {
            isCompleted = false;
            score = 0;
            timeTaken = 0f;
            mistakes = 0;
            selectedTime = 0f;
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
            mistakes = 0;
            selectedTime = 0f;
        }
    }



}

