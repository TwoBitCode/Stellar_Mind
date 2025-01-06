using TMPro;
using UnityEngine;

public class SortingGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameStartDelay = 4f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnInitialDelay = 1f;

    [Header("Asteroid Settings")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float spawnHorizontalRange = 200f;
    [SerializeField] private float spawnVerticalRange = 0f;
    [SerializeField] private float randomAssignThreshold = 0.5f;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI instructionsText;

    [Header("Game Components")]
    //[SerializeField] private GameFlowManager gameFlowManager; // Manages scene transitions and game flow
    [SerializeField] private ScoreManager scoreManager; // Tracks and updates score
    [SerializeField] private GameTimer gameTimer; // Manages the timer for the game

    private const string BlueType = "blue";
    private const string YellowType = "yellow";
    private bool isGameActive;
    public string LeftType { get; private set; }
    public string RightType { get; private set; }

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

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnHorizontalRange, spawnHorizontalRange),
            Random.Range(-spawnVerticalRange, spawnVerticalRange),
            0);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnArea);
        asteroid.transform.localPosition = spawnPosition;

        AssignAsteroidType(asteroid);
    }

    private void AssignAsteroidType(GameObject asteroid)
    {
        if (asteroid.TryGetComponent(out SortingDraggableItem draggable))
        {
            draggable.itemType = Random.value > randomAssignThreshold ? LeftType : RightType;
        }
    }

    public void StopGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnAsteroid));
        //HandleSceneTransition();
    }

 
}
