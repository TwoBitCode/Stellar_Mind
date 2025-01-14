using UnityEngine;
using TMPro;

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
    public PanelTransitionHandler panelTransitionHandler; // Handles panel transitions
    public SparkEffectHandler sparkEffectHandler; // Handles spark effects

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
            instructionText.text = "Game Completed!";
            return;
        }

        CableConnectionStage stage = stages[stageIndex];
        instructionText.text = stage.instruction;

        // Transition to the connected panel state
        panelTransitionHandler.ShowConnectedPanel(stage.connectedPanel, stage.disconnectedPanel, sparkEffectHandler, () =>
        {
            // After transition, enable the cables and targets
            //foreach (DragCable cable in stage.cables) cable.gameObject.SetActive(false);
            foreach (RectTransform target in stage.targets) target.gameObject.SetActive(true);
        });
    }



    public void OnCableConnected(DragCable cable, RectTransform target)
    {
        CableConnectionStage stage = stages[currentStage];

        if (AllCablesConnected(stage))
        {
            Debug.Log("Stage completed!");
            currentStage++;
            LoadStage(currentStage);
        }
    }

    private bool AllCablesConnected(CableConnectionStage stage)
    {
        foreach (DragCable cable in stage.cables)
        {
            RectTransform connectedTarget = GetConnectedTarget(cable);
            if (connectedTarget == null)
            {
                Debug.Log($"Cable {cable.name} is not connected to any target.");
                return false;
            }

            // Validate if the connected target is the correct one
            CableTarget targetScript = connectedTarget.GetComponent<CableTarget>();
            if (targetScript == null || targetScript.targetID != cable.name)
            {
                Debug.Log($"Cable {cable.name} is connected to the wrong target: {connectedTarget.name}");
                return false;
            }
        }

        Debug.Log("All cables are correctly connected!");
        return true;
    }



    private RectTransform GetConnectedTarget(DragCable cable)
    {
        foreach (RectTransform target in stages[currentStage].targets)
        {
            // Check if the cable overlaps the target
            if (RectTransformUtility.RectangleContainsScreenPoint(target, cable.transform.position, null))
            {
                CableTarget targetScript = target.GetComponent<CableTarget>();
                if (targetScript != null && targetScript.targetID == cable.name)
                {
                    return target; // Return the correct target
                }
            }
        }

        return null; // No valid target connected
    }

}
