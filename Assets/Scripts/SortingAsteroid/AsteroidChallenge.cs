using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AsteroidChallenge
{
    public string challengeName;
    public ScriptableObject sortingRule; // Reference to the sorting rule
    public float spawnDelay = 1f;
    public float spawnInterval = 2f;
    public int maxAsteroids = 20;

    [Tooltip("List of drop zone names to activate for this challenge")]
    public List<string> dropZones;

    [Tooltip("Assignments of drop zones with their types and display names")]
    public List<DropZoneAssignment> dropZoneAssignments = new List<DropZoneAssignment>();
}

