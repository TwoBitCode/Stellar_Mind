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
    private float stageStartTime; //Tracks when the stage timer starts
    private float gameTimeRemaining;
    private bool isGameActive = false;

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel; // The panel that appears when the final stage is completed
    public TextMeshProUGUI levelCompleteText; // Text to display final stage points and bonus
    public UnityEngine.UI.Button levelCompleteButton; // Button to exit or return to the map

    [Header("Audio")]
    [SerializeField] private AudioSource tickingAudioSource;


    private Button currentStartButton;
    private float selectedTimeForCurrentStage = 0f;

    public bool isInteractionAllowed = false; // Prevent dragging until allowed
    private void Awake()
    {
        Instance = this; // Assign Instance without `DontDestroyOnLoad`
        Debug.Log("EquipmentRecoveryGameManager started!");
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

        int currentGameIndex = GetCurrentGameIndex();
        if (currentGameIndex == -1) return; // Prevent errors if game index is invalid

        int firstUnfinishedStage = GetFirstUnfinishedStage(currentGameIndex);

        currentStageIndex = firstUnfinishedStage;
        SaveOriginalPositions();

        Debug.Log($" Resuming at First Unfinished Stage {currentStageIndex}");

        // **Deactivate all panels first**
        foreach (var panel in stagePanels)
        {
            panel.SetActive(false);
        }

        // **Activate the correct panel**
        if (currentStageIndex >= 0 && currentStageIndex < stagePanels.Count)
        {
            stagePanels[currentStageIndex].SetActive(true);
            Debug.Log($"Activated Panel: {stagePanels[currentStageIndex].name}");
        }
        else
        {
            Debug.LogError($" Invalid stage index: {currentStageIndex}. No panel to activate.");
        }

        // **Reset variables**
        correctPartsPlaced = 0;
        placedParts.Clear();
        penalizedParts.Clear();
        isInteractionAllowed = false;
        isGameActive = false;
        gameTimerUI.SetActive(false);
        countdownTimerUI.SetActive(false);

        // **Reset object positions**
        if (stageOriginalPositions.ContainsKey(currentStageIndex))
        {
            foreach (var part in stageOriginalPositions[currentStageIndex])
            {
                part.Key.transform.position = part.Value;
            }
        }

        Debug.Log($"Starting Stage {currentStageIndex}: {stages[currentStageIndex].stageName}");

        // **Start the pre-game countdown and memory phase**
        ShowStageInstructionPanel(currentStageIndex);
    }

    private void ShowStageInstructionPanel(int stageIndex)
    {
        var panel = stagePanels[stageIndex];
        var instructionPanel = panel.transform.Find("InstructionPanel");

        if (instructionPanel != null)
        {
            instructionPanel.gameObject.SetActive(true);
            currentStartButton = instructionPanel.GetComponentInChildren<Button>();
            if (currentStartButton != null)
            {
                currentStartButton.interactable = false; // Disable until time is selected
            }
        }
        else
        {
            Debug.LogError($"InstructionPanel not found in stage panel {stageIndex}");
        }
    }

    private void SaveOriginalPositions()
    {
        if (!stageOriginalPositions.ContainsKey(currentStageIndex))
        {
            stageOriginalPositions[currentStageIndex] = new Dictionary<GameObject, Vector3>();
        }
        else
        {
            // **Clear and re-save positions to avoid duplication**
            stageOriginalPositions[currentStageIndex].Clear();
        }

        foreach (Transform part in stagePanels[currentStageIndex].transform)
        {
            stageOriginalPositions[currentStageIndex][part.gameObject] = part.position;
        }

        Debug.Log($"Saved original positions for Stage {currentStageIndex}.");
    }




    private IEnumerator PreGameCountdown()
    {
        Debug.Log("Starting memory phase countdown...");

        countdownTimerUI.SetActive(true); // Show the full countdown UI
        TextMeshProUGUI countdownText = countdownTimerUI.GetComponentInChildren<TextMeshProUGUI>();

        if (countdownText == null)
        {
            Debug.LogError("Countdown Timer UI is missing a TextMeshProUGUI component.");
            yield break;
        }

        float countdown = initialCountdownTime;
        while (countdown > 0)
        {
            // Play ticking sound every second
            if (tickingAudioSource != null && tickingAudioSource.clip != null)
            {
                tickingAudioSource.PlayOneShot(tickingAudioSource.clip);
            }

            countdownText.text = $"{Mathf.Ceil(countdown)}";
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownTimerUI.SetActive(false); // Hide the entire countdown UI
        StartCoroutine(DelayedTurnBlack()); // Proceed to turn black
    }






    public IEnumerator DelayedTurnBlack()
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
        Debug.Log("Starting game timer...");

        gameTimeRemaining = selectedTimeForCurrentStage; // Use selected time, NOT fixed stage time
        isGameActive = true;

        gameTimerUI.SetActive(false);
        TextMeshProUGUI gameTimerText = gameTimerUI.GetComponentInChildren<TextMeshProUGUI>();

        if (gameTimerText == null)
        {
            Debug.LogError("Game Timer UI is missing a TextMeshProUGUI component.");
            return;
        }

        gameTimerText.text = $"{Mathf.Ceil(gameTimeRemaining)}s";
        stageStartTime = Time.time;

        // You may want to re-enable the timer if desired
        // StartCoroutine(UpdateGameTimer(gameTimerText));
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
            //gameTimerUI.SetActive(false);
            //GameOver();
            yield break;
        }
    }

    public void PartPlacedCorrectly(GameObject part)
    {
        if (placedParts.Contains(part)) return;

        placedParts.Add(part);
        penalizedParts.Remove(part);
        correctPartsPlaced++;

        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{CurrentStage.totalParts}");

        // **Show feedback when a correct part is placed**
        EquipmentRecoveryUIManager.Instance?.ShowFeedback("יפה", Color.green);

        if (correctPartsPlaced >= CurrentStage.totalParts)
        {
            StopAllCoroutines(); // Stop all running coroutines (stops timer)
            gameTimerUI.SetActive(false);
            isGameActive = false;
            Debug.Log("All parts placed. Timer stopped!");

            StageComplete();
        }
    }



    //private IEnumerator DelayedStageComplete()
    //{
    //    Debug.Log("Stage completed! Waiting 3 seconds before moving to next stage...");
    //    yield return new WaitForSeconds(3f); // Wait 3 seconds before moving to the next stage

    //    currentStageIndex++;
    //    if (currentStageIndex < stages.Count)
    //    {
    //        StartStage();
    //    }
    //    else
    //    {
    //        MiniGameComplete();
    //    }
    //}

    public void PartPlacedIncorrectly(GameObject part)
    {
        if (!penalizedParts.Contains(part))
        {
            penalizedParts.Add(part); // Mark the part as penalized
            Debug.Log("Player lost a point for incorrect placement.");

            // Deduct 1 point using the OverallScoreManager
            OverallScoreManager.Instance?.AddScore(-1);
            EquipmentRecoveryUIManager.Instance?.ShowFeedback("נסה שוב", Color.red);
        }
        else
        {
            Debug.Log("This part was already penalized for being incorrect.");
        }
    }

    private void StageComplete()
    {
        Debug.Log($"Stage {CurrentStage.stageName} complete!");

        StopAllCoroutines();
        gameTimerUI.SetActive(false);
        isGameActive = false;
        isInteractionAllowed = false;

        // === 1. Track time and mistakes ===
        float timeSpent = Time.time - stageStartTime;
        int mistakes = penalizedParts.Count;

        Debug.Log($"Stage completed in {timeSpent:F2} sec with {mistakes} mistakes.");

        // === 2. Scoring Logic ===
        int baseScore = 60;

        float selectedTime = initialCountdownTime; // caregiver's selected memory time

        // Memory time bonus
        int memoryBonus = selectedTime switch
        {
            <= 10f => 20,
            <= 15f => 10,
            _ =>0
        };


        // Bonus for solving in half the time
        bool solvedFast = timeSpent <= (selectedTime / 2f);
        int speedBonus = solvedFast ? 10 : 0;

        int totalScore = baseScore + memoryBonus + speedBonus;

        Debug.Log($"Scoring  Base: {baseScore}, Memory Bonus: {memoryBonus}, Speed Bonus: {speedBonus}, Total: {totalScore}");

        // === 3. Save Progress ===
        int currentGameIndex = GetCurrentGameIndex();
        int currentStage = currentStageIndex;

        GameProgressManager.Instance.SaveStageProgress(
            currentGameIndex,
            currentStage,
            timeSpent,
            mistakes,
            mistakes,
            (int)selectedTime // optionally save selected time
        );

        var stageData = GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].stages[currentStage];
        stageData.isCompleted = true;
        stageData.score = totalScore;

        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Stage {currentStage} saved! Time: {timeSpent:F2} sec, Mistakes: {mistakes}");

        // === 4. Completion UI ===
        if (GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].CheckIfCompleted())
        {
            Debug.Log("All stages completed! Marking the entire mini-game as completed...");
            GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].isCompleted = true;
            GameProgressManager.Instance.SaveProgress();

            EquipmentRecoveryUIManager.Instance?.ShowLevelCompletePanel(baseScore + memoryBonus, speedBonus);
            return;
        }

        // === 5. Show reward panel if more stages remain ===
        if (currentStageIndex < stages.Count - 1)
        {
            EquipmentRecoveryUIManager.Instance?.ShowRewardPanel(baseScore + memoryBonus, speedBonus);
        }
    }









    //private IEnumerator DelayedStageTransition()
    //{
    //    Debug.Log("Waiting 3 seconds before moving to next stage...");
    //    yield return new WaitForSeconds(3f); // Wait 3 seconds

    //    currentStageIndex++;
    //    if (currentStageIndex < stages.Count)
    //    {
    //        StartStage(); // Start the next stage after waiting
    //    }
    //    else
    //    {
    //        MiniGameComplete(); // If no more stages, finish the mini-game
    //    }
    //}


    //private void MiniGameComplete()
    //{
    //    Debug.Log("Mini-game complete!");

    //    if (EquipmentRecoveryUIManager.Instance != null)
    //    {
    //        EquipmentRecoveryUIManager.Instance.ShowGameOverPanel();
    //    }
    //}


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
        Debug.Log($" Restarting Stage {currentStageIndex}...");

        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.HideGameOverPanel();
        }

        StopAllCoroutines(); // Stop any running timers or coroutines

        // **Ensure the correct panel is active**
        foreach (var panel in stagePanels)
        {
            panel.SetActive(false);
        }

        if (currentStageIndex >= 0 && currentStageIndex < stagePanels.Count)
        {
            stagePanels[currentStageIndex].SetActive(true);
            Debug.Log($"Activated Panel: {stagePanels[currentStageIndex].name}");
        }
        else
        {
            Debug.LogError($" Invalid stage index: {currentStageIndex}. No panel to activate.");
        }

        RestoreRobotImage();

        // **Reset all parts to their original positions and enable BLOCKRAYCASTS**
        if (stageOriginalPositions.ContainsKey(currentStageIndex))
        {
            foreach (var part in stageOriginalPositions[currentStageIndex])
            {
                part.Key.transform.position = part.Value;

                // **Enable BLOCKRAYCASTS if CanvasGroup exists**
                CanvasGroup canvasGroup = part.Key.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = true;
                    Debug.Log($"{part.Key.name} - BLOCKRAYCASTS enabled.");
                }
            }
        }
        else
        {
            Debug.LogError($"No original positions found for Stage {currentStageIndex}! Objects cannot reset.");
        }

        // **Reset game state variables**
        correctPartsPlaced = 0;
        placedParts.Clear();
        penalizedParts.Clear();
        isInteractionAllowed = false;
        isGameActive = false;
        gameTimerUI.SetActive(false);
        countdownTimerUI.SetActive(false);

        Debug.Log($" Restarting memory phase for Stage {currentStageIndex}...");
        ShowStageInstructionPanel(currentStageIndex);

    }

    private void RestoreRobotImage()
    {
        if (CurrentStage == null || CurrentStage.targetObjectNames == null) return;

        foreach (string targetObjectName in CurrentStage.targetObjectNames)
        {
            Transform targetTransform = stagePanels[currentStageIndex].transform.Find(targetObjectName);
            if (targetTransform != null)
            {
                Image image = targetTransform.GetComponent<Image>();
                if (image != null)
                {
                    image.color = Color.white; // Reset to white
                    Debug.Log($"Reset {targetObjectName} to white.");
                }
            }
        }
    }




    public void ReturnToMap()
    {
        Debug.Log("Returning to map...");

        int currentGameIndex = GetCurrentGameIndex();
        if (currentGameIndex == -1) return;

        int firstUnfinishedStage = GetFirstUnfinishedStage(currentGameIndex);

        Debug.Log($"Saving first unfinished stage: {firstUnfinishedStage} for game {currentGameIndex}");

        // **Save the correct unfinished stage**
        GameProgressManager.Instance.SetLastPlayedGame(currentGameIndex, firstUnfinishedStage);
        GameProgressManager.Instance.SaveProgress();

        // **Deactivate all panels before leaving**
        foreach (var panel in stagePanels)
        {
            panel.SetActive(false);
        }

        SceneManager.LoadScene("GameMapScene-V");
    }




    public void NextStage()
    {
        Debug.Log("Proceeding to next stage...");

        // **Hide the reward panel before switching**
        if (EquipmentRecoveryUIManager.Instance != null)
        {
            EquipmentRecoveryUIManager.Instance.HideRewardPanel();
        }

        int currentGameIndex = GetCurrentGameIndex();
        int currentStage = currentStageIndex;

        // *Mark the current stage as completed only now, after clicking Continue**
        GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].stages[currentStage].isCompleted = true;
        GameProgressManager.Instance.SaveProgress();
        Debug.Log($"Stage {currentStage} marked as completed!");

        // **Check if this is the LAST stage**
        if (currentStageIndex >= stages.Count - 1)
        {
            Debug.Log("Final stage completed! Marking the entire mini-game as completed...");

            // **Mark the entire mini-game as completed**
            GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].isCompleted = true;
            GameProgressManager.Instance.SaveProgress();

            // **Show the Level Complete panel**
            EquipmentRecoveryUIManager.Instance?.ShowLevelCompletePanel(
                CurrentStage.pointsForCompletion,
                CurrentStage.bonusPoints
            );

            Debug.Log(" Mini-game completed! Level Complete Panel displayed.");
            return; // **Exit here, no more stages**
        }

        // **Move to the next stage**
        currentStageIndex++;

        Debug.Log($"Starting Stage {currentStageIndex}: {stages[currentStageIndex].stageName}");

        // **Deactivate all panels first**
        foreach (var panel in stagePanels)
        {
            panel.SetActive(false);
        }

        // **Activate only the new stage panel**
        if (currentStageIndex >= 0 && currentStageIndex < stagePanels.Count)
        {
            stagePanels[currentStageIndex].SetActive(true);
            Debug.Log($"Activated Panel: {stagePanels[currentStageIndex].name}");
        }
        else
        {
            Debug.LogError($"Invalid stage index: {currentStageIndex}. No panel to activate.");
        }

        StartStage();
    }




    private int GetCurrentGameIndex()
    {
        // Adjust this based on how mini-games are indexed in GameProgressManager
        return 2; // Assuming Equipment Recovery is the third mini-game (0-based index)
    }
    private int GetFirstUnfinishedStage(int gameIndex)
    {
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (gameIndex < 0 || !playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogError($"Invalid game index {gameIndex} in GameProgressManager.");
            return 0; // Default to stage 0 if something goes wrong
        }

        var gameProgress = playerProgress.gamesProgress[gameIndex];

        for (int i = 0; i < gameProgress.stages.Count; i++)
        {
            if (!gameProgress.stages[i].isCompleted)
            {
                Debug.Log($"Returning first unfinished stage: {i}");
                return i; // Sirst unfinished stage
            }
        }

        Debug.Log("All stages completed. Returning the last stage.");
        return gameProgress.stages.Count - 1; // If all stages are complete, return the last stage
    }


    public void ActivateStagePanel(int stageIndex)
    {
        Debug.Log($"Activating stage panel for stage {stageIndex}");

        // Ensure all panels are disabled first**
        foreach (var panel in stagePanels)
        {
            panel.SetActive(false);
        }

        // Activate only the correct panel
        if (stageIndex >= 0 && stageIndex < stagePanels.Count)
        {
            stagePanels[stageIndex].SetActive(true);
            Debug.Log($"Activated Panel: {stagePanels[stageIndex].name}");
        }
        else
        {
            Debug.LogError($" Invalid stage index: {stageIndex}. No panel to activate.");
        }

        // Start the correct stage after activation
        currentStageIndex = stageIndex;
        //StartStage();
    }
    public void SelectMemoryTime(float time)
    {
        Debug.Log($"Memory time selected: {time} seconds");
        selectedTimeForCurrentStage = time;

        // 🔧 Re-fetch the button explicitly from the panel
        var panel = stagePanels[currentStageIndex];
        var instructions = panel.transform.Find("InstructionPanel");
        if (instructions != null)
        {
            var startBtn = instructions.transform.Find("StartButton")?.GetComponent<Button>();
            if (startBtn != null)
            {
                startBtn.interactable = true;
                currentStartButton = startBtn;
                Debug.Log("Start button enabled after memory time selection.");
            }
            else
            {
                Debug.LogError("StartButton not found inside InstructionPanel.");
            }
        }
        else
        {
            Debug.LogError("InstructionPanel not found on current stage panel.");
        }
    }


    public void ConfirmStartMemory()
    {
        if (selectedTimeForCurrentStage <= 0)
        {
            Debug.LogWarning("Start clicked but no time selected!");
            return;
        }

        initialCountdownTime = selectedTimeForCurrentStage;

        var panel = stagePanels[currentStageIndex];
        var instructions = panel.transform.Find("InstructionPanel");
        if (instructions != null)
            instructions.gameObject.SetActive(false);

        StartCoroutine(PreGameCountdown());
    }



}
