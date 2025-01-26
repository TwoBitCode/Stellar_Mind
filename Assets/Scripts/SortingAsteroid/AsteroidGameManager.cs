using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private AsteroidsGameIntroductionManager introductionManager;
    private bool isIntroductionComplete = false;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }

    private void Start()
    {
        if (endPanel != null) endPanel.SetActive(false);

        if (introductionManager != null)
        {
            // Play the introduction sequence
            introductionManager.PlayIntroduction(OnIntroductionComplete);
        }
        else
        {
            // Skip introduction and start the game
            Debug.LogWarning("Introduction Manager is missing. Starting challenges directly.");
            StartChallenges();
        }
    }
    private void OnIntroductionComplete()
    {
        isIntroductionComplete = true;
        Debug.Log("Introduction complete. Starting challenges...");
        StartChallenges();
    }

    private void StartChallenges()
    {
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
        if (currentChallenge == null)
        {
            return "הוראות אינן זמינות.";
        }

        // Use predefined Hebrew instructions if available
        if (!string.IsNullOrEmpty(currentChallenge.instructionsHebrew))
        {
            return currentChallenge.instructionsHebrew;
        }

        // Fallback to dynamically generated instructions
        string instructions = "מיינו את האסטרואידים לפי הכללים:\n";

        if (currentChallenge.mixedConditions != null && currentChallenge.mixedConditions.Count > 0)
        {
            foreach (var condition in currentChallenge.mixedConditions)
            {
                instructions += $"- {condition.size} {condition.displayName} ל-{condition.dropZoneName.Replace("Area", "").ToLower()}\n";
            }
        }
        else if (currentChallenge.dropZoneAssignments != null)
        {
            foreach (var assignment in currentChallenge.dropZoneAssignments)
            {
                instructions += $"- {assignment.assignedType} ל-{assignment.dropZoneName.Replace("Area", "").ToLower()}\n";
            }
        }

        return instructions;
    }




    private void AssignDropZoneTypes()
    {
        var currentChallenge = asteroidChallengeManager.CurrentChallenge;

        if (currentChallenge == null || currentChallenge.dropZones == null)
        {
            Debug.LogError("Invalid challenge setup!");
            return;
        }

        // Ensure dropZoneAssignments is initialized
        if (currentChallenge.dropZoneAssignments == null)
        {
            currentChallenge.dropZoneAssignments = new List<DropZoneAssignment>();
        }

        currentChallenge.dropZoneAssignments.Clear();

        // Handle mixed conditions
        if (currentChallenge.mixedConditions != null && currentChallenge.mixedConditions.Count > 0)
        {
            for (int i = 0; i < currentChallenge.mixedConditions.Count && i < currentChallenge.dropZones.Count; i++)
            {
                var condition = currentChallenge.mixedConditions[i];
                currentChallenge.dropZoneAssignments.Add(new DropZoneAssignment
                {
                    dropZoneName = currentChallenge.dropZones[i],
                    assignedType = $"{condition.size} {condition.color}" // E.g., "Small Red"
                });
            }
        }
        else if (currentChallenge.sortingRule is ISortingRule rule)
        {
            // Handle regular sorting rules
            foreach (var dropZoneName in currentChallenge.dropZones)
            {
                var assignedType = rule.GetRandomType();
                currentChallenge.dropZoneAssignments.Add(new DropZoneAssignment
                {
                    dropZoneName = dropZoneName,
                    assignedType = assignedType
                });
            }
        }
        else
        {
            Debug.LogError("No valid sorting rule or mixed conditions found for the challenge.");
        }

        // Log assignments for debugging
        foreach (var assignment in currentChallenge.dropZoneAssignments)
        {
            Debug.Log($"Drop Zone '{assignment.dropZoneName}' assigned to type '{assignment.assignedType}'");
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

        var currentChallenge = asteroidChallengeManager.CurrentChallenge;
        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        // Check if the current challenge allows distractors
        bool allowDistractors = currentChallenge.distractorPrefabs != null && currentChallenge.distractorPrefabs.Count > 0;

        // Decide whether to spawn a distractor only if they are allowed
        bool isDistractor = allowDistractors && Random.value < 0.2f; // Adjust the probability as needed

        // Select the appropriate prefab
        GameObject selectedPrefab;
        if (isDistractor)
        {
            // Pick a random distractor prefab
            selectedPrefab = currentChallenge.distractorPrefabs[Random.Range(0, currentChallenge.distractorPrefabs.Count)];
        }
        else
        {
            selectedPrefab = asteroidPrefab; // Use the regular asteroid prefab
        }

        // Calculate spawn position within the spawn area
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            0
        );

        // Instantiate the asteroid
        GameObject asteroid = Instantiate(selectedPrefab, spawnArea);
        asteroid.transform.localPosition = spawnPosition;

        if (!isDistractor)
        {
            // Apply sorting rules to regular asteroids
            ApplySortingRules(asteroid);
        }
        else
        {
            // Distractor logic: no sorting applied, purely for confusion
            Debug.Log("Spawned distractor asteroid!");
        }

        // Track the spawned asteroid
        activeAsteroids.Add(asteroid);
        currentAsteroidCount++;
    }




    private void ApplySortingRules(GameObject asteroid)
    {
        var currentChallenge = asteroidChallengeManager.CurrentChallenge;
        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        if (asteroid.TryGetComponent<SortingDraggableItem>(out var draggableItem))
        {
            if (currentChallenge.mixedConditions != null && currentChallenge.mixedConditions.Count > 0)
            {
                // Mixed Challenge: Assign size and color from MixedConditions
                int conditionIndex = currentAsteroidCount % currentChallenge.mixedConditions.Count;
                var condition = currentChallenge.mixedConditions[conditionIndex];

                // Map string size to scale values for UI Image
                Vector3 scale = condition.size == "Small" ? Vector3.one * 1.0f : Vector3.one * 1.7f;

                draggableItem.AssignedSize = condition.size == "Small" ? 1.0f : 3.0f; // Assign float value for logic
                draggableItem.AssignedColor = condition.color;
                draggableItem.AssignedType = $"{condition.size} {condition.displayName}";

                // Apply size and color to the UI Image
                var rectTransform = asteroid.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = scale; // Adjust scale for size
                }
                var image = asteroid.GetComponent<Image>();
                if (image != null)
                {
                    image.color = condition.color; // Apply color
                }

                Debug.Log($"Mixed Challenge: Assigned {draggableItem.AssignedType} with Scale: {scale} to {condition.dropZoneName}");
            }
            else if (currentChallenge.sortingRule is ISortingRule rule)
            {
                // Regular Challenge: Use sorting rule
                string assignedType = rule.GetCategory(asteroid);
                draggableItem.AssignedType = assignedType;
                rule.ApplyVisuals(asteroid);
                Debug.Log($"Regular Challenge: Assigned type: {assignedType}");
            }
            else
            {
                Debug.LogError("No valid sorting rule or mixed conditions found for the challenge.");
            }
        }
        else
        {
            Debug.LogError("SortingDraggableItem component is missing on the asteroid.");
        }
    }





    public void EndChallenge()
    {
        isGameActive = false; // Stop the game
        Debug.Log("Timer ended! Challenge is over.");

        // Clear all remaining asteroids
        ClearAsteroids();

        // Advance to the next challenge
        asteroidChallengeManager.AdvanceToNextChallenge();

        if (!asteroidChallengeManager.HasMoreChallenges)
        {
            Debug.Log("No more challenges. Showing the end panel.");
            ShowEndPanel(); // Show the end panel if all challenges are completed
        }
        else
        {
            Debug.Log($"Moving to the next challenge: {asteroidChallengeManager.CurrentChallenge.challengeName}");
            InitializeChallenge(); // Start the next challenge, including showing instructions
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
