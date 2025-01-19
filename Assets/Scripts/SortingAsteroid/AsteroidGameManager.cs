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
    private RectTransform canvasRect; // Reference to the canvas RectTransform

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
    [SerializeField] private List<GameObject> allDropZones; // Drag all drop zones here in the Inspector

    [Header("Game Components")]
    [SerializeField] private GameTimer gameTimer;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }

    private void Start()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        // Ensure the end panel is hidden at the start
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        // Get the DoorManager instance
        //doorManager = Object.FindFirstObjectByType<DoorManager>();
        //if (doorManager == null)
        //{
        //    Debug.LogError("DoorManager not found in the scene!");
        //}

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

        // Assign drop zones and update instructions
        AssignDropZoneTypes();
        SetupDropZones(currentChallenge.dropZones);

        string instructions = GenerateInstructions();
        uiManager.ShowInstructions(instructions, StartGame);
    }


    private string GenerateInstructions()
    {
        var currentChallenge = asteroidChallengeManager.CurrentChallenge;
        if (currentChallenge == null || currentChallenge.dropZoneAssignments == null)
        {
            return "Follow the sorting rules.";
        }

        string instructions = "Sort asteroids as follows:\n";
        foreach (var assignment in currentChallenge.dropZoneAssignments)
        {
            instructions += $"- {assignment.assignedType} asteroids to the {assignment.dropZoneName.Replace("Area", "").ToLower()}\n";
        }
        return instructions;
    }





    private void AssignDropZoneTypes()
    {
        var currentChallenge = asteroidChallengeManager.CurrentChallenge;

        if (currentChallenge == null || currentChallenge.dropZones == null || currentChallenge.sortingRule == null)
        {
            Debug.LogError("Invalid challenge setup!");
            return;
        }

        var rule = currentChallenge.sortingRule as MixedSortingRule;
        if (rule == null) return;

        currentChallenge.dropZoneAssignments.Clear();

        foreach (var condition in rule.conditions)
        {
            currentChallenge.dropZoneAssignments.Add(new DropZoneAssignment
            {
                dropZoneName = condition.dropZoneName,
                assignedType = $"{condition.size} {condition.color}"
            });
        }
    }






    private void StartGame()
    {
        isGameActive = true;
        spawnTimer = 0f;
        timerManager.StartTimer();
        Debug.Log("Game started!");
    }

    // Adjusted SpawnAsteroid method based on your earlier version
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
    private void SetupDropZones(List<string> activeDropZoneNames)
    {
        // Hide all drop zones
        foreach (var dropZone in allDropZones)
        {
            dropZone.SetActive(false);
        }

        // Activate only the required drop zones for the current challenge
        foreach (var zoneName in activeDropZoneNames)
        {
            var dropZone = allDropZones.Find(dz => dz.name == zoneName);
            if (dropZone != null)
            {
                dropZone.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Drop zone with name {zoneName} not found!");
            }
        }
    }

}
