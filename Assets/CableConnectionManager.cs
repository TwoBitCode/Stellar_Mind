using UnityEngine;
using TMPro;

public class CableConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CableConnectionStage
    {
        public GameObject connectedPanel; // Panel showing the connected state
        public GameObject disconnectedPanel; // Panel for the disconnected state
        public DragCable[] cables; // Cables to be used in the stage
        public RectTransform[] targets; // Shapes (targets) for the stage
    }

    public CableConnectionStage[] stages; // All stages in the mini-game

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText; // Feedback text for the player
    public float feedbackDuration = 2f; // Duration to show the feedback
    public Color correctFeedbackColor = Color.green; // Color for correct feedback
    public Color stageCompletedFeedbackColor = Color.blue; // Color for stage completed feedback

    [Header("Stage Management")]
    public int currentStage = 0;

    void Start()
    {
        // Load the first stage
        LoadStage(currentStage);
    }

    public void LoadStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            ShowFeedback("Game Completed!", stageCompletedFeedbackColor);
            return;
        }

        CableConnectionStage stage = stages[stageIndex];

        // Enable targets for the stage
        foreach (RectTransform target in stage.targets)
        {
            target.gameObject.SetActive(true);
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
}
