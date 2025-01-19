using UnityEngine;
using TMPro;
using System.Collections;

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

        // Access the DoorManager singleton
        doorManager = Object.FindFirstObjectByType<DoorManager>();
        if (doorManager == null)
        {
            Debug.LogError("DoorManager not found in the scene!");
        }

        // Load the first stage
        LoadStage(currentStage);
    }


    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");

            // Mark the mini-game as completed in DoorManager
            //if (doorManager != null)
            //{
            //    doorManager.MarkGameAsCompleted(1); // Replace 3 with the correct index for this mini-game
            //}
            //else
            //{
            //    Debug.LogError("DoorManager instance not found!");
            //}

            // Show the end panel
            if (endPanel != null)
            {
                endPanel.SetActive(true);
            }

            // Hide the disconnected panel
            if (stages[currentStage - 1].disconnectedPanel != null) // Ensure the previous stage exists
            {
                stages[currentStage - 1].disconnectedPanel.SetActive(false);
            }

            return;
        }

        CableConnectionStage stage = stages[stageIndex];

        if (stage.dialoguePanel != null)
        {
            stage.dialoguePanel.StartDialogue(() =>
            {
                Debug.Log("Dialogue completed. Waiting for player to start.");
            });
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

                currentStage++;
                LoadStage(currentStage);
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

        // Start the countdown timer
        StartCoroutine(StartCountdown(stage));
    }

    private IEnumerator StartCountdown(CableConnectionStage stage)
    {
        float timeRemaining = countdownTime;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }

        while (timeRemaining > 0)
        {
            timerText.text = Mathf.Ceil(timeRemaining).ToString();
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Trigger the panel transition
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
    }
}
