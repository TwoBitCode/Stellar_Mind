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
            AlienGuideManager.Instance?.DisplayMissionIntro("Congratulations! All missions are complete.");
            return;
        }

        var mission = missions[currentMissionIndex];

        if (mission == null)
        {
            Debug.LogError($"Mission at index {currentMissionIndex} is null! Skipping to the next mission.");
            currentMissionIndex++;
            StartMission(); // Start the next mission
            return;
        }

        // Initialize the mission
        mission.Initialize();

        Debug.Log($"Starting Mission: {mission.missionName}");

        // Reset mission state
        NodeManager.Instance?.ResetAllNodeStates();
        AlienGuideManager.Instance?.DisplayMissionIntro(mission.missionName);

        // Ensure trajectory path is reset to the original
        mission.trajectoryPath = mission.originalTrajectoryPath.Clone() as Node[];

        // Initialize navigation
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
        currentMissionIndex++;
        StartMission(); // Start the next mission
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
