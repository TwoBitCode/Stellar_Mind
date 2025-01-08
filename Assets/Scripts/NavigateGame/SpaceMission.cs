using UnityEngine;

[System.Serializable]
public class SpaceMission
{
    public string missionName;
    public Node startNode;
    public Node[] restrictedNodes;
    public Node targetNode; // For NavigateToTarget missions
    public Node[] trajectoryPath; // Current trajectory path during gameplay
    public Node[] originalTrajectoryPath; // Original, unmodified path
    [HideInInspector] public int currentProgressIndex; // Tracks player's progress
    public enum MissionType { ReconstructTrajectory, NavigateToTarget }
    public MissionType missionType;

    // Constructor remains simple and does not clone paths
    public SpaceMission()
    {
        currentProgressIndex = 0;
    }

    // Call this explicitly after setting trajectoryPath
    public void Initialize()
    {
        if (trajectoryPath != null)
        {
            originalTrajectoryPath = trajectoryPath.Clone() as Node[];
        }
        else
        {
            Debug.LogWarning($"Mission '{missionName}' has a null trajectoryPath during initialization.");
            originalTrajectoryPath = new Node[0]; // Fallback to an empty array
        }
    }
}
