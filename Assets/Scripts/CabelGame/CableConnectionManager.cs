using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CableConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CableConnectionStage
    {
        public GameObject connectedPanel;
        public GameObject disconnectedPanel;
        public DragCable[] cables;
        public RectTransform[] targets;
        public DialoguePanel dialoguePanel;
        public int scoreToAdd;
        public string completionMessage;
        public float stageTimeLimit;
        public int mistakePenalty = 5; // Points deducted per mistake
        public int bonusThreshold = 5; // Time (in seconds) needed for bonus
        public int bonusPoints = 10; // Bonus points for finishing early

    }

    public CableConnectionStage[] stages;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 2f;
    public Color correctFeedbackColor = Color.green;
    public Color stageCompletedFeedbackColor = Color.blue;

    [Header("Stage Management")]
    public int currentStage = 0;
    [SerializeField] private PanelTransitionHandler panelTransitionHandler;
    public TextMeshProUGUI timerText;
    public float countdownTime = 5f;
    public CountdownTimer countdownTimer;
    public GameObject timerUI;
    public GameObject startButton;

    [Header("Stage Timer")]
    public GameObject stageTimerUI;
    public TextMeshProUGUI stageTimerText;
    private float stageTimeRemaining;
    private bool isStageTimerRunning = false;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnToMapButton; //

    [Header("Stage Complete Panel")]
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private TextMeshProUGUI stageCompleteText;
    [SerializeField] private Button nextStageButton;

    [SerializeField] private SparkEffectHandler sparkEffectHandler;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button returnToMapButtonE; // New reference for returning to the map


    private int gameIndex = 1; // Set the correct game index

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (stageCompletePanel != null) stageCompletePanel.SetActive(false);
        if (stageTimerUI != null) stageTimerUI.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        timerUI.SetActive(false);

        // Load last played stage from GameProgressManager for the current game index
        int savedStage = 0;

        if (GameProgressManager.Instance != null)
        {
            var playerProgress = GameProgressManager.Instance.playerProgress;

            if (playerProgress.gamesProgress.ContainsKey(gameIndex))
            {
                savedStage = playerProgress.lastPlayedStage;
                Debug.Log($"Resuming from last played stage {savedStage} in Game {gameIndex}");
            }
            else
            {
                Debug.LogWarning($"No saved progress found for Game {gameIndex}. Starting from stage 0.");
            }
        }
        else
        {
            Debug.LogError("GameProgressManager instance is missing! Defaulting to stage 0.");
        }

        currentStage = savedStage;
        LoadStage(currentStage);
    }


    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            return;
        }

        currentStage = stageIndex;
        CableConnectionStage stage = stages[currentStage];
        // **Turn off all other stages' panels before enabling the current stage**
        foreach (var s in stages)
        {
            if (s.connectedPanel != null) s.connectedPanel.SetActive(false);
            if (s.disconnectedPanel != null) s.disconnectedPanel.SetActive(false);
            if (s.dialoguePanel != null) s.dialoguePanel.gameObject.SetActive(false);
        }

        // Ensure connected panel is shown first
        if (stage.connectedPanel != null) stage.connectedPanel.SetActive(true);
        if (stage.disconnectedPanel != null) stage.disconnectedPanel.SetActive(false);

        stageTimeRemaining = stage.stageTimeLimit;
        isStageTimerRunning = false;

        // Ensure the Dialogue Panel is turned off properly when returning mid-stage
        if (stage.dialoguePanel != null)
        {
            if (GameProgressManager.Instance.playerProgress.lastPlayedStage > 0)
            {
                Debug.Log("Returning mid-stage, ensuring dialogue panel is disabled.");
                stage.dialoguePanel.gameObject.SetActive(false);
                //Canvas dialogueCanvas = stage.dialoguePanel.GetComponentInParent<Canvas>();
                //if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);
            }
        }

        // Only show dialogue at the very first stage
        if (currentStage == 0 && GameProgressManager.Instance.playerProgress.lastPlayedStage == 0)
        {
            if (stage.dialoguePanel != null)
            {
                stage.dialoguePanel.gameObject.SetActive(true);
                Canvas dialogueCanvas = stage.dialoguePanel.GetComponentInParent<Canvas>();
                if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(true);

                stage.dialoguePanel.StartDialogue(() =>
                {
                    stage.dialoguePanel.gameObject.SetActive(false);
                   // if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(false);
                    StartMemoryCountdown(); // Start countdown after dialogue
                });
            }
        }
        else
        {
            Debug.Log("Skipping dialogue, going straight to memory countdown.");
            StartMemoryCountdown();
        }
    }


    public void OnStartButtonClick()
    {
        Debug.Log("Start button clicked, beginning memory countdown.");

        if (startButton != null) startButton.SetActive(false); // Hide start button after clicking

        // Hide dialogue panel & canvas if it exists
        CableConnectionStage stage = stages[currentStage];
        if (stage.dialoguePanel != null)
        {
            stage.dialoguePanel.gameObject.SetActive(false);

            Canvas dialogueCanvas = stage.dialoguePanel.GetComponentInParent<Canvas>();
            if (dialogueCanvas != null)
            {
                dialogueCanvas.gameObject.SetActive(false);
            }
        }

        StartMemoryCountdown();
    }


    private void StartMemoryCountdown()
    {
        Debug.Log("Restarting Memory Countdown.");

        if (timerUI != null)
        {
            timerUI.SetActive(true);
        }

        // Ensure previous event listeners are removed to prevent duplicate calls
        countdownTimer.OnTimerStart -= OnCountdownStart;
        countdownTimer.OnTimerUpdate -= OnCountdownUpdate;
        countdownTimer.OnTimerEnd -= OnCountdownEnd;

        // Assign fresh event listeners
        countdownTimer.OnTimerStart += OnCountdownStart;
        countdownTimer.OnTimerUpdate += OnCountdownUpdate;
        countdownTimer.OnTimerEnd += OnCountdownEnd;

        countdownTimer.StopCountdown();
        countdownTimer.timerText = timerText;
        countdownTimer.StartCountdown(countdownTime);
    }



    private void OnCountdownStart()
    {
        Debug.Log("Memory countdown started!");
    }

    private void OnCountdownUpdate(float timeRemaining)
    {
        // Debug.Log($"Memory Time Remaining: {timeRemaining}");
    }

    private void OnCountdownEnd()
    {
        Debug.Log("Memory countdown ended! Transitioning to disconnected panel with spark effect.");

        CableConnectionStage stage = stages[currentStage];

        // Ensure the timer UI is still active
        if (timerUI != null)
        {
            timerUI.SetActive(true);
        }

        // Change the timer color instead of stopping it
        if (timerText != null)
        {
            timerText.color = Color.red; // Change to red when the stage timer starts
        }

        // **Trigger spark effect and transition to disconnected panel**
        if (panelTransitionHandler != null)
        {
            panelTransitionHandler.ShowConnectedPanel(
                stage.connectedPanel,
                stage.disconnectedPanel,
                sparkEffectHandler,
                () =>
                {
                    // **Now hide the connected panel after the transition**
                    if (stage.connectedPanel != null)
                    {
                        stage.connectedPanel.SetActive(false);
                    }

                    // **Ensure the disconnected panel is active**
                    if (stage.disconnectedPanel != null)
                    {
                        stage.disconnectedPanel.SetActive(true);
                    }

                    // Start the stage timer after transition completes
                    StartStageTimer();
                }
            );
        }
    }




    private void StartStageTimer()
    {
        Debug.Log("Stage timer started!");

        isStageTimerRunning = true;

        // No need to enable a separate timer UI; it's already active
        StartCoroutine(StageTimerCoroutine());
    }


    private IEnumerator StageTimerCoroutine()
    {
        while (stageTimeRemaining > 0)
        {
            if (!isStageTimerRunning) yield break; // Stop immediately if stage is completed

            stageTimerText.text = Mathf.Ceil(stageTimeRemaining).ToString();
            stageTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        isStageTimerRunning = false;
        ShowGameOverPanel();
    }


    private void ShowGameOverPanel()
    {
        Debug.Log("Time ran out! Showing Game Over Panel.");

        CableConnectionStage stage = stages[currentStage];

        // Hide disconnected panel when game over
        if (stage.disconnectedPanel != null)
        {
            stage.disconnectedPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Handle Restart Button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                Debug.Log("Restarting current stage...");
                gameOverPanel.SetActive(false);
                RestartStage();
            });
        }

        // Handle Return to Map Button
        if (returnToMapButton != null)
        {
            returnToMapButton.onClick.RemoveAllListeners();
            returnToMapButton.onClick.AddListener(() =>
            {
                Debug.Log("Returning to game map...");
                ReturnToMap();
            });
        }
    }


    public void RestartStage()
    {
        Debug.Log("Restarting Stage...");

        isStageTimerRunning = false;

        // Reset the countdown timer properly
        countdownTimer.StopCountdown();

        // Reset stage time limit
        CableConnectionStage stage = stages[currentStage];
        stageTimeRemaining = stage.stageTimeLimit;

        // Reset the timer UI
        if (timerUI != null)
        {
            timerUI.SetActive(false);
        }

        // Reset cables to their original positions
        foreach (DragCable cable in stages[currentStage].cables)
        {
            cable.ResetToStartPosition();
        }

        // If retrying the first stage, skip dialogue and restart both countdowns
        if (currentStage == 0)
        {
            if (stage.connectedPanel != null)
            {
                stage.connectedPanel.SetActive(true);
            }
            StartMemoryCountdown();
        }
        else
        {
            LoadStage(currentStage);
        }
    }




    public void OnCableConnected(DragCable cable, RectTransform target)
    {
        CableConnectionStage stage = stages[currentStage];

        if (IsCorrectConnection(cable, target))
        {
            Debug.Log($"Cable {cable.name} is connected to the correct target: {target.name}");
            ShowFeedback("Correct Connection!", correctFeedbackColor);

            if (AllCablesConnected(stage))
            {
                Debug.Log("Stage completed!");
                ShowFeedback("Stage Completed!", stageCompletedFeedbackColor);

                if (OverallScoreManager.Instance != null)
                {
                    int finalScore = stage.scoreToAdd;

                    // Check if bonus should be awarded
                    if (stageTimeRemaining >= stage.bonusThreshold)
                    {
                        Debug.Log($"Bonus achieved! Adding {stage.bonusPoints} points.");
                        finalScore += stage.bonusPoints;
                    }

                    OverallScoreManager.Instance.AddScoreFromStage($"Stage {currentStage + 1}", finalScore);
                }

                ShowStageCompletePanel();
            }
        }
        else
        {
            Debug.Log($"Incorrect connection! Deducting {stage.mistakePenalty} points.");

            if (OverallScoreManager.Instance != null)
            {
                OverallScoreManager.Instance.AddScore(-(stage.mistakePenalty));
            }

            ShowFeedback($"Mistake! -{stage.mistakePenalty} points", Color.red);
        }
    }

    private bool IsCorrectConnection(DragCable cable, RectTransform target)
    {
        CableTarget targetScript = target.GetComponent<CableTarget>();
        return targetScript != null && targetScript.targetID == cable.name;
    }

    private bool AllCablesConnected(CableConnectionStage stage)
    {
        foreach (DragCable cable in stage.cables)
        {
            RectTransform connectedTarget = GetConnectedTarget(cable);
            if (connectedTarget == null || !IsCorrectConnection(cable, connectedTarget))
            {
                return false;
            }
        }
        return true;
    }

    private RectTransform GetConnectedTarget(DragCable cable)
    {
        foreach (RectTransform target in stages[currentStage].targets)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(target, cable.transform.position, null))
            {
                CableTarget targetScript = target.GetComponent<CableTarget>();
                if (targetScript != null && targetScript.targetID == cable.name)
                {
                    return target;
                }
            }
        }
        return null;
    }



    private void ShowStageCompletePanel()
    {
        Debug.Log("Stage completed! Hiding timer.");

        isStageTimerRunning = false;

        if (timerUI != null)
        {
            timerUI.SetActive(false);
        }

        CableConnectionStage currentStageData = stages[currentStage];

        // Hide panels
        if (currentStageData.connectedPanel != null) currentStageData.connectedPanel.SetActive(false);
        if (currentStageData.disconnectedPanel != null) currentStageData.disconnectedPanel.SetActive(false);

        // Update Progress
        var playerProgress = GameProgressManager.Instance.playerProgress;

        if (playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            GameProgress gameProgress = playerProgress.gamesProgress[gameIndex];

            if (gameProgress.stages.ContainsKey(currentStage))
            {
                gameProgress.stages[currentStage].isCompleted = true;
                Debug.Log($"Stage {currentStage} in Game {gameIndex} marked as completed.");
            }

            // Check if the whole game is now completed
            if (gameProgress.CheckIfCompleted())
            {
                Debug.Log($"Game {gameIndex} is now fully completed!");

                // Mark game as completed
                gameProgress.isCompleted = true;

                // Save final overall score when the game is completed
                playerProgress.totalScore = OverallScoreManager.Instance.OverallScore;
                Debug.Log($"Final Score for Game {gameIndex}: {playerProgress.totalScore}");
            }
        }
        else
        {
            Debug.LogError($"Game {gameIndex} not found in progress manager!");
        }

        GameProgressManager.Instance.SaveProgress(); // Save updated progress

        // Check if this is the last stage
        if (currentStage >= stages.Length - 1)
        {
            Debug.Log("Final stage completed! Showing end panel.");
            if (endPanel != null)
            {
                endPanel.SetActive(true); // Show the finish game panel

                // Ensure the Return to Map button works
                if (returnToMapButtonE != null)
                {
                    returnToMapButtonE.onClick.RemoveAllListeners();
                    returnToMapButtonE.onClick.AddListener(() =>
                    {
                        Debug.Log("Returning to the map after completing the game.");
                        ReturnToMap();
                    });
                }
            }
        }
        else
        {
            // Show stage complete panel for regular stage completion
            stageCompletePanel.SetActive(true);
            if (stageCompleteText != null)
            {
                string message = currentStageData.completionMessage;
                stageCompleteText.text = string.IsNullOrEmpty(message) ? "Well done!" : message;
            }

            if (nextStageButton != null)
            {
                nextStageButton.onClick.RemoveAllListeners();
                nextStageButton.onClick.AddListener(() =>
                {
                    stageCompletePanel.SetActive(false);
                    currentStage++;
                    LoadStage(currentStage);
                });
            }
        }
    }




    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color; // Set the color of the feedback text
            feedbackText.gameObject.SetActive(true);
            Invoke(nameof(HideFeedback), feedbackDuration);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
        }
    }
    public void ReturnToMap()
    {
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.SaveProgress();
        }

        Debug.Log("Returning to game selection map.");

        SceneManager.LoadScene("GameMapScene-V"); // Make sure the scene name is correct
    }

}
