using UnityEngine;
using UnityEngine.EventSystems;

public class Wire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public LineRenderer lineRenderer; // Attach the LineRenderer in the Inspector
    public RectTransform sourceSocket; // The starting point (UI element in RectTransform)
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 offset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            Debug.LogError("Wire script must be attached to a UI element inside a Canvas!");
        }

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned to the Wire script!");
        }

        // Initialize LineRenderer
        if (sourceSocket != null)
        {
            lineRenderer.positionCount = 2; // Ensure it has 2 points
            UpdateLineStartPoint(); // Set the initial start point of the LineRenderer
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0)); // End point starts at the same position
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Calculate offset between pointer and object position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out offset
        );

        // Optional: Change appearance during drag
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false; // Allow events to pass through
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Vector2 pointerPosition;
        // Convert pointer position to Canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerPosition
        );

        // Update the dragged object's position
        rectTransform.anchoredPosition = pointerPosition - offset;

        // Update the LineRenderer's end position
        UpdateLineEndPoint();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Restore appearance after drag
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void UpdateLineStartPoint()
    {
        // Convert the source socket position from Canvas space to World space
        Vector3 worldStartPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, sourceSocket.position);
        worldStartPoint = Camera.main.ScreenToWorldPoint(new Vector3(worldStartPoint.x, worldStartPoint.y, Camera.main.nearClipPlane));
        worldStartPoint.z = 0; // Ensure the Z position is correct for the LineRenderer

        lineRenderer.SetPosition(0, worldStartPoint); // Set the start point
    }

    private void UpdateLineEndPoint()
    {
        // Convert the draggable object's position from Canvas space to World space
        Vector3 worldEndPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
        worldEndPoint = Camera.main.ScreenToWorldPoint(new Vector3(worldEndPoint.x, worldEndPoint.y, Camera.main.nearClipPlane));
        worldEndPoint.z = 0; // Ensure the Z position is correct for the LineRenderer

        lineRenderer.SetPosition(1, worldEndPoint); // Set the end point
    }
}
