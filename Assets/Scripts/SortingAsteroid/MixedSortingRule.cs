using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Sorting Rules/Mixed Sorting Rule")]
public class MixedSortingRule : ScriptableObject, ISortingRule
{
    [System.Serializable]
    public class MixedCondition
    {
        public string size; // "Small" or "Large"
        public Color color; // Assigned color
        public string dropZoneName; // Target drop zone
    }

    public List<MixedCondition> conditions; // Define mixed sorting conditions

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            var condition = conditions[Random.Range(0, conditions.Count)];
            draggable.AssignedType = $"{condition.size} {condition.color}";
            draggable.AssignedSize = condition.size == "Small" ? 1f : 2f; // Example size mapping
            draggable.AssignedColor = condition.color;
            return draggable.AssignedType;
        }
        Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        return null;
    }

    public void ApplyVisuals(GameObject item)
    {
        if (item.TryGetComponent<Image>(out var image) && item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            image.color = draggable.AssignedColor;
            item.transform.localScale = Vector3.one * draggable.AssignedSize; // Scale based on size
        }
    }

    public string GetRandomType()
    {
        if (conditions.Count > 0)
        {
            var condition = conditions[Random.Range(0, conditions.Count)];
            return $"{condition.size} {condition.color}";
        }
        return "Default";
    }
}
