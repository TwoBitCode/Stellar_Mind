using UnityEngine;

public class NavigateSpaceManager : MonoBehaviour
{
    public static NavigateSpaceManager Instance;

    [Header("Navigation Settings")]
    public Node startNode; // Starting node in the navigation graph

    private Node currentNode; // Current node the player is on

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartNavigation();
    }

    public void StartNavigation()
    {
        if (startNode == null)
        {
            Debug.LogError("Start Node is not assigned!");
            return;
        }

        // Notify the UI manager about mission details
        var currentMission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];
        NavigateSpaceUIManager.Instance.SetMissionDetails(
            currentMission.missionName,
            currentMission.missionType == SpaceMission.MissionType.ReconstructTrajectory
                ? "Follow the highlighted trajectory."
                : "Reach the target node without stepping on restricted nodes."
        );

        // Delegate trajectory highlighting to the UI manager
        if (currentMission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            NavigateSpaceUIManager.Instance.StartHighlightingPath(currentMission.trajectoryPath);
        }

        // Place the player on the starting node
        currentNode = startNode;
        PlayerController.Instance.SetPosition(startNode.transform.position);
    }

    public void OnNodeClicked(Node clickedNode)
    {
        var currentMission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];

        if (currentMission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            HandleTrajectoryReconstruction(clickedNode);
        }
        else if (currentMission.missionType == SpaceMission.MissionType.NavigateToTarget)
        {
            HandleTargetNavigation(clickedNode);
        }
    }

    private void HandleTrajectoryReconstruction(Node clickedNode)
    {
        var mission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];

        if (mission.trajectoryPath.Length > 0 && mission.trajectoryPath[0] == clickedNode)
        {
            mission.trajectoryPath = mission.trajectoryPath[1..]; // Remove the first node from the trajectory
            MoveToNode(clickedNode);
            NavigateSpaceUIManager.Instance.SetFeedback("Correct!", Color.green);

            if (mission.trajectoryPath.Length == 0)
            {
                Debug.Log("Trajectory completed!");
                SpaceMissionManager.Instance.CompleteMission();
            }
        }
        else
        {
            NavigateSpaceUIManager.Instance.SetFeedback("Incorrect! Try again.", Color.red);
        }
    }

    private void HandleTargetNavigation(Node clickedNode)
    {
        var mission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];

        if (clickedNode == mission.targetNode)
        {
            NavigateSpaceUIManager.Instance.SetFeedback("Mission Complete!", Color.green);
            SpaceMissionManager.Instance.CompleteMission();
        }
        else if (!clickedNode.isRestricted)
        {
            MoveToNode(clickedNode);
        }
        else
        {
            NavigateSpaceUIManager.Instance.SetFeedback("Cannot move to a restricted node!", Color.red);
        }
    }

    private void MoveToNode(Node newNode)
    {
        PlayerController.Instance.MoveTo(newNode.transform.position, () =>
        {
            currentNode = newNode;
        });
    }
}