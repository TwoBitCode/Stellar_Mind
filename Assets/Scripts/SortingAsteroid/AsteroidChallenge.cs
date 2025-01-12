using UnityEngine;

[System.Serializable]
public class AsteroidChallenge
{
    public string challengeName;
    public ScriptableObject sortingRule; // Reference to the sorting rule
    public float spawnDelay = 1f;       // Delay before starting to spawn asteroids
    public float spawnInterval = 2f;   // Time interval between spawns
    public int maxAsteroids = 20;      // Maximum number of asteroids for the challenge
}
