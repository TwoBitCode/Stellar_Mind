using UnityEngine;

public class SortingGameLogic : MonoBehaviour, IGameLogic
{
    [Header("Game Settings")]
    //[SerializeField] private float spawnInterval = 2f;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float spawnHorizontalRange = 200f;

    private bool isGameActive;

    public void InitializeGame()
    {
        isGameActive = true;
        SpawnAsteroids();
    }

    public void EndGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnAsteroids));
    }

    private void SpawnAsteroids()
    {
        if (!isGameActive) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            0,
            0
        );

        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    }
}
