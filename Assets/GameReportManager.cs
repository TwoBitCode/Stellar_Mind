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

    private void Start()
    {
        previousScene = PlayerPrefs.GetString("PreviousScene", "GameMapScene-V");
        LoadAsteroidGameReport();
    }

    public void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(previousScene);
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
            if (stageData is AsteroidStageProgress asteroidStage)
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