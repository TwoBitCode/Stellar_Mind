using UnityEngine;
using UnityEngine.EventSystems;

public class SortingDraggableItem : DraggableItem
{
    [Tooltip("Points awarded for a correct drop")]
    [SerializeField] private int pointsForCorrectDrop = 10;

    [Tooltip("The type of this draggable item (e.g., 'blue', 'yellow')")]
    [SerializeField] public string itemType;

    public string ItemType
    {
        get => itemType;
        set => itemType = value;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData); // Call the base class drag-end logic

        if (IsDroppedInCorrectArea(out var dropZone))
        {
            Debug.Log($"Dropped in the correct area: {dropZone.name}!");
            HandleCorrectDrop();
        }
        else
        {
            Debug.Log("Dropped in the wrong area!");
            HandleIncorrectDrop();
        }
    }

    private bool IsDroppedInCorrectArea(out GameObject dropZone)
    {
        dropZone = null;

        // Check if the item is over a valid drop area
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, raycastResults);

        foreach (var result in raycastResults)
        {
            // Check for drop area by tag or component
            if (result.gameObject.CompareTag("DropArea"))
            {
                dropZone = result.gameObject;
                return dropZone.GetComponent<SortingDropZone>()?.IsCorrectDrop(ItemType) ?? false;
            }
        }

        return false;
    }

    private void HandleCorrectDrop()
    {
        // Add points for the correct drop
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(pointsForCorrectDrop);
        }

        // Perform additional actions for a correct drop
        Destroy(gameObject); // Optional: Destroy the item after a correct drop
    }

    private void HandleIncorrectDrop()
    {
        // Optional: Add visual or auditory feedback for incorrect drops
        // Reset the item to its original position
        transform.SetParent(ParentAfterDrag);
        transform.localPosition = Vector3.zero;
    }
}
