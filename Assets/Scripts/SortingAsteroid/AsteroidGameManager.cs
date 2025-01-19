using System.Collections.Generic;
using UnityEngine;

public class AsteroidGameManager : MonoBehaviour
{
    [Header("Asteroid Settings")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float spawnHorizontalRange = 200f;
    [SerializeField] private float spawnVerticalRange = 100f;
    [SerializeField] private GameObject endPanel; // Assign in Inspector
    [Header("Game Components")]
    [SerializeField] private GameTimer timerManager;
    [SerializeField] private AsteroidGameUIManager uiManager;
    private DoorManager doorManager;


    [Header("Challenge Manager")]
    [SerializeField] private AsteroidChallengeManager asteroidChallengeManager;

    public AsteroidChallengeManager ChallengeManager => asteroidChallengeManager;

    private float spawnDelay;
    private float spawnInterval;
    private int maxAsteroids;
    private int currentAsteroidCount;
    private bool isGameActive;
    private float spawnTimer;
    private List<GameObject> activeAsteroids = new List<GameObject>(); // Track spawned asteroids

    [Header("Game Components")]
    [SerializeField] private GameTimer gameTimer;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }

    private void Start()
    {
        // Ensure the end panel is hidden at the start
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        // Get the DoorManager instance
        doorManager = Object.FindFirstObjectByType<DoorManager>();
        if (doorManager == null)
        {
            Debug.LogError("DoorManager not found in the scene!");
        }

        gameTimer.OnTimerEnd += EndChallenge;
        InitializeChallenge();
    }




    private void Update()
    {
        if (!isGameActive || currentAsteroidCount >= maxAsteroids) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnAsteroid();
        }
    }

    private void InitializeChallenge()
    {
        ClearAsteroids();
        var currentChallenge = asteroidChallengeManager.CurrentChallenge;

        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        spawnDelay = currentChallenge.spawnDelay;
        spawnInterval = currentChallenge.spawnInterval;
        maxAsteroids = currentChallenge.maxAsteroids;
        currentAsteroidCount = 0;

        AssignDropZoneTypes();

        string instructions = GenerateInstructions();
        uiManager.ShowInstructions(instructions, StartGame);
    }


    private string GenerateInstructions()
    {
        if (asteroidChallengeManager.CurrentChallenge.sortingRule is ColorSortingRule)
        {
            return $"Drag {LeftType} asteroids to the left basket and {RightType} asteroids to the right basket.";
        }
        else if (asteroidChallengeManager.CurrentChallenge.sortingRule is SizeSortingRule)
        {
            return "Drag small asteroids to the left basket and large asteroids to the right basket.";
        }
        else
        {
            return "Follow the instructions for this challenge.";
        }
    }

    private void AssignDropZoneTypes()
    {
        var rule = asteroidChallengeManager.CurrentChallenge.sortingRule;

        if (rule == null)
        {
            Debug.LogError("No sorting rule assigned for the current challenge!");
            LeftType = "DefaultLeft";
            RightType = "DefaultRight";
            return;
        }

        if (rule is ColorSortingRule colorRule)
        {
            var types = colorRule.typeColorPairs;
            if (types.Count >= 2)
            {
                LeftType = types[0].color.ToString();
                RightType = types[1].color.ToString();
                Debug.Log($"Assigned LeftColor: {LeftType}, RightColor: {RightType}");
            }
            else
            {
                Debug.LogError("Not enough colors defined in the ColorSortingRule!");
                LeftType = "DefaultLeftColor";
                RightType = "DefaultRightColor";
            }
        }
        else if (rule is SizeSortingRule)
        {
            LeftType = "Small";
            RightType = "Large";
            Debug.Log($"Assigned LeftType: Small, RightType: Large");
        }
        else
        {
            Debug.LogError($"Unsupported sorting rule type: {rule.GetType().Name}");
            LeftType = "DefaultLeft";
            RightType = "DefaultRight";
        }
    }



    private void StartGame()
    {
        isGameActive = true;
        spawnTimer = 0f;
        timerManager.StartTimer();
        Debug.Log("Game started!");
    }

    private void SpawnAsteroid()
    {
        if (currentAsteroidCount >= maxAsteroids) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            0);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnArea);
        asteroid.transform.localPosition = spawnPosition;

        ApplySortingRules(asteroid);
        activeAsteroids.Add(asteroid);
        currentAsteroidCount++;
    }

    private void ApplySortingRules(GameObject asteroid)
    {
        var rule = asteroidChallengeManager.CurrentChallenge?.sortingRule as ISortingRule;

        if (rule == null)
        {
            Debug.LogError("Sorting rule is either not assigned or does not implement ISortingRule!");
            return;
        }

        string assignedType = rule.GetCategory(asteroid);
        Debug.Log($"Asteroid type assigned dynamically: {assignedType}");

        rule.ApplyVisuals(asteroid);
    }

    public void EndChallenge()
    {
        isGameActive = false; // Stop the game
        Debug.Log("Timer ended! Challenge is over.");

        // Clear all remaining asteroids
        ClearAsteroids();

        //// Mark the game as completed
        //if (doorManager != null)
        //{
        //    doorManager.MarkGameAsCompleted(2); // Replace 0 with the index of this mini-game
        //}

        // Check if there are more challenges
        asteroidChallengeManager.AdvanceToNextChallenge();

        if (!asteroidChallengeManager.HasMoreChallenges)
        {
            ShowEndPanel(); // Show the end panel if all challenges are completed
        }
        else
        {
            InitializeChallenge(); // Start the next challenge
        }
    }


    private void ShowEndPanel()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true); // Activate the end panel
        }

        uiManager.HideAllUI(); // Hide any remaining UI elements
    }


    private void ClearAsteroids()
    {
        foreach (var asteroid in activeAsteroids)
        {
            if (asteroid != null)
            {
                Destroy(asteroid); // Destroy each asteroid GameObject
            }
        }

        activeAsteroids.Clear(); // Clear the list to reset it
        Debug.Log("All remaining asteroids have been cleared.");
    }

}
