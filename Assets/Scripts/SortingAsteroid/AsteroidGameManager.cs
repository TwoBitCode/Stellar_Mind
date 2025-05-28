using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    //private RectTransform canvasRect; // Reference to the canvas RectTransform
    private AsteroidChallenge currentChallenge;

    [Header("Challenge Manager")]
    [SerializeField] private AsteroidChallengeManager asteroidChallengeManager;
    private int currentChallengeIndex = 0;

    public AsteroidChallengeManager ChallengeManager => asteroidChallengeManager;

    // private float spawnDelay;
    private float spawnInterval;
    private int maxAsteroids;
    private int currentAsteroidCount;
    private int incorrectAsteroidCount;
    private int bonusAsteroidCount;
    private bool isGameActive;
    private float spawnTimer;
    private List<GameObject> activeAsteroids = new List<GameObject>(); // Track spawned asteroids
    [SerializeField] private List<GameObject> allDropZones; // Drag all drop zones here in the Inspector
    [Header("Game Components")]
    [SerializeField] private AsteroidsGameIntroductionManager introductionManager;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }
    private int correctAsteroidCount = 0;
    private int totalAsteroidCount = 0;
    private OverallScoreManager overallScoreManager;
    private float stageStartTime; // Tracks when the stage timer starts




    public AsteroidChallenge CurrentChallenge =>
        (currentChallengeIndex >= 0 && currentChallengeIndex < asteroidChallengeManager.Challenges.Count) ?
        asteroidChallengeManager.Challenges[currentChallengeIndex] : null;

    private void Start()
    {
        Debug.Log("AsteroidGameManager: Start() called.");
        if (endPanel != null) endPanel.SetActive(false);

        overallScoreManager = OverallScoreManager.Instance;
        if (overallScoreManager == null)
        {
            Debug.LogError("OverallScoreManager instance is missing in the scene!");
        }

        int currentGameIndex = 3; // Assuming this is the 4th mini-game (0-based index)
        GameProgress gameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex];

        // Check if ALL stages are completed
        bool allStagesCompleted = true;
        int lastUnfinishedStage = -1;

        foreach (var stage in gameProgress.stages)
        {
            if (!stage.Value.isCompleted)
            {
                allStagesCompleted = false;
                lastUnfinishedStage = stage.Key; // Track the last unfinished stage
            }
        }

        Debug.Log($"GameProgress hasStarted: {gameProgress.hasStarted}, Last Unfinished Stage: {lastUnfinishedStage}");

        // If all stages are completed, show the end panel
        if (allStagesCompleted)
        {
            Debug.Log("All challenges completed! Showing game complete panel.");
            ShowEndPanel();
            return;
        }

        // Always play intro for Stage 0
        if (lastUnfinishedStage == 2)
        {
            Debug.Log("Stage 0 detected. Playing introduction...");
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
        else
        {
            Debug.Log("Skipping introduction. Resuming from last played challenge.");
            StartChallenges();
        }
    }



    private void OnIntroductionComplete()
    {
        //isIntroductionComplete = true;
        Debug.Log("Introduction complete. Starting challenges...");
        StartChallenges();
    }

    private void StartChallenges()
    {
        if (introductionManager != null)
        {
            introductionManager.HideIntroduction(); // Hide the intro if it was skipped
        }

        Debug.Log("Starting Challenges...");
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

        int currentGameIndex = 3;
        GameProgress gameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex];

        int firstUnfinishedStage = -1;
        foreach (var stage in gameProgress.stages)
        {
            if (!stage.Value.isCompleted)
            {
                firstUnfinishedStage = stage.Key;
                break;
            }
        }

        if (firstUnfinishedStage == -1)
        {
            Debug.Log("All challenges completed! Showing game complete panel.");
            ShowEndPanel();
            return;
        }

        Debug.Log($"Resuming from challenge {firstUnfinishedStage}");
        currentChallengeIndex = firstUnfinishedStage;
        asteroidChallengeManager.SetCurrentChallengeIndex(currentChallengeIndex);
        currentChallenge = asteroidChallengeManager.CurrentChallenge;

        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        spawnInterval = currentChallenge.spawnInterval;
        maxAsteroids = currentChallenge.maxAsteroids;

        currentAsteroidCount = 0;
        correctAsteroidCount = 0;
        totalAsteroidCount = 0;
        isGameActive = false;

        AssignDropZoneTypes();
        SetupDropZones(currentChallenge.dropZones);

        // Update stage number text (e.g., 1/3)
        int totalStages = asteroidChallengeManager.GetChallengeCount();
        uiManager.UpdateStageNumber(currentChallengeIndex, totalStages);
        string instructions = GenerateInstructions();
        Debug.Log($"Updating instructions: {instructions}");
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

        if (currentChallenge == null || currentChallenge.dropZoneAssignments == null)
        {
            //   Debug.LogError("Invalid challenge setup!");
            return;
        }

        // Do nothing. Keep the manually assigned drop zones in Unity
        // Debug.Log("Using predefined drop zone assignments. No randomization applied.");
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

        // Use caregiver-selected duration instead of default
        float selectedTime = uiManager.SelectedDuration;
        if (selectedTime <= 0f)
        {
            Debug.LogWarning("No selected duration found. Falling back to 30 seconds.");
            selectedTime = 30f;
        }

        timerManager.SetDuration(selectedTime);
        timerManager.StartTimer();

        // Reset asteroid stats for this stage
        stageStartTime = Time.time;
        correctAsteroidCount = 0;
        incorrectAsteroidCount = 0;
        bonusAsteroidCount = 0;

        Debug.Log($"Game started! Timer set to {selectedTime} seconds.");
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
            // Debug.Log("Spawned distractor asteroid!");
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

                // Debug.Log($"Mixed Challenge: Assigned {draggableItem.AssignedType} with Scale: {scale} to {condition.dropZoneName}");
            }
            else if (currentChallenge.sortingRule is ISortingRule rule)
            {
                // Regular Challenge: Use sorting rule
                string assignedType = rule.GetCategory(asteroid);
                draggableItem.AssignedType = assignedType;
                rule.ApplyVisuals(asteroid);
                //Debug.Log($"Regular Challenge: Assigned type: {assignedType}");
            }
            else
            {
                // Debug.LogError("No valid sorting rule or mixed conditions found for the challenge.");
            }
        }
        else
        {
            // Debug.LogError("SortingDraggableItem component is missing on the asteroid.");
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
            endPanel.SetActive(true); // Show game completion panel

            //TextMeshProUGUI endText = endPanel.GetComponentInChildren<TextMeshProUGUI>();
            //if (endText != null)
            //{
            //    endText.text = "Great job! You have completed all asteroid challenges!";
            //}

            Button returnButton = endPanel.GetComponentInChildren<Button>();
            if (returnButton != null)
            {
                returnButton.onClick.RemoveAllListeners();
                returnButton.onClick.AddListener(ReturnToMainMenu);
                //returnButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return to Map";
            }
        }

        uiManager.HideAllUI(); // Hide all other UI elements
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
                // Debug.LogWarning($"Drop zone with name {zoneName} not found!");
            }
        }
    }

    private void CompleteChallenge()
    {
        Debug.Log("Challenge completed successfully!");

        // === 1. Base score: 60 points if correctAsteroidCount >= required ===
        int required = currentChallenge.minCorrectAsteroids;
        int basePoints = (correctAsteroidCount >= required) ? 60 : 0;
        OverallScoreManager.Instance?.AddScore(basePoints);

        // === 2. Bonus Asteroids ===
        bonusAsteroidCount = correctAsteroidCount - required;
        if (bonusAsteroidCount < 0) bonusAsteroidCount = 0;

        // === 3. Use caregiver-selected time ===
        float selectedTime = uiManager.SelectedDuration;
        if (selectedTime <= 0f)
        {
            Debug.LogWarning("Selected duration not found. Falling back to 30.");
            selectedTime = 30f;
        }

        // === 4. Bonus per asteroid based on selected time ===
        int bonusPerAsteroid = selectedTime switch
        {
            <= 20f => 5,
            <= 30f => 3,
            _ => 2
        };

        // === 5. Calculate total bonus ===
        int bonusPoints = bonusAsteroidCount * bonusPerAsteroid;
        OverallScoreManager.Instance?.AddScore(bonusPoints);

        Debug.Log($"Challenge completed! Base: {basePoints}, Bonus: {bonusPoints}, Bonus Asteroids: {bonusAsteroidCount}");

        // === 6. Save progress ===
        int currentGameIndex = 3;
        GameProgress gameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex];

        if (gameProgress.stages.ContainsKey(currentChallengeIndex))
        {
            gameProgress.stages[currentChallengeIndex].isCompleted = true;
            gameProgress.stages[currentChallengeIndex].score = OverallScoreManager.Instance?.OverallScore ?? 0;

            Debug.Log($"Challenge {currentChallengeIndex} marked as completed.");
        }

        if (gameProgress.CheckIfCompleted())
        {
            gameProgress.isCompleted = true;
            Debug.Log("All challenges in this game are completed!");
        }

        GameProgressManager.Instance.SaveStageProgress(
            currentGameIndex,
            currentChallengeIndex,
            gameProgress.stages[currentChallengeIndex].timeTaken,
            incorrectAsteroidCount,
            incorrectAsteroidCount,
            bonusAsteroidCount
        );

        GameProgressManager.Instance.SaveProgress();

        // === 7. Show appropriate panel ===
        if (currentChallengeIndex < asteroidChallengeManager.Challenges.Count - 1)
        {
            uiManager.ShowStageSuccessPanel(
     basePoints,
     bonusPoints,
     () => StartCoroutine(ProceedToNextChallenge()),
     ReturnToMainMenu // <== this is the missing fourth argument
 );

        }
        else
        {
            uiManager.ShowCompletionPanel(basePoints, bonusPoints, ReturnToMainMenu);
        }
    }







    private void FailChallenge()
    {
        //isChallengeActive = false;
        Debug.Log("Challenge failed. Showing failure panel.");

        uiManager.ShowFailurePanel(
            "לא הצלחנו במשימה, אבל בוא ננסה שוב!",
            RestartChallenge,  // אפשרות לנסות שוב
            ReturnToMainMenu   // אפשרות לחזור לתפריט הראשי
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
        //isChallengeActive = true;

        // Restart challenge
        InitializeChallenge();
    }


    private void ReturnToMainMenu()
    {
        // Save progress before leaving
        GameProgressManager.Instance.SaveProgress();

        // Reset any necessary game states
        isGameActive = false;
        ClearAsteroids();

        Debug.Log("Returning to Main Menu. Progress saved.");

        // Load the main menu scene
        SceneManager.LoadScene("GameMapScene-V");
    }



    public void OnAsteroidSorted(bool isCorrect, bool isBonus)
    {
        if (currentChallenge == null) return;

        totalAsteroidCount++;

        if (isCorrect)
        {
            correctAsteroidCount++;
        }
        else if (isBonus)
        {
            bonusAsteroidCount++; //  Keep tracking bonus asteroids even after min correct reached
        }
        else
        {
            incorrectAsteroidCount++;
            int penaltyPoints = currentChallenge.scorePenalty;
            OverallScoreManager.Instance?.AddScore(-penaltyPoints);
            Debug.Log($"Penalty applied: -{penaltyPoints}");
        }

        // Save the time when min correct asteroids are reached
        if (correctAsteroidCount == currentChallenge.minCorrectAsteroids)
        {
            float timeToComplete = Time.time - stageStartTime;
            Debug.Log($"Minimum correct asteroids reached! Saving time: {timeToComplete:F2} sec");

            int currentGameIndex = 3;
            GameProgressManager.Instance.SaveStageProgress(
                currentGameIndex,
                currentChallengeIndex,
                timeToComplete, // Save time at the right moment
                incorrectAsteroidCount,
                bonusAsteroidCount // Bonus asteroids not fully counted yet!
            );
        }
    }


    private IEnumerator ProceedToNextChallenge()
    {
        //yield return new WaitForSeconds(3f);

        int currentGameIndex = 3;
        GameProgress gameProgress = GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex];

        if (gameProgress.stages.ContainsKey(currentChallengeIndex))
        {
            gameProgress.stages[currentChallengeIndex].isCompleted = true;
            gameProgress.stages[currentChallengeIndex].score = OverallScoreManager.Instance.OverallScore;
        }

        GameProgressManager.Instance.SaveProgress();

        bool allStagesCompleted = true;
        int nextUnfinishedStage = -1;

        foreach (var stage in gameProgress.stages)
        {
            if (!stage.Value.isCompleted)
            {
                allStagesCompleted = false;
                nextUnfinishedStage = stage.Key;
                break;
            }
        }

        if (allStagesCompleted)
        {
            Debug.Log("All challenges completed. Showing completion panel.");
            uiManager.ShowCompletionPanel(
                gameProgress.stages[currentChallengeIndex].score,
                0, // Bonus points are already counted
                ReturnToMainMenu
            );
            yield break;
        }

        Debug.Log($"Moving to next challenge: {nextUnfinishedStage}");
        currentChallengeIndex = nextUnfinishedStage;
        asteroidChallengeManager.SetCurrentChallengeIndex(currentChallengeIndex);

        yield return null;

        uiManager.HideAllUI();
        InitializeChallenge();
    }




    public void SetCurrentChallengeIndex(int index)
    {
        if (index >= 0 && index < asteroidChallengeManager.GetChallengeCount())
        {
            currentChallengeIndex = index;
            Debug.Log($"AsteroidChallengeManager: Set challenge index to {currentChallengeIndex}");
        }
        else
        {
            Debug.LogError($"Invalid challenge index: {index}. Cannot set challenge.");
        }
    }




}

