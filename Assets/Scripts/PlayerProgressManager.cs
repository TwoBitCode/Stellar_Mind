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

        for (int i = 0; i < 4; i++)
        {
            gamesProgress[i] = new GameProgress();
        }
    }

    public void ConvertListToDictionary()
    {
        if (gamesProgress == null)
        {
            Debug.LogError("ConvertListToDictionary: gamesProgress was NULL before conversion! Creating new Dictionary.");
            gamesProgress = new Dictionary<int, GameProgress>();
        }

        if (gamesProgressList == null || gamesProgressList.Count == 0)
        {
            Debug.LogError("ConvertListToDictionary: gamesProgressList is EMPTY! Initializing new data.");
            gamesProgressList = new List<SerializableGameProgress>();

            // אם רשימת המשחקים ריקה, ניצור 4 משחקים ריקים כדי למנוע באגים
            for (int i = 0; i < 4; i++)
            {
                gamesProgressList.Add(new SerializableGameProgress { gameIndex = i, progress = new GameProgress() });
            }
        }

        Debug.Log($"ConvertListToDictionary: Converting {gamesProgressList.Count} games...");

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
                item.progress = new GameProgress();
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

            for (int i = 0; i < 4; i++)
            {
                gamesProgress[i] = new GameProgress();
            }
        }

        gamesProgressList.Clear();
        foreach (var pair in gamesProgress)
        {
            pair.Value.ConvertDictionaryToList(); // ממיר גם את השלבים בתוך המשחק
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


    public GameProgress()
    {
        isCompleted = false;
        stages = new Dictionary<int, StageProgress>();
        stagesList = new List<SerializableStageProgress>();

        for (int i = 0; i < 3; i++)
        {
            stages[i] = new StageProgress();
        }
        ConvertDictionaryToList();
    }

    public void ConvertDictionaryToList()
    {
        stagesList.Clear();
        foreach (var pair in stages)
        {
            stagesList.Add(new SerializableStageProgress { stageIndex = pair.Key, progress = pair.Value });
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

        stages.Clear();
        foreach (var item in stagesList)
        {
            stages[item.stageIndex] = item.progress;
        }
    }

    public bool CheckIfCompleted()
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

}

[System.Serializable]
public class StageProgress
{
    public bool isCompleted;
    public int score;

    public StageProgress()
    {
        isCompleted = false;
        score = 0;
    }
}

[System.Serializable]
public class SerializableStageProgress
{
    public int stageIndex;
    public StageProgress progress;
}