using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 pointerOffset; // Stores the offset between the pointer and the object's center

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if interaction is allowed
        if (!EquipmentRecoveryGameManager.Instance.IsInteractionAllowed())
        {
           // Debug.Log("Dragging is not allowed yet!");
            return;
        }

        canvasGroup.blocksRaycasts = false;

        // Calculate the offset between the mouse position and the object's position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );

        // Play drag start sound
        if (AudioFeedbackManager.Instance != null)
        {
            AudioFeedbackManager.Instance.PlayDragStartSound();
            //Debug.Log("Playing drag start sound.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Prevent dragging if interaction is not allowed
        if (!EquipmentRecoveryGameManager.Instance.IsInteractionAllowed())
            return;

        // Update the object's position based on the drag
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition
        ))
        {
            rectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Check if interaction is allowed before handling drop logic
        if (!EquipmentRecoveryGameManager.Instance.IsInteractionAllowed())
            return;

       // Debug.Log($"{gameObject.name} dropped!");
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts after dragging
    }
}
