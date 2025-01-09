using System.Collections.Generic;
using UnityEngine;

public class SpaceMissionManager : MonoBehaviour
{
    public static SpaceMissionManager Instance;

    [Header("Mission Settings")]
    public List<SpaceMission> missions = new List<SpaceMission>();
    private int currentMissionIndex = 0;
    public int CurrentMissionIndex => currentMissionIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        if (missions == null || missions.Count == 0)
        {
            Debug.LogError("No missions found! Please assign missions in the inspector.");
            return;
        }

        StartMission();
    }

    public void StartMission()
    {
        if (currentMissionIndex >= missions.Count)
        {
            Debug.Log("All missions completed!");
            AlienGuideManager.Instance?.UpdateAlienText("Congratulations! All missions are complete.");
            return;
        }

        var mission = missions[currentMissionIndex];

        if (mission == null)
        {
            Debug.LogError($"Mission at index {currentMissionIndex} is null! Skipping to the next mission.");
            currentMissionIndex++;
            StartMission();
            return;
        }

        Debug.Log($"Starting Mission: {mission.missionName}");
        Debug.Log("Restricted Nodes:");
        foreach (var node in mission.restrictedNodes)
        {
            Debug.Log(node.name);
        }

        mission.Initialize();

        NodeManager.Instance?.ResetAllNodeStates();
        AlienGuideManager.Instance?.DisplayMissionIntro(mission.missionName);

        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];
        NavigateSpaceManager.Instance?.InitializeMission(mission);
    }


    public void CompleteMission()
    {
        if (currentMissionIndex >= missions.Count)
        {
            Debug.LogWarning("Attempted to complete a mission, but all missions are already completed.");
            return;
        }

        AlienGuideManager.Instance?.ProvidePositiveFeedback();

        // Add 10 points to the overall score
        OverallScoreManager.Instance?.AddScoreFromStage(missions[currentMissionIndex].missionName, 10);

        currentMissionIndex++;

        // Reset node visuals (target highlight) before starting the next mission
        NodeManager.Instance?.ResetAllNodeStates();

        StartMission();
    }



    public void RestartMission()
    {
        if (currentMissionIndex >= missions.Count)
        {
            Debug.LogWarning("Attempted to restart a mission, but no mission is active.");
            return;
        }

        var mission = missions[currentMissionIndex];
        Debug.Log($"Restarting Mission: {mission.missionName}");
        AlienGuideManager.Instance?.ProvideRestartMessage();
        StartMission(); // Restart the current mission
    }
}
