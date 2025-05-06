using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class GameReportManager : MonoBehaviour
{
    private string previousScene;

    [Header("Cycle Navigation")]
    [SerializeField] private TextMeshProUGUI cycleHeaderText;
    [SerializeField] private Button nextCycleButton;
    [SerializeField] private Button prevCycleButton;

    [Header("Asteroid Game Report")]
    [SerializeField] private TextMeshProUGUI astroTimer1, astroMistakes1, astroBonus1, astroSelectedTime1;
    [SerializeField] private TextMeshProUGUI astroTimer2, astroMistakes2, astroBonus2, astroSelectedTime2;
    [SerializeField] private TextMeshProUGUI astroTimer3, astroMistakes3, astroBonus3, astroSelectedTime3;

    [Header("Equipment Recovery Game Report")]
    [SerializeField] private TextMeshProUGUI equipTimer1, equipMistakes1, equipSelectedTime1;
    [SerializeField] private TextMeshProUGUI equipTimer2, equipMistakes2, equipSelectedTime2;
    [SerializeField] private TextMeshProUGUI equipTimer3, equipMistakes3, equipSelectedTime3;

    [Header("Tubes Game Report")]
    [SerializeField] private TextMeshProUGUI tubesTimer1, tubesSelectedTime1;
    [SerializeField] private TextMeshProUGUI tubesTimer2, tubesSelectedTime2;
    [SerializeField] private TextMeshProUGUI tubesTimer3, tubesSelectedTime3;

    [Header("Cable Connection Game Report")]
    [SerializeField] private TextMeshProUGUI cableTimer1, cableMistakes1, cableSelectedTime1;
    [SerializeField] private TextMeshProUGUI cableTimer2, cableMistakes2, cableSelectedTime2;
    [SerializeField] private TextMeshProUGUI cableTimer3, cableMistakes3, cableSelectedTime3;

    private List<CycleSummary> cycleHistory;
    private int currentCycleIndex = 0;

    private async void Start()
    {
        previousScene = PlayerPrefs.GetString("LastSceneBeforeReport", "GameMapScene-V");

        if (GameProgressManager.Instance != null)
        {
            await GameProgressManager.Instance.LoadProgress();
        }

        var playerProgress = GameProgressManager.Instance.playerProgress;

        // Initialize cycleHistory list including completed + current cycle
        cycleHistory = new List<CycleSummary>();

        // Add all completed cycles
        if (playerProgress.cycleHistory != null)
        {
            cycleHistory.AddRange(playerProgress.cycleHistory);
        }

        // Add the current in-progress cycle
        var liveGames = new List<SerializableGameProgress>();
        foreach (var kvp in playerProgress.gamesProgress)
        {
            kvp.Value.ConvertDictionaryToList();
            liveGames.Add(new SerializableGameProgress
            {
                gameIndex = kvp.Key,
                progress = kvp.Value
            });
        }

        cycleHistory.Add(new CycleSummary
        {
            cycleNumber = playerProgress.currentCycle,
            startDate = playerProgress.currentCycleStartDate,
            endDate = "(כעת מתבצע)",  // Hebrew: "Currently in progress"
            gamesSnapshot = liveGames
        });

        // Enable navigation if needed
        nextCycleButton.onClick.AddListener(() => ChangeCycle(1));
        prevCycleButton.onClick.AddListener(() => ChangeCycle(-1));

        currentCycleIndex = cycleHistory.Count - 1; // Start by showing the current one
        DisplayCycle(currentCycleIndex);
    }


    private void ChangeCycle(int direction)
    {
        int total = cycleHistory.Count;
        currentCycleIndex = (currentCycleIndex + direction + total) % total;
        DisplayCycle(currentCycleIndex);
    }
    private void DisplayCycle(int index)
    {
        var cycle = cycleHistory[index];
      //  Debug.Log($"---- DISPLAYING CYCLE {cycle.cycleNumber} ({cycle.startDate} → {cycle.endDate}) ----");

        cycleHeaderText.text = $" {cycle.cycleNumber} ({cycle.startDate} - {cycle.endDate})";

        Dictionary<int, GameProgress> games = new Dictionary<int, GameProgress>();
        foreach (var item in cycle.gamesSnapshot)
        {
            // Force fix stages with correct subclass via StageProgressConverter
            item.progress.stages = new Dictionary<int, GameProgress.StageProgress>();

            foreach (var stageEntry in item.progress.stagesList)
            {
                try
                {
                    string stageJson = JsonConvert.SerializeObject(stageEntry.progress);
                    var converted = (GameProgress.StageProgress)JsonConvert.DeserializeObject(
                        stageJson,
                        typeof(GameProgress.StageProgress),
                        new StageProgressConverter()
                    );

                    item.progress.stages[stageEntry.stageIndex] = converted;

                    //Debug.Log($"Game {item.gameIndex}, Stage {stageEntry.stageIndex} → " +
                    //          $"Type: {converted.GetType().Name}, TimeTaken: {converted.timeTaken}, Score: {converted.score}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Deserialization failed for Game {item.gameIndex}, Stage {stageEntry.stageIndex}: {ex.Message}");
                }
            }

            games[item.gameIndex] = item.progress;
        }

        LoadAsteroidGameReport(games[3]);
        LoadEquipmentRecoveryGameReport(games[2]);
        LoadTubesGameReport(games[0]);
        LoadCableGameReport(games[1]);
    }


    private void LoadAsteroidGameReport(GameProgress game)
    {
        Debug.Log("Loading Asteroid report...");
        for (int i = 0; i < 3; i++)
        {
            if (game.stages.TryGetValue(i, out var data))
            {
               // Debug.Log($"Asteroid Stage {i} Type: {data.GetType().Name}");
                if (data is GameProgress.AsteroidStageProgress s)
                {
                   // Debug.Log($"  Time: {s.timeTaken}, Mistakes: {s.incorrectAsteroids}, Bonus: {s.bonusAsteroids}, SelTime: {s.selectedTime}");
                    switch (i)
                    {
                        case 0: astroTimer1.text = s.timeTaken.ToString("F2"); astroMistakes1.text = s.incorrectAsteroids.ToString(); astroBonus1.text = s.bonusAsteroids.ToString(); astroSelectedTime1.text = s.selectedTime + ""; break;
                        case 1: astroTimer2.text = s.timeTaken.ToString("F2"); astroMistakes2.text = s.incorrectAsteroids.ToString(); astroBonus2.text = s.bonusAsteroids.ToString(); astroSelectedTime2.text = s.selectedTime + ""; break;
                        case 2: astroTimer3.text = s.timeTaken.ToString("F2"); astroMistakes3.text = s.incorrectAsteroids.ToString(); astroBonus3.text = s.bonusAsteroids.ToString(); astroSelectedTime3.text = s.selectedTime + ""; break;
                    }
                }
                else
                {
                   // Debug.LogWarning($"Asteroid stage {i} is not AsteroidStageProgress. Actual type: {data.GetType().Name}");
                }
            }
            else
            {
                //Debug.LogWarning($"Asteroid stage {i} not found in dictionary.");
            }
        }
    }

    private void LoadEquipmentRecoveryGameReport(GameProgress game)
    {
       // Debug.Log("Loading Equipment Recovery report...");
        for (int i = 0; i < 3; i++)
        {
            if (game.stages.TryGetValue(i, out var data) && data is GameProgress.EquipmentRecoveryStageProgress s)
            {
               // Debug.Log($"Equip Stage {i}  Time: {s.timeTaken}, Mistakes: {s.mistakes}, SelTime: {s.selectedTime}");
                switch (i)
                {
                    case 0: equipTimer1.text = s.timeTaken.ToString("F2"); equipMistakes1.text = s.mistakes.ToString(); equipSelectedTime1.text = s.selectedTime + ""; break;
                    case 1: equipTimer2.text = s.timeTaken.ToString("F2"); equipMistakes2.text = s.mistakes.ToString(); equipSelectedTime2.text = s.selectedTime + ""; break;
                    case 2: equipTimer3.text = s.timeTaken.ToString("F2"); equipMistakes3.text = s.mistakes.ToString(); equipSelectedTime3.text = s.selectedTime + ""; break;
                }
            }
        }
    }

    private void LoadTubesGameReport(GameProgress game)
    {
        Debug.Log("Loading Tubes report...");
        for (int i = 0; i < 3; i++)
        {
            if (game.stages.TryGetValue(i, out var data) && data is GameProgress.StageProgress s)
            {
               // Debug.Log($"Tubes Stage {i}  Time: {s.timeTaken}, SelTime: {s.selectedTime}");
                switch (i)
                {
                    case 0: tubesTimer1.text = s.timeTaken.ToString("F2"); tubesSelectedTime1.text = s.selectedTime + ""; break;
                    case 1: tubesTimer2.text = s.timeTaken.ToString("F2"); tubesSelectedTime2.text = s.selectedTime + ""; break;
                    case 2: tubesTimer3.text = s.timeTaken.ToString("F2"); tubesSelectedTime3.text = s.selectedTime + ""; break;
                }
            }
        }
    }

    private void LoadCableGameReport(GameProgress game)
    {
        Debug.Log("Loading Cable report...");
        for (int i = 0; i < 3; i++)
        {
            if (game.stages.TryGetValue(i, out var data) && data is GameProgress.CableConnectionStageProgress s)
            {
              //  Debug.Log($"Cable Stage {i}  Time: {s.timeTaken}, Mistakes: {s.mistakes}, SelTime: {s.selectedTime}");
                switch (i)
                {
                    case 0: cableTimer1.text = s.timeTaken.ToString("F2"); cableMistakes1.text = s.mistakes.ToString(); cableSelectedTime1.text = s.selectedTime + ""; break;
                    case 1: cableTimer2.text = s.timeTaken.ToString("F2"); cableMistakes2.text = s.mistakes.ToString(); cableSelectedTime2.text = s.selectedTime + ""; break;
                    case 2: cableTimer3.text = s.timeTaken.ToString("F2"); cableMistakes3.text = s.mistakes.ToString(); cableSelectedTime3.text = s.selectedTime + ""; break;
                }
            }
        }
    }

    public void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(previousScene);
    }
}
