using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MixedSortingCondition
{
    public string dropZoneName; // The target drop zone (e.g., "LeftArea", "RightArea")
    public string size;         // Size condition (e.g., "Small", "Large")
    public Color color;         // Color condition (e.g., Red, Green)
    public string displayName;  // Human-readable name for the color/type
}

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
    [Tooltip("List of distractor asteroid prefabs for this challenge")]
    public List<GameObject> distractorPrefabs; // Add distractors specific to each challenge
    [Tooltip("List of mixed sorting conditions for this challenge")]
    public List<MixedSortingCondition> mixedConditions; // Define mixed conditions
}

