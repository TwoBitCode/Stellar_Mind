using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SortingGameManager : MonoBehaviour
{
    #region Constants for Game Settings

    [Tooltip("Delay before starting the game")]
    [SerializeField] private float gameStartDelay = 4f;

    [Tooltip("Interval between asteroid spawns")]
    [SerializeField] private float spawnInterval = 2f;

    [Tooltip("Initial delay before spawning asteroids")]
    [SerializeField] private float spawnInitialDelay = 1f;

    [Tooltip("Horizontal spawn range for asteroids")]
    [SerializeField] private float spawnHorizontalRange = 200;

    [Tooltip("Vertical spawn range for asteroids")]
    [SerializeField] private float spawnVerticalRange = 0;

    #endregion

    #region Constants for Asteroid Types

    [SerializeField] private const string BlueType = "blue";
    [SerializeField] private const string YellowType = "yellow";

    #endregion

    #region Constants for Random Assignment

    [Tooltip("Threshold for random assignment")]
    [SerializeField] private float randomAssignThreshold = 0.5f;

    #endregion

    #region References to UI Elements

    [SerializeField] private GameObject asteroidPrefab; // The asteroid prefab to spawn
    [SerializeField] private Transform spawnArea; // The UI area where asteroids are spawned
    [SerializeField] private TextMeshProUGUI instructionsText; // The UI text displaying game instructions

    #endregion

    #region Game State Variables

    private bool isGameActive = true; // Flag to control asteroid spawning

    [HideInInspector] public string leftType = ""; // Type assigned to the left area
    [HideInInspector] public string rightType = ""; // Type assigned to the right area

    [SerializeField] private GameTimer gameTimer; // Reference to the GameTimer script


    [SerializeField] private ScoreManager scoreManager;  // Reference to the UI Text element to display the score

    #endregion

    private void Start()
    {
        // Randomly assign types to left and right
        AssignTypes();

        // Display the instructions
        DisplayInstructions();

        // Start the game after a short delay
        Invoke(nameof(StartGame), gameStartDelay);
    }

    private void AssignTypes()
    {
        // Randomly assign type1 and type2 to left and right using the threshold constant
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
        // Set up the instructions text
        instructionsText.text = $"Instructions:\nDrag the meteors to the correct baskets by: Right is {rightType} and Left is {leftType}";
    }

    private void StartGame()
    {
        // Hide instructions when the game begins
        HideInstructions();

        // Start the game timer
        gameTimer.StartTimer();

        // Start spawning asteroids
        isGameActive = true;
        InvokeRepeating(nameof(SpawnAsteroid), spawnInitialDelay, spawnInterval);
    }

    private void SpawnAsteroid()
    {
        if (!isGameActive) return; // Stop spawning if the game is over

        // Instantiate the asteroid at a random position
        GameObject asteroid = Instantiate(asteroidPrefab, spawnArea);
        asteroid.transform.localPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            0); // Random horizontal position

        // Get the SortingDraggableItem component to set itemType
        SortingDraggableItem draggable = asteroid.GetComponent<SortingDraggableItem>();

        // Assign a random type (leftType or rightType) to the asteroid
        draggable.itemType = Random.value > randomAssignThreshold ? leftType : rightType;
    }

    public void StopGame()
    {
        isGameActive = false; // Stop asteroid spawning
        CancelInvoke(nameof(SpawnAsteroid)); // Cancel further asteroid spawns
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(scoreManager.score);

        }
    }

    private void HideInstructions()
    {
        instructionsText.gameObject.SetActive(false); // Hide instructions once the game starts
    }
}
