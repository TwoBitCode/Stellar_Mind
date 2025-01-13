using UnityEngine;
using TMPro; // Import TextMesh Pro namespace

public class CableConnectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CableConnectionStage
    {
        public string instruction; // Instruction text for the stage
        public GameObject connectedPanel; // Panel showing the connected state
        public GameObject disconnectedPanel; // Panel for the disconnected state
        public DragCable[] cables; // Cables to be used in the stage
        public RectTransform[] targets; // Shapes (targets) for the stage
    }

    public CableConnectionStage[] stages; // All stages in the mini-game
    public TextMeshProUGUI instructionText; // TextMesh Pro element to display the instructions
    public int currentStage = 0;

    void Start()
    {
        // Load the first stage
        LoadStage(currentStage);
    }

    public void LoadStage(int stageIndex)
    {
        // Check if the stage index is valid
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            instructionText.text = "Game Completed!"; // Display completion message
            return;
        }

        // Get the current stage
        CableConnectionStage stage = stages[stageIndex];

        // Show the connected panel
        ShowConnectedPanel(stage);

        // Set up the cables and targets for this stage
        foreach (DragCable cable in stage.cables)
        {
            cable.gameObject.SetActive(false); // Hide cables initially
        }

        foreach (RectTransform target in stage.targets)
        {
            target.gameObject.SetActive(true); // Ensure targets are active
        }

        // Update the instruction text
        instructionText.text = stage.instruction;
    }

    private void ShowConnectedPanel(CableConnectionStage stage)
    {
        // Enable the connected panel
        stage.connectedPanel.SetActive(true);
        stage.disconnectedPanel.SetActive(false);

        // After a delay, switch to the disconnected panel
        Invoke(nameof(SwitchToDisconnectedPanel), 10f); // Show for 2 seconds
    }

    private void SwitchToDisconnectedPanel()
    {
        // Get the current stage
        CableConnectionStage stage = stages[currentStage];

        // Trigger disconnect animation for each cable
        foreach (DragCable cable in stage.cables)
        {
            LineRendererAnimator animator = cable.GetComponent<LineRendererAnimator>();
            if (animator != null)
            {
                // Retract the cable back to its start point
                animator.StartDisconnectAnimation(animator.startPoint.position);
            }
        }

        // Delay switching to the disconnected panel until the animation completes
        Invoke(nameof(ActivateDisconnectedPanel), 1f); // Match animation duration
    }

    private void ActivateDisconnectedPanel()
    {
        // Enable the disconnected panel after the disconnect animation
        CableConnectionStage stage = stages[currentStage];
        stage.connectedPanel.SetActive(false);
        stage.disconnectedPanel.SetActive(true);

        // Activate cables for the disconnected state
        foreach (DragCable cable in stage.cables)
        {
            cable.gameObject.SetActive(true);
        }
    }


    public void OnCableConnected(DragCable cable, RectTransform target)
    {
        // Get the current stage
        CableConnectionStage stage = stages[currentStage];

        // Check if the cable and target belong to this stage
        if (System.Array.Exists(stage.cables, c => c == cable) &&
            System.Array.Exists(stage.targets, t => t == target))
        {
            // Check if the cable is connected to the correct target
            CableTarget targetScript = target.GetComponent<CableTarget>();
            if (targetScript != null && targetScript.targetID == cable.name)
            {
                Debug.Log($"Correct connection: {cable.name} -> {targetScript.targetID}");

                // Check if all connections are complete
                if (AllCablesConnected(stage))
                {
                    Debug.Log("Stage completed!");
                    currentStage++;
                    LoadStage(currentStage); // Load the next stage
                }
            }
            else
            {
                Debug.Log("Wrong connection!");
            }
        }
    }

    private bool AllCablesConnected(CableConnectionStage stage)
    {
        foreach (DragCable cable in stage.cables)
        {
            // Get the target the cable is connected to
            RectTransform connectedTarget = GetConnectedTarget(cable);

            // Check if the target is valid and matches the cable's name
            if (connectedTarget == null ||
                connectedTarget.GetComponent<CableTarget>().targetID != cable.name)
            {
                return false; // Not all cables are connected correctly
            }
        }

        return true; // All cables are correctly connected
    }

    private RectTransform GetConnectedTarget(DragCable cable)
    {
        // Check if the cable overlaps any target
        foreach (RectTransform target in stages[currentStage].targets)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(target, cable.transform.position, null))
            {
                return target; // The cable is connected to this target
            }
        }

        return null; // No target is connected
    }
}
