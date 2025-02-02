using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SortingDraggableItem : DraggableItem, IDragHandler
{
    public string AssignedType { get; set; } // Dynamically assigned type
    public float AssignedSize { get; set; } // Dynamically assigned size
    public bool IsDistractor { get; set; } // Determines if the asteroid is a distractor

    //[SerializeField] private int pointsForCorrectDrop = 10;
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

        // Apply visuals based on the assigned type or mixed conditions
        var currentChallenge = gameManager?.ChallengeManager?.CurrentChallenge;

        if (currentChallenge != null)
        {
            if (currentChallenge.mixedConditions != null && currentChallenge.mixedConditions.Count > 0)
            {
                // Mixed challenge: visuals are based on assigned size and color
                GetComponent<Image>().color = AssignedColor;
               // Debug.Log($"Asteroid visuals applied for Mixed Challenge: {AssignedType}");
            }
            else if (currentChallenge.sortingRule is ISortingRule rule)
            {
                // Regular challenge: Apply sorting rule visuals
                rule.ApplyVisuals(gameObject);
               // Debug.Log($"Asteroid visuals applied for Regular Challenge: {AssignedType}");
            }
            else
            {
               // Debug.LogError("No valid sorting rule or mixed conditions found!");
            }
        }
        else
        {
            //Debug.LogError("No current challenge available!");
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        // Ensure the Image component is enabled
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.enabled = true;
        }

        // Play grab sound globally
        GlobalAsteroidSoundManager.Instance?.PlayGrabSound();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (IsDistractor)
        {
            //Debug.Log("Distractor asteroid ignored!");
            Destroy(gameObject, destroyDelay); // Distractors disappear after being dropped
            return;
        }

        base.OnEndDrag(eventData);

        var currentChallenge = gameManager?.ChallengeManager?.CurrentChallenge;
        if (currentChallenge == null)
        {
            //Debug.LogError("No current challenge available!");
            return;
        }

        // Check all active drop zones
        foreach (var assignment in currentChallenge.dropZoneAssignments)
        {
            if (IsOverCorrectDropZone(assignment.dropZoneName))
            {
                if (currentChallenge.mixedConditions != null && currentChallenge.mixedConditions.Count > 0)
                {
                    // Mixed Challenge: Check size and color
                    var matchingCondition = currentChallenge.mixedConditions.Find(
                        condition => condition.dropZoneName == assignment.dropZoneName
                                     && condition.size == GetSizeAsString(AssignedSize) // Compare size as string
                                     && condition.color == AssignedColor);

                    if (matchingCondition != null)
                    {
                        //Debug.Log($"Correct placement for Mixed Condition: {matchingCondition.dropZoneName}");
                        HandleCorrectPlacement();
                        return;
                    }
                }
                else
                {
                    // Regular Challenge: Check only the assigned type
                    if (assignment.assignedType == AssignedType)
                    {
                        //Debug.Log($"Correct placement in {assignment.dropZoneName} for AssignedType: {AssignedType}");
                        HandleCorrectPlacement();
                        return;
                    }
                }
            }
        }

      //  Debug.Log($"Incorrect placement for AssignedType: {AssignedType}");
        HandleIncorrectPlacement();
    }




    private bool IsOverCorrectDropZone(string dropZoneName)
    {
        RectTransform area = GameObject.Find(dropZoneName)?.GetComponent<RectTransform>();
        if (area == null)
        {
            //Debug.LogError($"Drop area '{dropZoneName}' not found!");
            return false;
        }

        bool isOver = RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition, Camera.main);
       // Debug.Log($"Checking placement over {dropZoneName}: {isOver}");
        return isOver;
    }

    private string GetSizeAsString(float sizeValue)
    {
        return sizeValue == 1.0f ? "Small" : sizeValue == 3.0f ? "Large" : "Unknown";
    }

    private void HandleCorrectPlacement()
    {
        GlobalAsteroidSoundManager.Instance?.PlayCorrectSound();

        // Notify the game manager
        gameManager?.OnAsteroidSorted(true);

        if (correctIndicatorPrefab != null)
        {
            GameObject correctIndicator = Instantiate(correctIndicatorPrefab, transform.parent);
            Destroy(correctIndicator, destroyDelay);
        }

        Destroy(gameObject);
    }

    private void HandleIncorrectPlacement()
    {
        GlobalAsteroidSoundManager.Instance?.PlayIncorrectSound();

        // Notify the game manager
        gameManager?.OnAsteroidSorted(false);

        if (incorrectIndicatorPrefab != null)
        {
            GameObject incorrectIndicator = Instantiate(incorrectIndicatorPrefab, transform.parent);
            Destroy(incorrectIndicator, destroyDelay);
        }

        Destroy(gameObject, destroyDelay);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();

        if (rectTransform != null && canvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint);

            rectTransform.localPosition = localPoint; // Move inside UI
        }

        Debug.Log($"Asteroid {gameObject.name} is moving to: {rectTransform.localPosition}");
    }




}
