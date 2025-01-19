using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewColorSortingRule", menuName = "Sorting Rules/Color Sorting Rule")]
public class ColorSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Define type-color pairs for sorting")]
    public List<TypeColorPair> typeColorPairs;

    // A HashSet to track assigned types (unique)
    private HashSet<string> assignedTypes = new HashSet<string>();

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Ensure we have unassigned types
            List<TypeColorPair> unassignedPairs = GetUnassignedPairs();

            if (unassignedPairs.Count > 0)
            {
                // Select a type-color pair from the unassigned list
                var assignedPair = unassignedPairs[Random.Range(0, unassignedPairs.Count)];
                assignedTypes.Add(assignedPair.typeName); // Mark this type as assigned

                draggable.AssignedType = assignedPair.typeName; // Assign the type name
                draggable.AssignedColor = assignedPair.color;   // Assign the color

                Debug.Log($"Assigned Type: {assignedPair.typeName}, Assigned Color: {assignedPair.color}");
                return assignedPair.typeName; // Return the assigned type name
            }
            else
            {
                Debug.LogWarning("No unassigned types available. Resetting assigned types.");
                ResetAssignedTypes(); // Reset when all types are used
                return GetCategory(item); // Retry
            }
        }

        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return "None"; // Fallback value
    }

    public void ApplyVisuals(GameObject asteroid)
    {
        if (asteroid.TryGetComponent<Image>(out var image))
        {
            if (asteroid.TryGetComponent<SortingDraggableItem>(out var draggableItem))
            {
                // Check if the AssignedColor is valid and exists in typeColorPairs
                if (typeColorPairs.Exists(pair => pair.color == draggableItem.AssignedColor))
                {
                    image.color = draggableItem.AssignedColor; // Apply color
                    Debug.Log($"Asteroid visuals applied. Type: {draggableItem.AssignedType}, Color: {draggableItem.AssignedColor}");
                }
                else
                {
                    Debug.LogWarning("AssignedColor is invalid or not found in typeColorPairs, applying default color.");
                    image.color = Color.white; // Default fallback
                }
            }
            else
            {
                Debug.LogError("SortingDraggableItem component missing on the asteroid!");
            }
        }
        else
        {
            Debug.LogError("Image component missing on the asteroid prefab!");
        }
    }

    public string GetRandomType()
    {
        List<TypeColorPair> unassignedPairs = GetUnassignedPairs();

        if (unassignedPairs.Count > 0)
        {
            var randomPair = unassignedPairs[Random.Range(0, unassignedPairs.Count)];
            assignedTypes.Add(randomPair.typeName); // Mark as assigned

            Debug.Log($"Randomly selected type: {randomPair.typeName}, Color: {randomPair.color}");
            return randomPair.typeName;
        }

        Debug.LogWarning("No unassigned types available. Resetting assigned types.");
        ResetAssignedTypes(); // Reset when all types are used
        return GetRandomType(); // Retry
    }

    private List<TypeColorPair> GetUnassignedPairs()
    {
        // Return type-color pairs that are not yet assigned
        return typeColorPairs.FindAll(pair => !assignedTypes.Contains(pair.typeName));
    }

    private void ResetAssignedTypes()
    {
        // Clear the assigned types
        assignedTypes.Clear();
    }
}
