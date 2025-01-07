using TMPro;
using UnityEngine;

public class SortingGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameStartDelay = 4f; // Delay before the game starts
    [SerializeField] private float spawnInterval = 2f; // Interval between spawning asteroids
    [SerializeField] private float spawnInitialDelay = 1f; // Initial delay before spawning starts

    [Header("Asteroid Settings")]
    [SerializeField] private GameObject asteroidPrefab; // Prefab for the asteroid objects
    [SerializeField] private Transform spawnArea; // Parent transform for spawned asteroids
    [SerializeField] private float spawnHorizontalRange = 200f; // Horizontal range for asteroid spawning
    [SerializeField] private float spawnVerticalRange = 0f; // Vertical range for asteroid spawning
    [SerializeField] private float randomAssignThreshold = 0.5f; // Threshold for random type assignment

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionsText; // Instructions text on the UI

    [Header("Game Components")]
    [SerializeField] private GameFlowManager gameFlowManager; // Manages scene transitions and game flow
    [SerializeField] private ScoreManager scoreManager; // Tracks and updates the game score
    [SerializeField] private GameTimer gameTimer; // Manages the game timer

    private const string BlueType = "blue"; // Represents the blue type
    private const string YellowType = "yellow"; // Represents the yellow type
    private const float DefaultZPosition = 0f; // Default Z-axis position for spawning

    private bool isGameActive;
    public string LeftType { get; private set; } // Type assigned to the left basket
    public string RightType { get; private set; } // Type assigned to the right basket

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        AssignRandomTypes();
        DisplayInstructions();
        Invoke(nameof(StartGame), gameStartDelay);
    }

    private void AssignRandomTypes()
    {
        if (Random.value > randomAssignThreshold)
        {
            LeftType = BlueType;
            RightType = YellowType;
        }
        else
        {
            LeftType = YellowType;
            RightType = BlueType;
        }
    }

    private void DisplayInstructions()
    {
        instructionsText.text = $"Instructions:\nDrag the meteors to the correct baskets: Right is {RightType} and Left is {LeftType}";
    }

    private void StartGame()
    {
        instructionsText.gameObject.SetActive(false);
        gameTimer.StartTimer();
        isGameActive = true;
        InvokeRepeating(nameof(SpawnAsteroid), spawnInitialDelay, spawnInterval);
    }

    private void SpawnAsteroid()
    {
        if (!isGameActive) return;

        // Generate a random spawn position within the defined horizontal and vertical ranges
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            DefaultZPosition // Use a constant for clarity
        );

        // Instantiate the asteroid and set its local position
        GameObject asteroid = Instantiate(asteroidPrefab, spawnArea);
        asteroid.transform.localPosition = spawnPosition;

        // Assign a type to the asteroid
        AssignAsteroidType(asteroid);
    }

    private void AssignAsteroidType(GameObject asteroid)
    {
        // Check if the asteroid has a SortingDraggableItem component
        if (asteroid.TryGetComponent(out SortingDraggableItem draggable))
        {
            // Randomly assign the type based on the threshold
            draggable.itemType = Random.value > randomAssignThreshold ? LeftType : RightType;
        }
    }

    public void StopGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnAsteroid));
        UpdateOverallScore();
        HandleSceneTransition();
    }

    private void UpdateOverallScore()
    {
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(scoreManager.CurrentScore);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }
    }

    private void HandleSceneTransition()
    {
        gameFlowManager.HandleSceneTransition();
    }
}
