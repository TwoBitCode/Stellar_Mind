using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        // Play drag start sound
        if (AudioFeedbackManager.Instance != null)
        {
            AudioFeedbackManager.Instance.PlayDragStartSound();
            Debug.Log("Playing drag start sound.");
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Update the object's position based on the drag
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"{gameObject.name} dropped!");
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts after dragging
    }

}
