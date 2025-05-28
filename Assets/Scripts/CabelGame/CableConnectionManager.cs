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

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 2f;
    public Color correctFeedbackColor = Color.green;
    public Color penaltyFeedbackColor = Color.red;
    private Color defaultScoreColor = Color.black;

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
    [SerializeField] private Button strategyButton;

    [Header("Stage Complete Panel")]
    [SerializeField] private GameObject stageCompletePanel;
    [SerializeField] private TextMeshProUGUI stageCompleteText;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private TextMeshProUGUI stageCompleteBaseScoreText; // NEW: Base score display
    [SerializeField] private TextMeshProUGUI stageCompleteBonusScoreText; // NEW: Bonus score display
    [SerializeField] private Button stageCompleteReturnToMapButton; // NEW
    [SerializeField] private Button stageCompleteStrategyButton;    // NEW



    [SerializeField] private SparkEffectHandler sparkEffectHandler;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button returnToMapButtonE; // New reference for returning to the map
    [SerializeField] private TextMeshProUGUI endBaseScoreText; // NEW: Base score display
    [SerializeField] private TextMeshProUGUI endBonusScoreText; // NEW: Bonus score display

    [Header("Strategy Panel")]
    [SerializeField] private StrategyManager strategyManager;
    [Header("Start Stage Panel")]
    [SerializeField] private GameObject startStagePanel; 
    [SerializeField] private Button startStageButton;
    [SerializeField] private Button dialogueNextButton; 
    [SerializeField] private Canvas dialogueCanvas;

    [Header("Instruction Audio")]
    [SerializeField] private AudioSource instructionAudioSource; // Specific audio source for instruction


    [Header("Memory Time Buttons")]
    [SerializeField] private Button timer15sButton;
    [SerializeField] private Button timer20sButton;
    [SerializeField] private Button timer25sButton;

    [SerializeField] private TextMeshProUGUI stageNumberText;


    private int gameIndex = 1; // Set the correct game index
    private Color defaultTimerColor; // Store original color
    private float stageStartTime; // Tracks when the stage timer starts
    private int mistakesCount = 0; // Track mistakes per stage

    private int selectedMemoryTime = 20;
    public int SelectedMemoryTime => selectedMemoryTime;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (stageCompletePanel != null) stageCompletePanel.SetActive(false);
        if (stageTimerText != null)
        {
            defaultTimerColor = stageTimerText.color; // Save the original color
        }
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
        timer15sButton.onClick.AddListener(() => SetMemoryTime(15));
        timer20sButton.onClick.AddListener(() => SetMemoryTime(20));
        timer25sButton.onClick.AddListener(() => SetMemoryTime(25));

        if (startStagePanel != null)
        {
            startStagePanel.SetActive(false);
        }
        currentStage = savedStage;
        LoadStage(currentStage);
    }
    public void SetMemoryTime(int seconds)
    {
        selectedMemoryTime = seconds;
        Debug.Log($"Memory time selected: {seconds}s");
    }

    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            return;
        }

        currentStage = stageIndex;
        if (stageNumberText != null)
        {
            stageNumberText.text = $"{currentStage + 1}/{stages.Length}";
        }

        mistakesCount = 0;
        CableConnectionStage stage = stages[currentStage];
        // Show intro only for first stage
        if (currentStage == 0 && stage.dialoguePanel != null)
        {
            if (dialogueCanvas != null) dialogueCanvas.gameObject.SetActive(true);
            stage.dialoguePanel.gameObject.SetActive(true);


            stage.dialoguePanel.StartDialogue(OnDialogueFinished);

            if (dialogueNextButton != null)
                dialogueNextButton.gameObject.SetActive(true);
            return;
        }

        // Turn off all panels
        foreach (var s in stages)
        {
            if (s.connectedPanel != null) s.connectedPanel.SetActive(false);
            if (s.disconnectedPanel != null) s.disconnectedPanel.SetActive(false);
            if (s.dialoguePanel != null) s.dialoguePanel.gameObject.SetActive(false);
        }

        if (stage.connectedPanel != null) stage.connectedPanel.SetActive(true);
        if (stage.disconnectedPanel != null) stage.disconnectedPanel.SetActive(false);

        stageTimeRemaining = stage.stageTimeLimit;
        isStageTimerRunning = false;

        // Always show the instruction panel before starting
        if (startStagePanel != null)
        {
            startStagePanel.SetActive(true);
        }

        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(true);
        }

        Debug.Log($"Loaded Stage {currentStage}. Awaiting memory time selection and start.");
    }


    public void OnStartStageButtonClick()
    {

        if (selectedMemoryTime <= 0)
        {
            Debug.LogWarning("Please select a memory time before starting.");
            return;
        }

        if (startStagePanel != null)
        {
            startStagePanel.SetActive(false);
        }

        if (dialogueCanvas != null)
        {
            dialogueCanvas.gameObject.SetActive(false);
        }

        if (instructionAudioSource != null && instructionAudioSource.isPlaying)
        {
            instructionAudioSource.Stop();
            Debug.Log("Stopped instruction audio on Start Stage button click.");
        }

        CableConnectionStage stage = stages[currentStage];
        if (stage.dialoguePanel != null)
        {
            var dialogueAudioSource = stage.dialoguePanel.GetComponent<AudioSource>();
            if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
            {
                dialogueAudioSource.Stop();
                Debug.Log("Stopped dialogue audio from dialogue panel on Start Stage click.");
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
        countdownTimer.StartCountdown(selectedMemoryTime);

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

        if (timerUI != null)
        {
            timerUI.SetActive(true);
        }

        // Change the timer color to red to indicate urgency
        if (timerText != null)
        {
            timerText.color = Color.red;
        }

        // **Start the stage timer IMMEDIATELY, before transition starts**
        StartStageTimer();
        if (timerUI != null)
        {
            timerUI.SetActive(false); // Hides the timer before cable interaction starts
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
                    if (stage.connectedPanel != null)
                    {
                        stage.connectedPanel.SetActive(false);
                    }

                    if (stage.disconnectedPanel != null)
                    {
                        stage.disconnectedPanel.SetActive(true);
                    }
                }
            );
        }
    }


    private void StartStageTimer()
    {
        Debug.Log("Stage timer started!");

        isStageTimerRunning = true;

        // Start tracking time when the stage begins
        stageStartTime = Time.time;

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


        // ShowGameOverPanel();
        isStageTimerRunning = false;
        yield break; // Stop silently — player continues

    }


    public void ShowGameOverPanel()
    {
        Debug.Log("Game Over triggered. Stopping everything and showing panel.");

        CableConnectionStage stage = stages[currentStage];

        // 1. עצירת כל הטיימרים
        isStageTimerRunning = false;
        countdownTimer.StopCountdown();

        if (timerUI != null)
            timerUI.SetActive(false);
        if (stageTimerUI != null)
            stageTimerUI.SetActive(false);

        // 2. איפוס כבלים – רק אם disconnectedPanel פעיל (כלומר כבר הופיעו הכבלים)
        if (stage.disconnectedPanel != null && stage.disconnectedPanel.activeInHierarchy)
        {
            foreach (var cable in stage.cables)
            {
                if (cable != null)
                    cable.ResetToStartPosition();
            }
        }
        else
        {
            Debug.Log("Skipped cable reset — disconnectedPanel not active yet.");
        }

        // 3. כיבוי גם של connectedPanel וגם disconnectedPanel — לא משנה איפה היינו
        if (stage.connectedPanel != null)
            stage.connectedPanel.SetActive(false);
        if (stage.disconnectedPanel != null)
            stage.disconnectedPanel.SetActive(false);

        // 4. הצגת פאנל Game Over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // 5. כפתור "נסה שוב"
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

        // 6. כפתור חזרה למפה
        if (returnToMapButton != null)
        {
            returnToMapButton.onClick.RemoveAllListeners();
            returnToMapButton.onClick.AddListener(() =>
            {
                Debug.Log("Returning to game map...");
                ReturnToMap();
            });
        }

        // 7. כפתור אסטרטגיה
        if (strategyButton != null)
        {
            strategyButton.onClick.RemoveAllListeners();
            strategyButton.onClick.AddListener(() =>
            {
                if (strategyManager != null)
                {
                    strategyManager.ShowNextStrategy();
                }
            });
        }
    }




    public void RestartStage()
    {
        Debug.Log("Restarting Stage...");

        isStageTimerRunning = false;

        // עצירת קאונטר זיכרון
        countdownTimer.StopCountdown();

        // שליפת שלב נוכחי
        CableConnectionStage stage = stages[currentStage];
        stageTimeRemaining = stage.stageTimeLimit;

        // הסתרת טיימרים
        if (timerUI != null)
            timerUI.SetActive(false);

        // איפוס כבלים — רק אם disconnectedPanel פעיל (כלומר הכבלים הופיעו)
        if (stage.disconnectedPanel != null && stage.disconnectedPanel.activeInHierarchy)
        {
            foreach (DragCable cable in stage.cables)
            {
                if (cable != null)
                    cable.ResetToStartPosition();
            }
        }
        else
        {
            Debug.Log("No need to reset cables — disconnectedPanel not active yet.");
        }

        // שליטה על הדיאלוג והתחלה מחדש
        if (currentStage == 0)
        {
            if (stage.connectedPanel != null)
                stage.connectedPanel.SetActive(true);

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
            ShowFeedback("יפה!", correctFeedbackColor);

            if (AllCablesConnected(stage))
            {
                Debug.Log("Stage completed!");

                if (OverallScoreManager.Instance != null)
                {
                    float timeSpent = Time.time - stageStartTime;

                    // Scoring logic
                    int basePoints = 60;
                    int timeBonus = selectedMemoryTime == 15 ? 20 :
                                    selectedMemoryTime == 20 ? 10 : 0;
                    int speedBonus = timeSpent <= selectedMemoryTime / 2f ? 10 : 0;

                    int finalScore = basePoints + timeBonus + speedBonus;

                    Debug.Log($"Score → Base: {basePoints}, TimeBonus: {timeBonus}, SpeedBonus: {speedBonus}, Final: {finalScore}");

                    OverallScoreManager.Instance.AddScoreFromStage($"Stage {currentStage + 1}", finalScore);
                    ShowStageCompletePanel(basePoints, timeBonus, speedBonus);

                }



            }
        }
        else
        {
            mistakesCount++;
            Debug.Log($"Incorrect connection! Deducting {stage.mistakePenalty} points.");

            if (OverallScoreManager.Instance != null)
            {
                OverallScoreManager.Instance.AddScore(-stage.mistakePenalty);
                StartCoroutine(FlashScorePenalty());
            }

            ShowFeedback($"טעות!", Color.red);
        }
    }


    private IEnumerator FlashScorePenalty()
    {
        if (scoreText != null)
        {
            scoreText.color = penaltyFeedbackColor; // Turn red
            yield return new WaitForSeconds(1f);
            scoreText.color = defaultScoreColor; // Restore default color
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
    public void OnCableResetMistake()
    {
        CableConnectionStage stage = stages[currentStage];

        // Increase mistakes count
        mistakesCount++;

        Debug.Log($"Cable reset! Mistake counted. Total mistakes: {mistakesCount}");

        // Deduct score penalty
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(-stage.mistakePenalty);
            StartCoroutine(FlashScorePenalty()); // Flash penalty effect
        }

        //// Play mistake sound (if available)
        //if (CableAudioManager.Instance != null && CableAudioManager.Instance.mistakeSound != null)
        //{
        //    CableAudioManager.Instance.PlayOneShot(CableAudioManager.Instance.mistakeSound);
        //}

        ShowFeedback($"טעות!", Color.red);
    }



    private void ShowStageCompletePanel(int baseScore, int timeBonus, int speedBonus)
    {
        isStageTimerRunning = false;
        Debug.Log("Stage completed! Hiding timer.");

        // Hide timer UI
        if (stageTimerText != null) stageTimerText.color = defaultTimerColor;
        if (timerUI != null) timerUI.SetActive(false);

        CableConnectionStage currentStageData = stages[currentStage];

        // Hide panels
        if (currentStageData.connectedPanel != null) currentStageData.connectedPanel.SetActive(false);
        if (currentStageData.disconnectedPanel != null) currentStageData.disconnectedPanel.SetActive(false);

        // Update Progress
        var playerProgress = GameProgressManager.Instance.playerProgress;
        float timeSpent = Time.time - stageStartTime;

        if (playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            GameProgress gameProgress = playerProgress.gamesProgress[gameIndex];

            if (gameProgress.stages.ContainsKey(currentStage))
            {
                gameProgress.stages[currentStage].isCompleted = true;
                GameProgressManager.Instance.SaveStageProgress(gameIndex, currentStage, timeSpent, mistakesCount);
                Debug.Log($"Stage {currentStage} in Game {gameIndex} marked as completed.");
            }

            if (gameProgress.CheckIfCompleted())
            {
                Debug.Log($"Game {gameIndex} is now fully completed!");
                gameProgress.isCompleted = true;
                playerProgress.totalScore = OverallScoreManager.Instance.OverallScore;
                Debug.Log($"Final Score for Game {gameIndex}: {playerProgress.totalScore}");
            }
        }
        else
        {
            Debug.LogError($"Game {gameIndex} not found in progress manager!");
        }

        GameProgressManager.Instance.SaveProgress();

        // --- Show either final panel or stage panel ---
        if (currentStage >= stages.Length - 1)
        {
            Debug.Log("Final stage completed! Showing End Panel.");

            if (endPanel != null)
            {
                endPanel.SetActive(true);
                endBaseScoreText.text = $"{baseScore + timeBonus}"; // Show full base + time bonus
                endBonusScoreText.text = $"{speedBonus}";           // Show speed bonus

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
            stageCompletePanel.SetActive(true);

            if (stageCompleteText != null)
            {
                string message = currentStageData.completionMessage;
                // Optional: stageCompleteText.text = string.IsNullOrEmpty(message) ? "Well done!" : message;
            }

            stageCompleteBaseScoreText.text = $"{baseScore + timeBonus}";
            stageCompleteBonusScoreText.text = $"{speedBonus}";

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

            if (stageCompleteReturnToMapButton != null)
            {
                stageCompleteReturnToMapButton.onClick.RemoveAllListeners();
                stageCompleteReturnToMapButton.onClick.AddListener(() =>
                {
                    Debug.Log("Returning to Map from Stage Complete Panel...");
                    ReturnToMap();
                });
            }

            if (stageCompleteStrategyButton != null)
            {
                stageCompleteStrategyButton.onClick.RemoveAllListeners();
                stageCompleteStrategyButton.onClick.AddListener(() =>
                {
                    if (strategyManager != null)
                    {
                        strategyManager.ShowNextStrategy();
                    }
                });
            }
        }
    }


    public void OnDialogueFinished()
    {
        // כיבוי כפתור הדיאלוג כדי למנוע לחיצות נוספות
        if (dialogueNextButton != null)
        {
            dialogueNextButton.gameObject.SetActive(false);
        }

        // הצגת הפאנל של "Start Stage"
        if (startStagePanel != null)
        {
            startStagePanel.SetActive(true);
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
