using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewColorSortingRule", menuName = "Sorting Rules/Color Sorting Rule")]
public class ColorSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Define type-color pairs for sorting")]
    public List<TypeColorPair> typeColorPairs;

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Assign a random color-based category if not already assigned
            if (typeColorPairs.Count > 0)
            {
                Color assignedColor = typeColorPairs[Random.Range(0, typeColorPairs.Count)].color;
                draggable.AssignedColor = assignedColor; // Store for consistency
                Debug.Log($"Assigned random color: {assignedColor}");
                return assignedColor.ToString(); // Use color as the category
            }
            else
            {
                Debug.LogError("typeColorPairs list is empty! Cannot assign a random color.");
                return "Default"; // Fallback
            }
        }

        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return "Default"; // Fallback
    }

    public void ApplyVisuals(GameObject item)
    {
        if (item.TryGetComponent<Image>(out var image) && item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Find the color pair based on the assigned color
            var pair = typeColorPairs.Find(p => p.color == draggable.AssignedColor);

            if (pair != null)
            {
                image.color = pair.color; // Apply the matching color
                Debug.Log($"Asteroid visuals applied for color: {pair.color}");
            }
            else
            {
                Debug.LogWarning($"No matching color found for assigned color. Applying default color.");
                image.color = Color.white; // Default color
            }
        }
        else
        {
            Debug.LogError("Image or SortingDraggableItem component not found on the asteroid!");
        }
    }
}
