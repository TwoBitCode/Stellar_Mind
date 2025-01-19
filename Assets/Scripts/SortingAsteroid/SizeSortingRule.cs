using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Asteroid Challenges/Size Sorting Rule")]
public class SizeSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Scale for small asteroids")]
    public Vector3 smallSize = new Vector3(1f, 1f, 1f);

    [Tooltip("Scale for large asteroids")]
    public Vector3 largeSize = new Vector3(1.2f, 1.2f, 1.2f);

    // List to track unassigned types
    private List<string> remainingTypes = new List<string> { "Small", "Large" };

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Assign type based on predefined logic
            string type = GetRandomType(); // Use the improved method for type selection
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

    public string GetRandomType()
    {
        // Ensure each type is seen at least once before repeating
        if (remainingTypes.Count == 0)
        {
            // Reset the list when all types have been used
            remainingTypes.Add("Small");
            remainingTypes.Add("Large");
        }

        // Select a random type from the remaining list
        int index = Random.Range(0, remainingTypes.Count);
        string selectedType = remainingTypes[index];
        remainingTypes.RemoveAt(index); // Remove the selected type to avoid repetition

        Debug.Log($"Randomly selected type: {selectedType}. Remaining: {string.Join(", ", remainingTypes)}");
        return selectedType;
    }
}
