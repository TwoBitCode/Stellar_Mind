using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpaceMission
{
    public string missionName;
    public Node startNode;
    public List<Node> restrictedNodes; // Changed to List<Node>
    public Node targetNode; // For NavigateToTarget missions
    public Node[] trajectoryPath; // Current trajectory path during gameplay
    public Node[] originalTrajectoryPath; // Original, unmodified path
    [HideInInspector] public int currentProgressIndex; // Tracks player's progress
    public string missionInstruction; // Add this field for stage-specific instructions
    public enum MissionType { ReconstructTrajectory, NavigateToTarget }
    public MissionType missionType;

    public SpaceMission()
    {
        currentProgressIndex = 0;
        restrictedNodes = new List<Node>(); // Initialize empty list to avoid null issues
    }

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
