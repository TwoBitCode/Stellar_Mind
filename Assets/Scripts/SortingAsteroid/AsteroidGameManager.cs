using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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
    //private DoorManager doorManager;
    private RectTransform canvasRect; // Reference to the canvas RectTransform
    private AsteroidChallenge currentChallenge;

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
    private int correctAsteroids = 0;
    private int remainingTime;
    // private bool isChallengeFailed = false;
    [Header("Game Components")]
    [SerializeField] private AsteroidsGameIntroductionManager introductionManager;
    private bool isIntroductionComplete = false;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }
    private int correctAsteroidCount = 0;
    private int totalAsteroidCount = 0;
    private bool isChallengeActive = false;
    private OverallScoreManager overallScoreManager;


    private void Start()
    {
        // Initialization
        if (endPanel != null) endPanel.SetActive(false);

        overallScoreManager = OverallScoreManager.Instance;
        if (overallScoreManager == null)
        {
            Debug.LogError("OverallScoreManager instance is missing in the scene!");
        }

        if (introductionManager != null)
        {
            introductionManager.PlayIntroduction(OnIntroductionComplete);
        }
        else
        {
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
        timerManager.OnTimerEnd += EndChallenge;
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

        currentChallenge = asteroidChallengeManager.CurrentChallenge; // Get the current challenge

        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        spawnDelay = currentChallenge.spawnDelay;
        spawnInterval = currentChallenge.spawnInterval;
        maxAsteroids = currentChallenge.maxAsteroids;

        // Reset counts and states
        currentAsteroidCount = 0;
        correctAsteroidCount = 0;
        totalAsteroidCount = 0;
        isGameActive = false;

        AssignDropZoneTypes();
        SetupDropZones(currentChallenge.dropZones);

        string instructions = GenerateInstructions();
        uiManager.ShowInstructions(instructions, StartGame); // Only starts the game when the button is pressed
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

        if (currentChallenge == null || currentChallenge.dropZoneAssignments == null)
        {
            Debug.LogError("Invalid challenge setup!");
            return;
        }

        // Do nothing. Keep the manually assigned drop zones in Unity
        Debug.Log("Using predefined drop zone assignments. No randomization applied.");
    }




    private void StartGame()
    {
        if (currentChallenge == null)
        {
            Debug.LogError("No challenge available to start!");
            return;
        }

        isGameActive = true;
        spawnTimer = 0f;
        uiManager.ShowTimerText();

        // Set the timer duration based on the current challenge
        timerManager.SetDuration(currentChallenge.timeLimit);
        timerManager.StartTimer();

        Debug.Log($"Game started! Timer set to {currentChallenge.timeLimit} seconds.");
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
        isGameActive = false;
        Debug.Log("Timer ended! Challenge is over.");

        ClearAsteroids();

        if (correctAsteroidCount >= currentChallenge.minCorrectAsteroids)
        {
            CompleteChallenge();
        }
        else
        {
            FailChallenge();
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

    private void CompleteChallenge()
    {
        isChallengeActive = false;
        Debug.Log("Challenge completed successfully!");

        if (correctAsteroidCount >= currentChallenge.minCorrectAsteroids)
        {
            // Base points for completing the challenge
            int basePoints = currentChallenge.completionScore;
            OverallScoreManager.Instance?.AddScore(basePoints);

            // Bonus points for extra asteroids
            int bonusAsteroids = correctAsteroidCount - currentChallenge.minCorrectAsteroids;
            int bonusPoints = bonusAsteroids * currentChallenge.bonusScore;
            OverallScoreManager.Instance?.AddScore(bonusPoints);

            Debug.Log($"Challenge completed! Base Points: {basePoints}, Bonus Points: {bonusPoints}");

            // Show the success panel with a dynamic message
            uiManager.ShowSuccessPanel($"Challenge completed!\nBase Points: {basePoints}\nBonus Points: {bonusPoints}");

            // Start coroutine to wait a few seconds and then move to the next stage
            StartCoroutine(ProceedToNextChallenge());
        }
        else
        {
            FailChallenge();
        }
    }


    private void FailChallenge()
    {
        isChallengeActive = false;
        Debug.Log("Challenge failed. Showing failure panel.");

        uiManager.ShowFailurePanel(
            "Challenge failed. Try again or return to the menu.",
            RestartChallenge,  // Option to retry
            ReturnToMainMenu   // Option to go back to main menu
        );
    }



    private void RestartChallenge()
    {
        Debug.Log("Restarting challenge...");

        // If we want to forgive penalty points, refund half of them
        int refundPoints = (currentChallenge.scorePenalty * totalAsteroidCount) / 2;
        OverallScoreManager.Instance?.AddScore(refundPoints);
        Debug.Log($"Refunding {refundPoints} points for retrying.");

        // Reset challenge state
        correctAsteroidCount = 0;
        totalAsteroidCount = 0;
        isChallengeActive = true;

        // Restart challenge
        InitializeChallenge();
    }


    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    public void OnAsteroidSorted(bool isCorrect)
    {
        if (currentChallenge == null) return;

        totalAsteroidCount++;

        if (isCorrect)
        {
            correctAsteroidCount++;
        }
        else
        {
            // Deduct penalty for incorrect drop
            int penaltyPoints = currentChallenge.scorePenalty;
            OverallScoreManager.Instance?.AddScore(-penaltyPoints);
            Debug.Log($"Penalty applied: -{penaltyPoints}");
        }

        if (totalAsteroidCount >= currentChallenge.maxAsteroids)
        {
            if (correctAsteroidCount >= currentChallenge.minCorrectAsteroids)
            {
                CompleteChallenge();
            }
            else
            {
                FailChallenge();
            }
        }
    }
    private IEnumerator ProceedToNextChallenge()
    {
        // Optional: Add a delay before moving to the next challenge
        yield return new WaitForSeconds(3f); // Wait for 3 seconds (or adjust as needed)

        asteroidChallengeManager.AdvanceToNextChallenge();

        if (asteroidChallengeManager.HasMoreChallenges)
        {
            Debug.Log($"Moving to the next challenge: {asteroidChallengeManager.CurrentChallenge.challengeName}");

            // Reset UI and prepare the next challenge
            uiManager.HideAllUI();
            InitializeChallenge(); // Show instructions and reset the challenge
        }
        else
        {
            Debug.Log("No more challenges. Showing the end panel.");
            ShowEndPanel(); // Show the final completion screen
        }
    }




}

