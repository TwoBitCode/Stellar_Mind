using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Constants for the asteroid spawner (Now serialized for use in the Unity Inspector)
    [Tooltip("Horizontal spawn range for asteroids")]
    [SerializeField] private float spawnHorizontalRange = 7f;  // Horizontal spawn range for asteroids

    [Tooltip("Vertical spawn position for asteroids")]
    [SerializeField] private float spawnVerticalPosition = 6f; // Vertical spawn position for asteroids

    [Tooltip("Z position for spawning (used for all spawnings)")]
    [SerializeField] private float spawnZPosition = 0f;  // Z position for spawning (used for all spawnings)

    [Tooltip("Time between spawns")]
    [SerializeField] private float spawnInterval = 2f; // Time between spawns

    [Tooltip("Initial value for the timer")]
    [SerializeField] private float initialTimer = 0f;  // Initial value for the timer

    private const int STARTING_RANGE = 0;  // Start range for random selection of asteroid prefab

    // References to asteroid prefabs
    public GameObject[] asteroidPrefabs; // Array for two types of asteroids

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAsteroid();
            timer = initialTimer;
        }
    }

    void SpawnAsteroid()
    {
        // Randomly pick an asteroid prefab
        int index = Random.Range(STARTING_RANGE, asteroidPrefabs.Length);

        // Define the spawn position using serialized fields
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnHorizontalRange, spawnHorizontalRange), spawnVerticalPosition, spawnZPosition);

        // Instantiate the asteroid prefab at the spawn position
        Instantiate(asteroidPrefabs[index], spawnPosition, Quaternion.identity);
    }
}
