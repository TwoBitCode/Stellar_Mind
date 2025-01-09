using System.Collections;
using System.Linq;
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

        // Show mission instructions using the instruction panel
        NavigateSpaceUIManager.Instance.ShowMissionDetails(
            mission.missionName,
            mission.missionType == SpaceMission.MissionType.ReconstructTrajectory
                ? "Follow the highlighted trajectory."
                : "Reach the target node without stepping on restricted nodes.",
            mission.trajectoryPath
        );

        // Highlight the target node for NavigateToTarget missions
        if (mission.missionType == SpaceMission.MissionType.NavigateToTarget && mission.targetNode != null)
        {
            NavigateSpaceUIManager.Instance.HighlightTargetNode(mission.targetNode);
        }

        // Set the player's starting position
        currentNode = mission.startNode ?? startNode;
        PlayerController.Instance.SetPosition(currentNode.transform.position);
    }



    private IEnumerator HideInstructionsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NavigateSpaceUIManager.Instance.HideMissionDetails();
    }



    public void OnNodeClicked(Node clickedNode)
    {
        if (NavigateSpaceUIManager.Instance.IsPathShowing())
        {
            Debug.Log("Path is still being highlighted. Wait until it's complete.");
            return;
        }

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
            mission.trajectoryPath = mission.trajectoryPath[1..];
            MoveToNode(clickedNode);
            AlienGuideManager.Instance.ProvidePositiveFeedback(); // Alien feedback for correct move

            if (mission.trajectoryPath.Length == 0)
            {
                Debug.Log("Trajectory completed!");
                NavigateSpaceUIManager.Instance.DelayNextMissionUI(() =>
                {
                    SpaceMissionManager.Instance.CompleteMission();
                });
            }
        }
        else
        {
            AlienGuideManager.Instance.ProvideNegativeFeedback(); // Alien feedback for incorrect move
            RestartStage(mission);
        }
    }



    // Restart stage logic
    private void RestartStage(SpaceMission mission)
    {
        Debug.Log("Restarting stage due to a mistake...");

        // Reset the trajectory path to the original state
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Reset player to the starting position
        var initialNode = mission.startNode ?? startNode;
        currentNode = initialNode;
        PlayerController.Instance.SetPosition(initialNode.transform.position);

        // Notify the alien to provide restart feedback
        AlienGuideManager.Instance.ProvideRestartMessage();

        // Re-highlight the trajectory path for Reconstruct Trajectory missions
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            Debug.Log("Re-highlighting trajectory for Reconstruct Trajectory mission.");
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
        else if (mission.restrictedNodes != null && mission.restrictedNodes.Contains(clickedNode))
        {
            Debug.Log($"Player clicked a restricted node: {clickedNode.name}");
            AlienGuideManager.Instance.ProvideNegativeFeedback();
            RestartStage(mission);
        }
        else
        {
            Debug.Log($"Player clicked a valid node: {clickedNode.name}");
            MoveToNode(clickedNode);
            AlienGuideManager.Instance.ProvidePositiveFeedback();
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
