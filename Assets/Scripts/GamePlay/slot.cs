using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    // Constants
    private const int NO_CHILDREN = 0;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == NO_CHILDREN)
        {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            if (draggableItem != null)
            {
                // Use the public property to set the new parent
                draggableItem.ParentAfterDrag = transform;
            }
        }
    }
}
