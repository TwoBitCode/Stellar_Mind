using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    // A constant to represent an empty slot (no children in the transform)
    private const int NO_CHILDREN = 0;

    // This method is called when a draggable item is dropped onto this slot
    public void OnDrop(PointerEventData eventData)
    {
        // Check if the slot is empty (no child objects)
        if (transform.childCount == NO_CHILDREN)
        {
            // Get the game object that was dragged and dropped
            GameObject dropped = eventData.pointerDrag;

            // Try to get the DraggableItem component from the dropped object
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                // If the dropped object has a DraggableItem component, 
                // set this slot as its new parent after the drag operation
                draggableItem.ParentAfterDrag = transform;
            }
        }
    }
}
