using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SortingDraggableItem : DraggableItem
{
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

    private RectTransform leftArea;
    private RectTransform rightArea;

    [SerializeField] public string itemType; // Type of this asteroid ("blue" or "yellow")
    private Image asteroidImage;
    private ScoreManager scoreManager;
    private SortingGameManager gameManager;

    [SerializeField] private GameObject correctIndicatorPrefab;
    [SerializeField] private GameObject incorrectIndicatorPrefab;

    private Color originalColor;

    private void Start()
    {
        // Initialize components and managers
        asteroidImage = GetComponent<Image>();
        originalColor = asteroidImage.color;

        leftArea = GameObject.Find(leftAreaName).GetComponent<RectTransform>();
        rightArea = GameObject.Find(rightAreaName).GetComponent<RectTransform>();

        scoreManager = GameObject.Find(scoreManagerName).GetComponent<ScoreManager>();
        gameManager = GameObject.Find(gameManagerName).GetComponent<SortingGameManager>();

        if (gameManager == null)
        {
            Debug.LogError("SortingGameManager not found in the scene!");
        }

        SetAsteroidColor();
        SetInFrontOfTargetAreas();
    }

    private void SetInFrontOfTargetAreas()
    {
        transform.SetAsLastSibling(); // Ensure the item appears above other elements
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData); // Retain base drag behavior

        // Validate drop
        if (gameManager.LeftType == itemType && IsOverArea(leftArea))
        {
            Debug.Log($"Correct! Dropped {itemType} in the left area.");
            HandleCorrectPlacement();
        }
        else if (gameManager.RightType == itemType && IsOverArea(rightArea))
        {
            Debug.Log($"Correct! Dropped {itemType} in the right area.");
            HandleCorrectPlacement();
        }
        else
        {
            Debug.Log($"Incorrect placement of {itemType}. Returning to original position.");
            HandleIncorrectPlacement();
        }
    }
    private bool IsOverArea(RectTransform area)
    {
        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition, Camera.main);
        Debug.Log($"Checking area: {area.name}, Mouse Position: {Input.mousePosition}, Is Over: {isOver}");
        return isOver;
    }


    private void HandleCorrectPlacement()
    {
        scoreManager.AddScore(pointsForCorrectDrop);

        if (correctIndicatorPrefab != null)
        {
            GameObject correctIndicator = Instantiate(correctIndicatorPrefab, transform.parent);
            correctIndicator.transform.localPosition = correctIndicatorPrefab.transform.localPosition;
            Destroy(correctIndicator, destroyDelay);
        }

        Destroy(gameObject); // Remove asteroid after correct placement
    }

    private void HandleIncorrectPlacement()
    {
        if (incorrectIndicatorPrefab != null)
        {
            GameObject incorrectIndicator = Instantiate(incorrectIndicatorPrefab, transform.parent);
            incorrectIndicator.transform.localPosition = incorrectIndicatorPrefab.transform.localPosition;
            Destroy(incorrectIndicator, destroyDelay);
        }

        asteroidImage.color = Color.red; // Indicate incorrect placement
        Destroy(gameObject, destroyDelay); // Destroy asteroid after delay
    }

    private void SetAsteroidColor()
    {
        if (asteroidImage != null)
        {
            asteroidImage.color = itemType == "blue" ? Color.blue : itemType == "yellow" ? Color.yellow : originalColor;
        }
    }
}
