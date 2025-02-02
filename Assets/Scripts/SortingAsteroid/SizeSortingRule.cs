using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Asteroid Challenges/Size Sorting Rule")]
public class SizeSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Scale for small asteroids")]
    public Vector3 smallSize = new Vector3(1f, 1f, 1f);

    [Tooltip("Scale for large asteroids")]
    public Vector3 largeSize = new Vector3(1.2f, 1.2f, 1.2f);

    private Queue<string> typeQueue = new Queue<string>(new[] { "Small", "Large" }); // Queue for cyclic selection

    public string GetCategory(GameObject item)
    {
        if (item.TryGetComponent<SortingDraggableItem>(out var draggable))
        {
            // Assign type from the queue
            string type = GetNextType();
            draggable.AssignedType = type;
            return type;
        }

      //  Debug.LogError("SortingDraggableItem component missing on the asteroid!");
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
              //  Debug.Log($"Asteroid scaled to small size: {smallSize}");
            }
            else if (draggable.AssignedType == "Large")
            {
                item.transform.localScale = largeSize;
               // Debug.Log($"Asteroid scaled to large size: {largeSize}");
            }
        }
        else
        {
            //Debug.LogError("SortingDraggableItem component missing on the asteroid!");
        }
    }

    // Implement GetRandomType to satisfy ISortingRule
    public string GetRandomType()
    {
        // Redirect to GetNextType to maintain compatibility
        return GetNextType();
    }

    private string GetNextType()
    {
        // Dequeue the next type and re-enqueue it to the end
        string nextType = typeQueue.Dequeue();
        typeQueue.Enqueue(nextType);

     //   Debug.Log($"Selected type: {nextType}");
        return nextType;
    }
}
