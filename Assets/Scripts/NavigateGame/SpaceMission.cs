using UnityEngine;

[System.Serializable]
public class SpaceMission
{
    public string missionName;
    public Node startNode;
    public Node[] restrictedNodes;
    public Node targetNode; // For NavigateToTarget missions
    public Node[] trajectoryPath; // Current trajectory path during gameplay
    [HideInInspector] public Node[] originalTrajectoryPath; // Original path for resets
    [HideInInspector] public int currentProgressIndex; // Tracks player's progress
    public enum MissionType { ReconstructTrajectory, NavigateToTarget }
    public MissionType missionType;
}
