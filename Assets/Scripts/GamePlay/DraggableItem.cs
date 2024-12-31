using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image image; // The item's image component

    public int TubeID { get; set; } // Hidden unique identifier for internal use

    public Image Image => image; // Public property for read-only access

    private Transform parentAfterDrag;

    public Transform ParentAfterDrag
    {
        get => parentAfterDrag;
        set => parentAfterDrag = value;
    }

    private const string LOG_BEGIN_DRAG = "Begin drag";
    private const string LOG_DRAGGING = "Dragging";
    private const string LOG_END_DRAG = "End drag";

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(LOG_BEGIN_DRAG);

        ParentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false; // Disable raycast while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Allow dragging
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(LOG_END_DRAG);

        transform.SetParent(ParentAfterDrag);
        image.raycastTarget = true; // Re-enable raycast
    }
}
