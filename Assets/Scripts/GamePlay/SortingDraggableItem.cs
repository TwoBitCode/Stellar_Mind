using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SortingDraggableItem : DraggableItem
{
    public string AssignedType { get; set; } // Dynamically assigned type

    [SerializeField] private int pointsForCorrectDrop = 10;
    [SerializeField] private float destroyDelay = 0.2f;

    [SerializeField] private GameObject correctIndicatorPrefab;
    [SerializeField] private GameObject incorrectIndicatorPrefab;


    private AsteroidGameManager gameManager;
    private ScoreManager scoreManager;

    private void Start()
    {
        // Find the AsteroidGameManager in the scene
        gameManager = Object.FindFirstObjectByType<AsteroidGameManager>();

        if (gameManager == null)
        {
            Debug.LogError("AsteroidGameManager not found in the scene!");
            return;
        }

        // Find the ScoreManager in the scene
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();

        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not found in the scene!");
        }

        // Apply visuals based on the assigned type and rule
        var currentChallenge = gameManager?.ChallengeManager?.CurrentChallenge;
        var rule = currentChallenge?.sortingRule as ISortingRule;

        if (rule != null)
        {
            rule.ApplyVisuals(gameObject);
            Debug.Log($"Asteroid visuals applied for type: {AssignedType}");
        }
        else
        {
            Debug.LogError("Sorting rule not found or invalid!");
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        if (gameManager == null)
        {
            Debug.LogError("GameManager is not assigned!");
            return;
        }

        // Validate placement based on the assigned type
        if (gameManager.LeftType == AssignedType && IsOverArea("LeftArea"))
        {
            Debug.Log($"Correct placement in LeftArea for type: {AssignedType}");
            HandleCorrectPlacement();
        }
        else if (gameManager.RightType == AssignedType && IsOverArea("RightArea"))
        {
            Debug.Log($"Correct placement in RightArea for type: {AssignedType}");
            HandleCorrectPlacement();
        }
        else
        {
            Debug.Log($"Incorrect placement for type: {AssignedType}");
            HandleIncorrectPlacement();
        }
    }

    private bool IsOverArea(string areaName)
    {
        RectTransform area = GameObject.Find(areaName)?.GetComponent<RectTransform>();
        if (area == null)
        {
            Debug.LogError($"Drop area '{areaName}' not found!");
            return false;
        }

        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition, Camera.main);
        Debug.Log($"Checking placement over {areaName}: {isOver}");
        return isOver;
    }

    private void HandleCorrectPlacement()
    {
        // Add points using the existing ScoreManager
        if (scoreManager != null)
        {
            scoreManager.AddScore(pointsForCorrectDrop);
        }
        else
        {
            Debug.LogError("ScoreManager instance is not available!");
        }

        if (correctIndicatorPrefab != null)
        {
            GameObject correctIndicator = Instantiate(correctIndicatorPrefab, transform.parent);
            Destroy(correctIndicator, destroyDelay);
        }

        Destroy(gameObject); // Remove the asteroid after correct placement
    }


    private void HandleIncorrectPlacement()
    {
        if (incorrectIndicatorPrefab != null)
        {
            GameObject incorrectIndicator = Instantiate(incorrectIndicatorPrefab, transform.parent);
            Destroy(incorrectIndicator, destroyDelay);
        }

        Destroy(gameObject, destroyDelay); // Remove the asteroid after incorrect placement
    }
}
