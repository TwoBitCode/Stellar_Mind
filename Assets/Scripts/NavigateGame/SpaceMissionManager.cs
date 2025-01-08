using System.Collections.Generic;
using UnityEngine;

public class SpaceMissionManager : MonoBehaviour
{
    public static SpaceMissionManager Instance; // Singleton instance

    [Header("Mission Settings")]
    public List<SpaceMission> missions = new List<SpaceMission>();
    private int currentMissionIndex = 0;
    public int CurrentMissionIndex => currentMissionIndex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartMission();
    }

    public void StartMission()
    {
        if (currentMissionIndex >= missions.Count)
        {
            Debug.Log("All missions completed!");
            return;
        }

        var mission = missions[currentMissionIndex];
        Debug.Log($"Starting Mission: {mission.missionName}");

        // Reset all node states
        NodeManager.Instance.ResetAllNodeStates();

        // Set restricted nodes if applicable
        if (mission.missionType == SpaceMission.MissionType.NavigateToTarget)
        {
            NodeManager.Instance.SetRestrictedNodes(mission.restrictedNodes);
        }

        // Set player position to the starting node
        PlayerController.Instance.SetPosition(mission.startNode.transform.position);

        // Notify AlienGuideManager about the mission type
        AlienGuideManager.Instance.SetMissionType(mission.missionType);

        // Start highlighting for ReconstructTrajectory missions
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            NavigateSpaceUIManager.Instance.StartHighlightingPath(mission.trajectoryPath);
        }
    }


    public void HandleMistake()
    {
        // Reset the current mission
        StartMission();

        // Provide a strategy to the player
        AlienGuideManager.Instance.ProvideStrategy();
    }

    public void CompleteMission()
    {
        AlienGuideManager.Instance.ProvidePositiveFeedback();
        currentMissionIndex++;
        StartMission();
    }

    private void ResetNodeStates()
    {
        // Notify NodeManager or directly iterate nodes if necessary
        NodeManager.Instance.ResetAllNodeStates();
    }

    private void SetRestrictedNodes(Node[] restrictedNodes)
    {
        foreach (var node in restrictedNodes)
        {
            node.isRestricted = true;
        }
    }
    public void RestartMission()
    {
        var mission = missions[currentMissionIndex];
        Debug.Log($"Restarting Mission: {mission.missionName}");

        // Reset all node states
        NodeManager.Instance.ResetAllNodeStates();

        // Reset restricted nodes if applicable
        if (mission.missionType == SpaceMission.MissionType.NavigateToTarget)
        {
            NodeManager.Instance.SetRestrictedNodes(mission.restrictedNodes);
        }

        // Reset player position to the starting node
        PlayerController.Instance.SetPosition(mission.startNode.transform.position);

        // Set mission type for AlienGuideManager
        AlienGuideManager.Instance.SetMissionType(mission.missionType);

        // Restart highlights for ReconstructTrajectory missions
        if (mission.missionType == SpaceMission.MissionType.ReconstructTrajectory)
        {
            NavigateSpaceUIManager.Instance.StartHighlightingPath(mission.trajectoryPath);
        }
    }


}