using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EquipmentRecoveryGameManager : MonoBehaviour
{
    public static EquipmentRecoveryGameManager Instance;

    [Header("Stage Settings")]
    public List<EquipmentRecoveryStage> stages;
    public List<GameObject> stagePanels;

    private int currentStageIndex = 0;
    private int correctPartsPlaced = 0;
    private HashSet<GameObject> placedParts = new HashSet<GameObject>();
    private HashSet<GameObject> penalizedParts = new HashSet<GameObject>(); // Tracks penalized objects
    [Header("Timers")]
    public GameObject countdownTimerUI; // Full GameObject (background + text) for the countdown
    public GameObject gameTimerUI; // Full GameObject (background + text) for the in-game timer
    public float initialCountdownTime = 5f; // Time before robot turns black

    private float gameTimeRemaining;
    private bool isGameActive = false;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel; // The panel that appears when the final stage is completed
    public TextMeshProUGUI levelCompleteText; // Text to display final stage points and bonus
    public UnityEngine.UI.Button levelCompleteButton; // Button to exit or return to the map


    public bool isInteractionAllowed = false; // Prevent dragging until allowed
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private EquipmentRecoveryStage CurrentStage => stages[currentStageIndex];

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();



    private Dictionary<int, Dictionary<GameObject, Vector3>> stageOriginalPositions = new Dictionary<int, Dictionary<GameObject, Vector3>>();

    public void StartStage()
    {
        Debug.Log("StartStage() called!");

        if (stages == null || stages.Count == 0)
        {
            Debug.LogError("Stages list is empty!");
            return;
        }

        if (currentStageIndex >= stages.Count)
        {
            Debug.LogError($"Invalid stage index: {currentStageIndex}. Stages count: {stages.Count}");
            return;
        }

        Debug.Log($"Activating Stage {currentStageIndex}: {stages[currentStageIndex].stageName}");

        // Reset logic for the new stage
        correctPartsPlaced = 0;
        placedParts.Clear();
        penalizedParts.Clear();
        isInteractionAllowed = false;

        // Activate the correct panel
        for (int i = 0; i < stagePanels.Count; i++)
        {
            stagePanels[i].SetActive(i == currentStageIndex);
        }

        // Store original positions for each stage
        if (!stageOriginalPositions.ContainsKey(currentStageIndex))
        {
            stageOriginalPositions[currentStageIndex] = new Dictionary<GameObject, Vector3>();

            foreach (Transform part in stagePanels[currentStageIndex].transform)
            {
                if (!stageOriginalPositions[currentStageIndex].ContainsKey(part.gameObject))
                {
                    stageOriginalPositions[currentStageIndex][part.gameObject] = part.position;
                }
            }
        }

        // Ensure timers are fully hidden at the start
        countdownTimerUI.SetActive(false);
        gameTimerUI.SetActive(false);

        Debug.Log($"Stage {currentStageIndex} ({CurrentStage.stageName}) has a time limit of {CurrentStage.stageTimeLimit} seconds.");

        StartCoroutine(PreGameCountdown());
    }


    private IEnumerator PreGameCountdown()
    {
        float countdown = initialCountdownTime;
        countdownTimerUI.SetActive(true); // Show the full countdown UI

        TextMeshProUGUI countdownText = countdownTimerUI.GetComponentInChildren<TextMeshProUGUI>();
        if (countdownText == null)
        {
            Debug.LogError("Countdown Timer UI is missing a TextMeshProUGUI component.");
            yield break;
        }

        while (countdown > 0)
        {
            countdownText.text = $"{Mathf.Ceil(countdown)}";
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownTimerUI.SetActive(false); // Hide the entire countdown UI
        StartCoroutine(DelayedTurnBlack());
    }



    private IEnumerator DelayedTurnBlack()
    {
        Debug.Log("Turning images black...");

        // Check if CurrentStage is valid
        if (CurrentStage == null)
        {
            Debug.LogError("CurrentStage is null! Cannot process target objects.");
            yield break;
        }

        // Check if targetObjectNames is valid
        if (CurrentStage.targetObjectNames == null || CurrentStage.targetObjectNames.Count == 0)
        {
            Debug.LogError($"Stage {CurrentStage.stageName} has no target objects!");
            yield break;
        }

        // Check if stagePanels is valid
        if (currentStageIndex >= stagePanels.Count || stagePanels[currentStageIndex] == null)
        {
            Debug.LogError($"Stage panel for index {currentStageIndex} is missing!");
            yield break;
        }

        foreach (string targetObjectName in CurrentStage.targetObjectNames)
        {
            Debug.Log($"Searching for {targetObjectName} in {stagePanels[currentStageIndex].name}");

            Transform targetTransform = stagePanels[currentStageIndex].transform.Find(targetObjectName);
            if (targetTransform == null)
            {
                Debug.LogWarning($"Target object {targetObjectName} not found in {stagePanels[currentStageIndex].name}.");
                continue;
            }

            GameObject targetObject = targetTransform.gameObject;
            Image image = targetObject.GetComponent<Image>();

            if (image == null)
            {
                Debug.LogWarning($"No Image component found on {targetObjectName} in {stagePanels[currentStageIndex].name}.");
                continue;
            }

            image.color = Color.black; // Set the image color to black
            Debug.Log($"Successfully turned {targetObjectName} black.");
        }

        // Enable interaction after the robot turns black
        isInteractionAllowed = true;
        Debug.Log("Interaction allowed.");

        // Debug before enabling the timer
        Debug.Log("Attempting to start the game timer...");
        StartGameTimer();

        yield return null;
    }



    private void StartGameTimer()
    {
        // Reset the time for the new stage
        gameTimeRemaining = CurrentStage.stageTimeLimit;

        if (gameTimerUI == null)
        {
            Debug.LogError("gameTimerUI is NULL! Make sure it is assigned in the Inspector.");
            return;
        }

        gameTimerUI.SetActive(true); // Ensure the timer UI is visible
        Debug.Log("Game Timer UI is now active.");

        // Find the correct TextMeshProUGUI component
        TextMeshProUGUI gameTimerText = gameTimerUI.GetComponentInChildren<TextMeshProUGUI>();
        if (gameTimerText == null)
        {
            Debug.LogError("Game Timer UI is missing a TextMeshProUGUI component.");
            return;
        }

        // Ensure the first frame updates correctly
        gameTimerText.text = $"Time Left: {Mathf.Ceil(gameTimeRemaining)}s";

        StopAllCoroutines(); // Stop any previous timer coroutine
        StartCoroutine(UpdateGameTimer(gameTimerText));
    }



    private IEnumerator UpdateGameTimer(TextMeshProUGUI gameTimerText)
    {
        isGameActive = true; // Ensure the game is marked as active

        while (gameTimeRemaining > 0)
        {
            if (!isGameActive) yield break; // Stop the timer if the game is not active

            gameTimerText.text = $"{Mathf.Ceil(gameTimeRemaining)}";
            yield return new WaitForSeconds(1f);
            gameTimeRemaining -= 1f;
        }

        // Only trigger GameOver if the player hasn't already won
        if (isGameActive)
        {
            gameTimerUI.SetActive(false);
            GameOver();
        }
    }


    public void PartPlacedCorrectly(GameObject part)
    {
        if (placedParts.Contains(part)) return; // Skip if the part is already placed correctly

        placedParts.Add(part);
        penalizedParts.Remove(part); // Clear penalty tracking on correct placement
        correctPartsPlaced++;

        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{CurrentStage.totalParts}");

        // Check if the stage is complete
        if (correctPartsPlaced >= CurrentStage.totalParts)
        {
            StageComplete();
        }
    }

    private IEnumerator DelayedStageComplete()
    {
        Debug.Log("Stage completed! Waiting 3 seconds before moving to next stage...");
        yield return new WaitForSeconds(3f); // Wait 3 seconds before moving to the next stage

        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            StartStage();
        }
        else
        {
            MiniGameComplete();
        }
    }

    public void PartPlacedIncorrectly(GameObject part)
    {
        if (!penalizedParts.Contains(part))
        {
            penalizedParts.Add(part); // Mark the part as penalized
            Debug.Log("Player lost a point for incorrect placement.");

            // Deduct 1 point using the OverallScoreManager
            OverallScoreManager.Instance?.AddScore(-1);
        }
        else
        {
            Debug.Log("This part was already penalized for being incorrect.");
        }
    }

    private void StageComplete()
    {
        Debug.Log($"Stage {CurrentStage.stageName} complete!");

        // Stop the game timer to prevent it from reaching zero
        StopAllCoroutines();
        gameTimerUI.SetActive(false); // Hide the timer UI

        isGameActive = false; // Prevent further interactions
        isInteractionAllowed = false; // Disable player input

        int totalPointsEarned = CurrentStage.pointsForCompletion;
        int bonusEarned = 0;

        // Check if the player earns a bonus
        if (gameTimeRemaining >= CurrentStage.bonusTimeThreshold)
        {
            bonusEarned = CurrentStage.bonusPoints;
            Debug.Log($"Bonus achieved! {bonusEarned} points awarded.");
        }

        // Add the score
        OverallScoreManager.Instance?.AddScore(totalPointsEarned + bonusEarned);

        if (currentStageIndex + 1 < stages.Count)
        {
            // Not the last stage - Show reward panel
            if (EquipmentRecoveryUIManager.Instance != null)
            {
                EquipmentRecoveryUIManager.Instance.ShowRewardPanel(totalPointsEarned, bonusEarned);
            }
        }
        else
        {
            // Last stage - Show Level Complete panel with final rewards
            if (EquipmentRecoveryUIManager.Instance != null)
            {
                EquipmentRecoveryUIManager.Instance.ShowLevelCompletePanel(totalPointsEarned, bonusEarned);
            }
        }

    }



    private IEnumerator DelayedStageTransition()
    {
        Debug.Log("Waiting 3 seconds before moving to next stage...");
        yield return new WaitForSeconds(3f); // Wait 3 seconds

        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            StartStage(); // Start the next stage after waiting
        }
        else
        {
            MiniGameComplete(); // If no more stages, finish the mini-game
        }
    }


    private void MiniGameComplete()
    {
        Debug.Log("Mini-game complete!");

        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.ShowGameOverPanel();
        }
    }


    public bool IsInteractionAllowed()
    {
        return isInteractionAllowed;
    }
    private void GameOver()
    {
        Debug.Log("Time's up! Game Over.");

        gameTimerUI.SetActive(false);
        countdownTimerUI.SetActive(false);
        isInteractionAllowed = false;

        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.ShowGameOverPanel();
        }

    }
    public void RestartStage()
    {
        Debug.Log($"Restarting Stage {currentStageIndex}...");

        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.HideGameOverPanel();
        }

        StopAllCoroutines(); // Stop any running timers

        // Reset all parts to their original positions for the current stage
        if (stageOriginalPositions.ContainsKey(currentStageIndex))
        {
            foreach (var part in stageOriginalPositions[currentStageIndex])
            {
                part.Key.transform.position = part.Value;
            }
        }
        else
        {
            Debug.LogError($"No original positions found for Stage {currentStageIndex}!");
        }

        // Reset game state variables
        correctPartsPlaced = 0;
        placedParts.Clear();
        penalizedParts.Clear();
        isInteractionAllowed = false;
        isGameActive = false;

        StartStage();
    }



    public void ReturnToMap()
    {
        Debug.Log("Returning to map...");
        SceneManager.LoadScene("MapScene"); // Make sure the scene name matches your map scene
    }
    public void NextStage()
    {
        Debug.Log("Proceeding to next stage...");

        // Hide the reward panel before switching
        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.HideRewardPanel();
        }

        currentStageIndex++;

        if (currentStageIndex < stages.Count)
        {
            Debug.Log($"Starting Stage {currentStageIndex}: {stages[currentStageIndex].stageName}");
            StartStage(); // Start the next stage
        }
        else
        {
            Debug.Log("No more stages remaining. Mini-game complete!");
            MiniGameComplete(); // If no more stages, finish the mini-game
        }
    }



}
