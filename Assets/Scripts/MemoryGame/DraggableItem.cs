using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    [SerializeField] public Image image;

    [HideInInspector] public Transform parentAfterDrag;

    [Tooltip("log text when draggin starts")]
    [SerializeField] private const string LOG_BEGIN_DRAG = "Begin drag";
    [Tooltip("log text while draggin")]
    [SerializeField] private const string LOG_DRAGGING = "dragging";
    [Tooltip("log text when draggin ends")]
    [SerializeField] private const string LOG_END_DRAG = "End drag";

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(LOG_BEGIN_DRAG);

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(LOG_DRAGGING);
        transform.position = Input.mousePosition;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(LOG_END_DRAG);
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
