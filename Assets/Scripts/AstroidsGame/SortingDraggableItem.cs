using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SortingDraggableItem : DraggableItem
{
    // Serialized fields for game settings and UI elements
    [Tooltip("Points awarded for a correct drop")]
    [SerializeField] private int pointsForCorrectDrop = 10;

    [Tooltip("Delay before destroying the indicator")]
    [SerializeField] private float destroyDelay = 0.2f;

    [Tooltip("Name of the left area for dropping items")]
    [SerializeField] private string leftAreaName = "LeftArea";

    [Tooltip("Name of the right area for dropping items")]
    [SerializeField] private string rightAreaName = "RightArea";

    [Tooltip("Name of the score manager")]
    [SerializeField] private string scoreManagerName = "ScoreManager";

    [Tooltip("Name of the game manager")]
    [SerializeField] private string gameManagerName = "GameManager";

    // Constants for item types (these don't need to be serialized)
    [SerializeField] private const string BLUE_ITEM_TYPE = "blue";
    [SerializeField] private const string YELLOW_ITEM_TYPE = "yellow";

    // References to UI and game elements
    private RectTransform leftArea;  // Reference to the left area
    private RectTransform rightArea; // Reference to the right area
    [SerializeField] public string itemType; // Define the type of asteroid, e.g., "blue" or "yellow"

    private Image asteroidImage; // Reference to the Image component
    private ScoreManager scoreManager; // Reference to the ScoreManager
    private SortingGameManager gameManager; // Reference to the SortingGameManager

    [SerializeField] private GameObject correctIndicatorPrefab;  // Reference to the correct indicator prefab
    [SerializeField] private GameObject incorrectIndicatorPrefab;  // Reference to the incorrect indicator prefab
    private Color originalColor;  // To store original color for incorrect feedback

    private void Start()
    {
        // Get the Image component on the asteroid
        asteroidImage = GetComponent<Image>();
        originalColor = asteroidImage.color; // Store the original color for incorrect feedback

        // Assign areas dynamically
        leftArea = GameObject.Find(leftAreaName).GetComponent<RectTransform>();
        rightArea = GameObject.Find(rightAreaName).GetComponent<RectTransform>();

        // Get references to managers
        scoreManager = GameObject.Find(scoreManagerName).GetComponent<ScoreManager>();
        gameManager = GameObject.Find(gameManagerName).GetComponent<SortingGameManager>();

        if (gameManager == null)
        {
            Debug.LogError("SortingGameManager not found in the scene!");
        }

        // Set asteroid color based on the type
        SetAsteroidColor();

        // Ensure the asteroid appears in front of the target areas
        SetInFrontOfTargetAreas();
    }

    private void SetInFrontOfTargetAreas()
    {
        transform.SetAsLastSibling(); // Ensure asteroid is in front of target areas
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData); // Keep the base drag behavior

        // Check if dropped in the correct area
        if (gameManager.leftType == itemType && IsOverArea(leftArea))
        {
            Debug.Log("Correct! Dropped in left area.");
            HandleCorrectPlacement();
        }
        else if (gameManager.rightType == itemType && IsOverArea(rightArea))
        {
            Debug.Log("Correct! Dropped in right area.");
            HandleCorrectPlacement();
        }
        else
        {
            Debug.Log("Wrong placement. Returning to original position.");
            HandleIncorrectPlacement();
        }
    }

    private bool IsOverArea(RectTransform area)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition);
    }

    private void HandleCorrectPlacement()
    {
        // Add points for a correct placement
        scoreManager.AddScore(pointsForCorrectDrop);

        // Instantiate the correct placement indicator
        if (correctIndicatorPrefab != null)
        {
            GameObject correctIndicator = Instantiate(correctIndicatorPrefab, transform.parent);
            correctIndicator.transform.localPosition = correctIndicatorPrefab.transform.localPosition; // Set to prefab's position
            Destroy(correctIndicator, destroyDelay); // Destroy after a delay
        }

        // Optionally, destroy the asteroid
        Destroy(gameObject);
    }

    private void SetAsteroidColor()
    {
        if (asteroidImage != null)
        {
            if (itemType == BLUE_ITEM_TYPE)
            {
                asteroidImage.color = Color.blue;  // Set blue for "blue" type
            }
            else if (itemType == YELLOW_ITEM_TYPE)
            {
                asteroidImage.color = Color.yellow;  // Set yellow for "yellow" type
            }
        }
    }

    private void HandleIncorrectPlacement()
    {
        // Instantiate the incorrect placement indicator
        if (incorrectIndicatorPrefab != null)
        {
            GameObject incorrectIndicator = Instantiate(incorrectIndicatorPrefab, transform.parent);
            incorrectIndicator.transform.localPosition = incorrectIndicatorPrefab.transform.localPosition; // Set to prefab's position
            Destroy(incorrectIndicator, destroyDelay); // Destroy after a delay
        }

        // Set the asteroid color to red as feedback for incorrect placement
        asteroidImage.color = Color.red;

        // Destroy the asteroid after a short delay to give feedback
        Destroy(gameObject, destroyDelay);
    }
}
