using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SortingDraggableItem : DraggableItem
{
    public string AssignedType { get; set; } // Dynamically assigned type
    public float AssignedSize { get; set; } // Dynamically assigned size
    public bool IsDistractor { get; set; } // Determines if the asteroid is a distractor

    [SerializeField] private int pointsForCorrectDrop = 10;
    [SerializeField] private float destroyDelay = 0.2f;

    [SerializeField] private GameObject correctIndicatorPrefab;
    [SerializeField] private GameObject incorrectIndicatorPrefab;
    public Color AssignedColor { get; set; } // Store assigned color

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
        if (IsDistractor)
        {
            Debug.Log("Distractor asteroid ignored!");
            Destroy(gameObject, destroyDelay); // Distractors disappear after being dropped
            return;
        }

        base.OnEndDrag(eventData);

        var currentChallenge = gameManager?.ChallengeManager?.CurrentChallenge;
        if (currentChallenge == null)
        {
            Debug.LogError("No current challenge available!");
            return;
        }

        // Check all active drop zones
        foreach (var assignment in currentChallenge.dropZoneAssignments)
        {
            if (IsOverCorrectDropZone(AssignedType, assignment.dropZoneName))
            {
                if (assignment.assignedType == AssignedType)
                {
                    Debug.Log($"Correct placement in {assignment.dropZoneName} for AssignedType: {AssignedType}");
                    HandleCorrectPlacement();
                    return;
                }
            }
        }

        Debug.Log($"Incorrect placement for AssignedType: {AssignedType}");
        HandleIncorrectPlacement();
    }




    private bool IsOverCorrectDropZone(string assignedType, string dropZoneName)
    {
        RectTransform area = GameObject.Find(dropZoneName)?.GetComponent<RectTransform>();
        if (area == null)
        {
            Debug.LogError($"Drop area '{dropZoneName}' not found!");
            return false;
        }

        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition, Camera.main);
        Debug.Log($"Checking placement over {dropZoneName}: {isOver}");
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
