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
            if (typeColorPairs.Count > 0)
            {
                // Randomly select a type-color pair
                var assignedPair = typeColorPairs[Random.Range(0, typeColorPairs.Count)];
                draggable.AssignedType = assignedPair.typeName; // Assign the type name
                draggable.AssignedColor = assignedPair.color;   // Assign the color
                Debug.Log($"Assigned Type: {assignedPair.typeName}, Assigned Color: {assignedPair.color}");
                return assignedPair.typeName; // Return the assigned type name
            }
            else
            {
                Debug.LogError("typeColorPairs list is empty! Cannot assign a random color.");
                return "Default"; // Fallback value
            }
        }

        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return "Default"; // Fallback value
    }




    public void ApplyVisuals(GameObject item)
    {
        if (item.TryGetComponent<Image>(out var image) && item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            Debug.Log($"Applying visuals for AssignedColor: {draggable.AssignedColor}");

            // Find the matching TypeColorPair for the assigned color
            var pair = typeColorPairs.Find(p => p.color == draggable.AssignedColor);

            if (pair != null)
            {
                image.color = pair.color; // Apply the matching color
                Debug.Log($"Asteroid visuals applied for AssignedType: {pair.typeName}, Color: {pair.color}");
            }
            else
            {
                Debug.LogWarning($"No matching color found for AssignedColor: {draggable.AssignedColor}. Applying default color.");
                image.color = Color.white; // Default color
            }
        }
        else
        {
            Debug.LogError("Image or SortingDraggableItem component not found on the asteroid!");
        }
    }



    public string GetRandomType()
    {
        if (typeColorPairs.Count > 0)
        {
            // Pick a random type-color pair and return the type
            var randomPair = typeColorPairs[Random.Range(0, typeColorPairs.Count)];
            return randomPair.typeName; // Return the name of the type (e.g., "Red", "Blue")
        }

        Debug.LogError("typeColorPairs list is empty! Cannot generate a random type.");
        return "Default"; // Fallback
    }
}
