using UnityEngine;

[CreateAssetMenu(menuName = "Asteroid Challenges/Size Sorting Rule")]
public class SizeSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Scale for small asteroids")]
    public Vector3 smallSize = new Vector3(1f, 1f, 1f);

    [Tooltip("Scale for large asteroids")]
    public Vector3 largeSize = new Vector3(1.2f, 1.2f, 1.2f);

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Assign type based on predefined logic
            string type = Random.value > 0.5f ? "Large" : "Small";
            draggable.AssignedType = type; // Store the type
            return type;
        }

        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return null;
    }

    public void ApplyVisuals(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Apply the predefined size based on the type
            if (draggable.AssignedType == "Small")
            {
                item.transform.localScale = smallSize;
                Debug.Log($"Asteroid scaled to small size: {smallSize}");
            }
            else if (draggable.AssignedType == "Large")
            {
                item.transform.localScale = largeSize;
                Debug.Log($"Asteroid scaled to large size: {largeSize}");
            }
        }
        else
        {
            Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        }
    }

    // New method for ISortingRule
    public string GetRandomType()
    {
        // Randomly return "Small" or "Large"
        return Random.value > 0.5f ? "Small" : "Large";
    }
}
