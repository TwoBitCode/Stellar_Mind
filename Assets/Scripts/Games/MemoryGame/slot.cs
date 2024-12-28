using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    // Constant to define the condition for accepting a drop
    private const int NoChildren = 0;

    public void OnDrop(PointerEventData eventData)
    {
        // Check if the slot is empty
        if (transform.childCount == NoChildren)
        {
            // Get the dragged object
            GameObject dropped = eventData.pointerDrag;

            // Ensure the object has a DraggableItem component
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                // Update the parent of the draggable item
                draggableItem.ParentAfterDrag = transform;

                // Optionally: snap the item to the center of the slot
                SnapToSlot(dropped);
            }
        }
    }

    // Method to snap the draggable item to the center of the slot
    private void SnapToSlot(GameObject droppedItem)
    {
        RectTransform droppedRectTransform = droppedItem.GetComponent<RectTransform>();
        RectTransform slotRectTransform = GetComponent<RectTransform>();

        if (droppedRectTransform != null && slotRectTransform != null)
        {
            // Align the position of the dragged item to the slot's position
            droppedRectTransform.anchoredPosition = slotRectTransform.anchoredPosition;
        }
    }
}
