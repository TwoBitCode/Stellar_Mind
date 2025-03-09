using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameReportManager : MonoBehaviour
{
    private string previousScene;

    [Header("Asteroid Game Report UI References")]
    [SerializeField] private TextMeshProUGUI astroTimer1;
    [SerializeField] private TextMeshProUGUI astroMistakes1;
    [SerializeField] private TextMeshProUGUI astroBonus1;
    [SerializeField] private TextMeshProUGUI astroTimer2;
    [SerializeField] private TextMeshProUGUI astroMistakes2;
    [SerializeField] private TextMeshProUGUI astroBonus2;
    [SerializeField] private TextMeshProUGUI astroTimer3;
    [SerializeField] private TextMeshProUGUI astroMistakes3;
    [SerializeField] private TextMeshProUGUI astroBonus3;

    [Header("Equipment Recovery Game Report UI References")]
    [SerializeField] private TextMeshProUGUI equipTimer1;
    [SerializeField] private TextMeshProUGUI equipMistakes1;
    [SerializeField] private TextMeshProUGUI equipTimer2;
    [SerializeField] private TextMeshProUGUI equipMistakes2;
    [SerializeField] private TextMeshProUGUI equipTimer3;
    [SerializeField] private TextMeshProUGUI equipMistakes3;


    [Header("Tubes Game Report UI References")]
    [SerializeField] private TextMeshProUGUI tubesTimer1;
    [SerializeField] private TextMeshProUGUI tubesTimer2;
    [SerializeField] private TextMeshProUGUI tubesTimer3;

    private void Start()
    {
        previousScene = PlayerPrefs.GetString("PreviousScene", "GameMapScene-V");
        LoadAsteroidGameReport();
        LoadEquipmentRecoveryGameReport();
        LoadTubesGameReport();
    }

    public void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(previousScene);
    }
    private void LoadEquipmentRecoveryGameReport()
    {
        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError("GameProgressManager or PlayerProgress is missing!");
            return;
        }

        int gameIndex = 2; // Equipment Recovery Game Index
        if (!GameProgressManager.Instance.playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError("Equipment Recovery game progress not found!");
            return;
        }

        var equipGameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[gameIndex];

        if (equipGameProgress.stages.Count == 0)
        {
            Debug.LogError("No Equipment Recovery game stages found!");
            return;
        }

        for (int i = 1; i <= 3; i++)
        {
            if (!equipGameProgress.stages.ContainsKey(i - 1)) continue;

            var stageData = equipGameProgress.stages[i - 1];

            // ry explicit conversion (fallback in case of incorrect save format)
            if (stageData is GameProgress.EquipmentRecoveryStageProgress equipStage)
            {
                switch (i)
                {
                    case 1:
                        equipTimer1.text = $"{equipStage.timeTaken:F2}";
                        equipMistakes1.text = $"{equipStage.mistakes}";
                        break;
                    case 2:
                        equipTimer2.text = $"{equipStage.timeTaken:F2}";
                        equipMistakes2.text = $"{equipStage.mistakes} ";
                        break;
                    case 3:
                        equipTimer3.text = $"{equipStage.timeTaken:F2}";
                        equipMistakes3.text = $"{equipStage.mistakes}";
                        break;
                }
            }
            else
            {
                Debug.LogError($"Stage {i - 1} is not of type EquipmentRecoveryStageProgress! Data may not display correctly.");
                Debug.Log($"Stage {i - 1} actual type: {stageData.GetType().Name}"); // Debugging Line
            }
        }
    }
    private void LoadTubesGameReport()
    {
        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError("GameProgressManager or PlayerProgress is missing!");
            return;
        }

        int gameIndex = 0; // Tubes Game Index (adjust if needed)
        if (!GameProgressManager.Instance.playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError("Tubes game progress not found!");
            return;
        }

        var tubesGameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[gameIndex];

        if (tubesGameProgress.stages.Count == 0)
        {
            Debug.LogError("No Tubes game stages found!");
            return;
        }

        // Loop through the first 3 stages
        for (int i = 1; i <= 3; i++)
        {
            if (!tubesGameProgress.stages.ContainsKey(i - 1)) continue;

            var stageData = tubesGameProgress.stages[i - 1];

            // Ensure it's StageProgress before accessing timeTaken
            if (stageData is GameProgress.StageProgress tubesStage)
            {
                switch (i)
                {
                    case 1:
                        tubesTimer1.text = $"{tubesStage.timeTaken:F2}";
                        break;
                    case 2:
                        tubesTimer2.text = $"{tubesStage.timeTaken:F2}";
                        break;
                    case 3:
                        tubesTimer3.text = $"{tubesStage.timeTaken:F2}";
                        break;
                }
            }
            else
            {
                Debug.LogError($"Stage {i - 1} is not of type StageProgress! Data may not display correctly.");
                Debug.Log($"Stage {i - 1} actual type: {stageData.GetType().Name}"); // Debugging Line
            }
        }
    }


    private void LoadAsteroidGameReport()
    {
        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError("GameProgressManager or PlayerProgress is missing!");
            return;
        }

        int gameIndex = 3; // Asteroid Game Index
        if (!GameProgressManager.Instance.playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError("Asteroid game progress not found!");
            return;
        }

        var asteroidGameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[gameIndex];

        // Ensure that stages exist
        if (asteroidGameProgress.stages.Count == 0)
        {
            Debug.LogError("No asteroid game stages found!");
            return;
        }

        // Loop through the first 3 stages (adjust if needed)
        for (int i = 1; i <= 3; i++)
        {
            if (!asteroidGameProgress.stages.ContainsKey(i - 1)) continue;

            var stageData = asteroidGameProgress.stages[i - 1];

            // Ensure we are working with AsteroidStageProgress
            if (stageData is GameProgress.AsteroidStageProgress asteroidStage)
            {
                switch (i)
                {
                    case 1:
                        astroTimer1.text = $"{asteroidStage.timeTaken:F2}";
                        astroMistakes1.text = $"{asteroidStage.incorrectAsteroids}";
                        astroBonus1.text = $"{asteroidStage.bonusAsteroids}";
                        break;
                    case 2:
                        astroTimer2.text = $"{asteroidStage.timeTaken:F2}";
                        astroMistakes2.text = $"{asteroidStage.incorrectAsteroids}";
                        astroBonus2.text = $"{asteroidStage.bonusAsteroids}";
                        break;
                    case 3:
                        astroTimer3.text = $"{asteroidStage.timeTaken:F2}";
                        astroMistakes3.text = $"{asteroidStage.incorrectAsteroids}";
                        astroBonus3.text = $"{asteroidStage.bonusAsteroids}";
                        break;
                }
            }
            else
            {
                Debug.LogError($"Stage {i - 1} is not of type AsteroidStageProgress!");
            }
        }
    }
}