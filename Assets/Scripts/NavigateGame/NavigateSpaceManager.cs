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
        // Set the correct strategy bank in AlienGuideManager
        AlienGuideManager.Instance.SetMissionType(mission.missionType);

        // Reset trajectory to the original path
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Reset path showing state
        NavigateSpaceUIManager.Instance.ResetPathState();

        // Use mission-specific instructions
        string instruction = string.IsNullOrEmpty(mission.missionInstruction)
            ? "Complete the mission by following the instructions!" // Default fallback instruction
            : mission.missionInstruction;

        // Show mission instructions
        NavigateSpaceUIManager.Instance.ShowMissionDetails(
            mission.missionName,
            instruction, // Display the specific instruction for the mission
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
        Debug.Log("Player made a mistake. Showing strategy panel...");

        // Show strategy feedback from the alien
        AlienGuideManager.Instance.ProvideNegativeFeedback();

        // Show the strategy panel with a tip
        NavigateSpaceUIManager.Instance.ShowStrategyPanel(() =>
        {
            // When "Continue" is clicked, reset the mission
            Debug.Log("Restarting the mission...");
            ResetMissionState(mission);
        });
    }

    private void ResetMissionState(SpaceMission mission)
    {
        // Reset the trajectory path to the original state
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Reset player to the starting position
        var initialNode = mission.startNode ?? startNode;
        currentNode = initialNode;
        PlayerController.Instance.SetPosition(initialNode.transform.position);

        // Update the alien text to reflect the restart
        AlienGuideManager.Instance.UpdateAlienText("Follow the highlighted trajectory to complete the mission!");

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
            Debug.Log($"Player clicked the target node: {clickedNode.name}");
            MoveToNode(clickedNode); // Move to the target node
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
            MoveToNode(clickedNode); // Move to the clicked node
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
