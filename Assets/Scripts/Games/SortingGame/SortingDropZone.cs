using UnityEngine;

public class SortingDropZone : MonoBehaviour
{
    [Tooltip("The allowed item type for this drop zone (e.g., 'blue', 'yellow')")]
    [SerializeField] private string allowedItemType;

    public bool IsCorrectDrop(string itemType)
    {
        return itemType == allowedItemType;
    }
}
