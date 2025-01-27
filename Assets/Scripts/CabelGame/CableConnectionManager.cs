using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CableConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CableConnectionStage
    {
        public GameObject connectedPanel; // Panel showing the connected state
        public GameObject disconnectedPanel; // Panel for the disconnected state
        public DragCable[] cables; // Cables to be used in the stage
        public RectTransform[] targets; // Shapes (targets) for the stage
        public DialoguePanel dialoguePanel; // Dialogue panel for the stage
        public int scoreToAdd; // Score to add upon stage completion
        public string completionMessage; // Custom message to show on the "Stage Complete" panel
    }

    public CableConnectionStage[] stages; // All stages in the mini-game

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText; // Feedback text for the player
    public float feedbackDuration = 2f; // Duration to show the feedback
    public Color correctFeedbackColor = Color.green; // Color for correct feedback
    public Color stageCompletedFeedbackColor = Color.blue; // Color for stage completed feedback

    [Header("Stage Management")]
    public int currentStage = 0;
    [SerializeField] private PanelTransitionHandler panelTransitionHandler;
    public TextMeshProUGUI timerText; // Text for the timer
    public float countdownTime = 5f; // Time for the countdown
    public CountdownTimer countdownTimer; // Reference to the CountdownTimer script
    public GameObject timerUI; // Assign the timer UI Image in the Inspector
    [Header("Stage Complete Panel")]
    [SerializeField] private GameObject stageCompletePanel; // Assign the "Stage Complete" panel in the Inspector
    [SerializeField] private TextMeshProUGUI stageCompleteText; // Optional: Text to display custom messages
    [SerializeField] private Button nextStageButton; // Button to proceed to the next stage


    [SerializeField] private SparkEffectHandler sparkEffectHandler; // Ensure this is assigned in the Inspector

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel; // Panel to show when all stages are completed
    private DoorManager doorManager;

    void Start()
    {
        // Ensure the end panel is initially hidden
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
        // Load the first stage
        timerUI.SetActive(false);

        LoadStage(currentStage);
    }


    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            if (endPanel != null)
            {
                endPanel.SetActive(true); // Show the end panel
            }
            return;
        }

        CableConnectionStage stage = stages[stageIndex];

        if (stage.connectedPanel != null)
        {
            stage.connectedPanel.SetActive(true); // Show the connected panel
        }

        // Handle dialogue panel and its canvas
        if (stage.dialoguePanel != null)
        {
            // Ensure the canvas containing the dialogue panel is active
            Canvas dialogueCanvas = stage.dialoguePanel.GetComponentInParent<Canvas>();
            if (dialogueCanvas != null)
            {
                dialogueCanvas.gameObject.SetActive(true); // Enable the canvas
            }

            stage.dialoguePanel.gameObject.SetActive(true); // Show the dialogue panel

            stage.dialoguePanel.StartDialogue(() =>
            {
                Debug.Log("Dialogue completed. Enabling timer UI.");

                // Turn off the dialogue panel and its canvas
                if (stage.dialoguePanel != null)
                {
                    stage.dialoguePanel.gameObject.SetActive(false);
                }
                if (dialogueCanvas != null)
                {
                    dialogueCanvas.gameObject.SetActive(false); // Disable the canvas
                }

                // Enable the timer UI
                if (timerUI != null)
                {
                    timerUI.SetActive(true);
                }
            });
        }
        else
        {
            // No dialogue for this stage; start the timer directly
            if (timerUI != null)
            {
                timerUI.SetActive(true);
            }
        }
    }







    public void OnCableConnected(DragCable cable, RectTransform target)
    {
        CableConnectionStage stage = stages[currentStage];

        // Validate if this cable is connected to the correct target
        if (IsCorrectConnection(cable, target))
        {
            Debug.Log($"Cable {cable.name} is connected to the correct target: {target.name}");
            ShowFeedback("Correct Connection!", correctFeedbackColor);

            // Check if all cables are connected to their correct targets
            if (AllCablesConnected(stage))
            {
                Debug.Log("Stage completed!");
                ShowFeedback("Stage Completed!", stageCompletedFeedbackColor);

                // Add the stage's score to the overall score
                if (OverallScoreManager.Instance != null)
                {
                    OverallScoreManager.Instance.AddScoreFromStage($"Stage {currentStage + 1}", stage.scoreToAdd);
                }

                // Show the "Stage Complete" panel
                ShowStageCompletePanel();
            }

        }
        else
        {
            Debug.Log($"Cable {cable.name} is not connected to the correct target.");
        }
    }

    private bool IsCorrectConnection(DragCable cable, RectTransform target)
    {
        CableTarget targetScript = target.GetComponent<CableTarget>();
        return targetScript != null && targetScript.targetID == cable.name;
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

    private bool AllCablesConnected(CableConnectionStage stage)
    {
        foreach (DragCable cable in stage.cables)
        {
            RectTransform connectedTarget = GetConnectedTarget(cable);
            if (connectedTarget == null || !IsCorrectConnection(cable, connectedTarget))
            {
                return false; // At least one cable is not correctly connected
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

    public void OnStartButtonClick()
    {
        CableConnectionStage stage = stages[currentStage];

        // Hide the dialogue panel when the start button is clicked
        if (stage.dialoguePanel != null)
        {
            stage.dialoguePanel.gameObject.SetActive(false);
        }
        timerUI.SetActive(true);
        // Start the countdown timer
        if (countdownTimer != null)
        {
            countdownTimer.timerText = timerText; // Set the timerText dynamically
            countdownTimer.OnTimerStart += OnCountdownStart;
            countdownTimer.OnTimerUpdate += OnCountdownUpdate;
            countdownTimer.OnTimerEnd += OnCountdownEnd;

            countdownTimer.StartCountdown(countdownTime);
        }
    }

    private void OnCountdownStart()
    {
        Debug.Log("Countdown started!");
    }

    private void OnCountdownUpdate(float timeRemaining)
    {
        //Debug.Log($"Time remaining: {timeRemaining}");
    }

    private void OnCountdownEnd()
    {
        Debug.Log("Countdown ended!");

        // Trigger the panel transition
        CableConnectionStage stage = stages[currentStage];
        if (panelTransitionHandler != null)
        {
            panelTransitionHandler.ShowConnectedPanel(
                stage.connectedPanel,
                stage.disconnectedPanel,
                sparkEffectHandler,
                () =>
                {
                    foreach (RectTransform target in stage.targets)
                    {
                        target.gameObject.SetActive(true);
                    }
                }
            );
        }

        // Cleanup timer events
        if (countdownTimer != null)
        {
            countdownTimer.OnTimerStart -= OnCountdownStart;
            countdownTimer.OnTimerUpdate -= OnCountdownUpdate;
            countdownTimer.OnTimerEnd -= OnCountdownEnd;
        }
    }
    private void ShowStageCompletePanel()
    {
        if (stageCompletePanel != null)
        {
            // כיבוי הפאנלים של השלב הנוכחי
            CableConnectionStage currentStageData = stages[currentStage];
            if (currentStageData.connectedPanel != null)
            {
                currentStageData.connectedPanel.SetActive(false);
            }
            if (currentStageData.disconnectedPanel != null)
            {
                currentStageData.disconnectedPanel.SetActive(false);
            }


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




}
