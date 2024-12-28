using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    [SerializeField] public Image image; // Add this field for the image reference

    // Stores the original parent of the item before dragging
    public Transform ParentAfterDrag { get; set; }

    // Called when the user starts dragging the item
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save the item's current parent
        ParentAfterDrag = transform.parent;

        // Move the item to the root of the hierarchy (so it stays on top visually)
        transform.SetParent(transform.root);

        // Make sure the item is the last sibling, ensuring it appears on top
        transform.SetAsLastSibling();
    }

    // Called every frame while the user drags the item
    public void OnDrag(PointerEventData eventData)
    {
        // Update the item's position to follow the mouse cursor
        transform.position = Input.mousePosition;
    }

    // Called when the user releases the item
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // Return the item to its original parent
        transform.SetParent(ParentAfterDrag);
    }
}
