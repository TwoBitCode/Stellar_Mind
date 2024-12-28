using TMPro;
using UnityEngine;

public class SortingGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Delay before starting the game")]
    [SerializeField] private float gameStartDelay = 4f;

    [Tooltip("Interval between asteroid spawns")]
    [SerializeField] private float spawnInterval = 2f;

    [Tooltip("Initial delay before spawning asteroids")]
    [SerializeField] private float spawnInitialDelay = 1f;

    [Tooltip("Horizontal spawn range for asteroids")]
    [SerializeField] private float spawnHorizontalRange = 200f;

    [Tooltip("Vertical spawn range for asteroids")]
    [SerializeField] private float spawnVerticalRange = 0f;

    [Header("Asteroid Types")]
    [Tooltip("Type assigned to the left area")]
    [SerializeField] private const string BlueType = "blue";

    [Tooltip("Type assigned to the right area")]
    [SerializeField] private const string YellowType = "yellow";

    [Tooltip("Threshold for random type assignment")]
    [SerializeField] private float randomAssignThreshold = 0.5f;

    [Header("References")]
    [Tooltip("Asteroid prefab to spawn")]
    [SerializeField] private GameObject asteroidPrefab;

    [Tooltip("UI area where asteroids are spawned")]
    [SerializeField] private Transform spawnArea;

    [Tooltip("Text to display game instructions")]
    [SerializeField] private TextMeshProUGUI instructionsText;

    [Tooltip("Reference to the game timer")]
    [SerializeField] private GameTimer gameTimer;

    [Tooltip("Reference to the score manager")]
    [SerializeField] private ScoreManager scoreManager;

    private bool isGameActive;
    private string leftType;
    private string rightType;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        AssignAsteroidTypes();
        DisplayInstructions();

        // Delay game start
        Invoke(nameof(StartGame), gameStartDelay);
    }

    private void AssignAsteroidTypes()
    {
        // Randomly assign types to left and right areas
        if (Random.value > randomAssignThreshold)
        {
            leftType = BlueType;
            rightType = YellowType;
        }
        else
        {
            leftType = YellowType;
            rightType = BlueType;
        }
    }

    private void DisplayInstructions()
    {
        instructionsText.text = $"Drag the meteors to the correct baskets:\n" +
                                $"Left: {leftType}, Right: {rightType}";
    }

    private void StartGame()
    {
        HideInstructions();
        gameTimer.StartTimer();
        isGameActive = true;

        // Start spawning asteroids
        InvokeRepeating(nameof(SpawnAsteroid), spawnInitialDelay, spawnInterval);
    }

    private void HideInstructions()
    {
        instructionsText.gameObject.SetActive(false);
    }

    private void SpawnAsteroid()
    {
        if (!isGameActive) return;

        // Instantiate asteroid
        GameObject asteroid = Instantiate(asteroidPrefab, spawnArea);

        // Randomize position within spawn range
        asteroid.transform.localPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            0f
        );

        // Assign random type to the asteroid
        AssignAsteroidType(asteroid);
    }

    private void AssignAsteroidType(GameObject asteroid)
    {
        SortingDraggableItem draggable = asteroid.GetComponent<SortingDraggableItem>();
        if (draggable == null)
        {
            Debug.LogError("Asteroid does not have a SortingDraggableItem component!");
            return;
        }

        draggable.itemType = Random.value > randomAssignThreshold ? leftType : rightType;
    }

    public void StopGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnAsteroid));

        // Add score to the overall score manager
        if (OverallScoreManager.Instance != null && scoreManager != null)
        {
            OverallScoreManager.Instance.AddScore(scoreManager.Score); // Use the Score property
        }
    }

}
