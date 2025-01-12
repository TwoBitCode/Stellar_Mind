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
            // Use the already assigned type if available
            if (!string.IsNullOrEmpty(draggable.AssignedType))
            {
                return draggable.AssignedType;
            }

            // Otherwise, assign a new random type
            if (typeColorPairs.Count > 0)
            {
                string assignedType = typeColorPairs[Random.Range(0, typeColorPairs.Count)].typeName;
                draggable.AssignedType = assignedType; // Store for consistency
                Debug.Log($"Assigned random type: {assignedType}");
                return assignedType;
            }
            else
            {
                Debug.LogError("typeColorPairs list is empty! Cannot assign a random type.");
                return null;
            }
        }

        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return null;
    }

    public void ApplyVisuals(GameObject item)
    {
        if (item.TryGetComponent(out Image image) && item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            var pair = typeColorPairs.Find(p => p.typeName == draggable.AssignedType);

            if (pair != null)
            {
                image.color = pair.color; // Apply the matching color
                Debug.Log($"Asteroid visuals applied for type: {draggable.AssignedType}");
            }
            else
            {
                Debug.LogWarning($"No color mapping found for type: {draggable.AssignedType}. Applying default color.");
                image.color = Color.white; // Apply a default color (e.g., white)
            }
        }
        else
        {
            Debug.LogError("Image or SortingDraggableItem component not found on the asteroid!");
        }
    }
}
