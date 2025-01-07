using UnityEngine;

public class SortingGameLogic : MonoBehaviour, IGameLogic
{
    [Header("Game Settings")]
    [SerializeField] private GameObject asteroidPrefab; // Prefab for the asteroid
    [SerializeField] private Transform spawnArea; // Reference to the spawn area
    [SerializeField] private float spawnHorizontalRange = 200f; // Horizontal range for spawning

    [Header("Spawn Position Defaults")]
    [SerializeField] private float spawnYPosition = 0f; // Default Y position for spawned objects
    [SerializeField] private float spawnZPosition = 0f; // Default Z position for spawned objects

    private bool isGameActive;

    // Initializes the game and starts spawning asteroids
    public void InitializeGame()
    {
        isGameActive = true;
        SpawnAsteroids();
    }

    // Ends the game and stops spawning asteroids
    public void EndGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnAsteroids));
    }

    // Spawns an asteroid at a random position within the horizontal range
    private void SpawnAsteroids()
    {
        if (!isGameActive) return;

        // Generate a random spawn position within the horizontal range
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            spawnYPosition,
            spawnZPosition
        );

        // Instantiate the asteroid at the calculated position
        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    }
}
