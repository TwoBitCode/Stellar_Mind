using UnityEngine;

public class NavigateSpaceManager : MonoBehaviour
{
    public static NavigateSpaceManager Instance;

    [Header("Navigation Settings")]
    public Node startNode; // Default starting node

    private Node currentNode;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void InitializeMission(SpaceMission mission)
    {
        // Reset trajectory to the original path for safety
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Update UI
        NavigateSpaceUIManager.Instance.SetMissionDetails(
            mission.missionName,
            mission.missionType == SpaceMission.MissionType.ReconstructTrajectory
                ? "Follow the highlighted trajectory."
                : "Reach the target node without stepping on restricted nodes."
        );

        // Highlight trajectory if applicable
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            NavigateSpaceUIManager.Instance.StartHighlightingPath(mission.trajectoryPath);
        }

        // Set player position
        currentNode = mission.startNode ?? startNode;
        PlayerController.Instance.SetPosition(currentNode.transform.position);
    }


    public void OnNodeClicked(Node clickedNode)
    {
        var mission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            HandleTrajectoryReconstruction(mission, clickedNode);
        }
        else if (mission.missionType == SpaceMission.MissionType.NavigateToTarget)
        {
            HandleTargetNavigation(mission, clickedNode);
        }
    }

    private void HandleTrajectoryReconstruction(SpaceMission mission, Node clickedNode)
    {
        if (mission.trajectoryPath.Length > 0 && mission.trajectoryPath[0] == clickedNode)
        {
            // Correct node
            mission.trajectoryPath = mission.trajectoryPath[1..]; // Remove completed node
            MoveToNode(clickedNode);
            AlienGuideManager.Instance.ProvidePositiveFeedback();

            if (mission.trajectoryPath.Length == 0)
            {
                Debug.Log("Trajectory completed!");
                SpaceMissionManager.Instance.CompleteMission();
            }
        }
        else
        {
            // Incorrect node
            AlienGuideManager.Instance.ProvideNegativeFeedback();
            RestartStage(mission);
        }
    }


    // Restart stage logic
    private void RestartStage(SpaceMission mission)
    {
        Debug.Log("Restarting stage due to mistake...");

        // Reset the trajectory path to the original state
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Reset player to the starting position
        var initialNode = mission.startNode ?? startNode;
        currentNode = initialNode;
        PlayerController.Instance.SetPosition(initialNode.transform.position);

        // Re-highlight the full path
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            NavigateSpaceUIManager.Instance.StartHighlightingPath(mission.trajectoryPath);
        }
    }



    private void HandleTargetNavigation(SpaceMission mission, Node clickedNode)
    {
        if (clickedNode == mission.targetNode)
        {
            AlienGuideManager.Instance.ProvidePositiveFeedback();
            SpaceMissionManager.Instance.CompleteMission();
        }
        else if (!clickedNode.isRestricted)
        {
            MoveToNode(clickedNode);
            AlienGuideManager.Instance.ProvidePositiveFeedback(); // Optional: Positive for valid moves
        }
        else
        {
            AlienGuideManager.Instance.ProvideNegativeFeedback();
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
