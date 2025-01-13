using UnityEngine;
using UnityEngine.EventSystems;

public class DragCable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform; // The draggable wire
    private Canvas canvas; // The canvas the wire belongs to
    public LineRenderer lineRenderer; // The line representing the cable
    public RectTransform startPoint; // The fixed start point of the cable
    public CableConnectionManager connectionManager; // Reference to the manager script
    void Start()
    {
        // Initialize required components
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Set up the LineRenderer
        if (lineRenderer != null && startPoint != null)
        {
            lineRenderer.positionCount = 2; // Two points: start and end
            //UpdateLineStartPosition();
            UpdateLineEndPosition();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        // Triggered when the user clicks on the wire
        Debug.Log("Dragging Started");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Handle dragging the wire
        Vector2 pointerDelta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += pointerDelta;

        // Update the end position of the line during the drag
        UpdateLineEndPosition();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Dragging Ended");

        // Check for overlap with a target
        RectTransform connectedTarget = GetConnectedTarget();

        if (connectedTarget != null && connectionManager != null)
        {
            // Notify the connection manager about the connection
            connectionManager.OnCableConnected(this, connectedTarget);
        }
        else
        {
            Debug.Log("No target connected");
        }
    }
    private RectTransform GetConnectedTarget()
    {
        // Iterate through all targets in the current stage
        foreach (RectTransform target in connectionManager.stages[connectionManager.currentStage].targets)
        {
            // Check if the pointer is within the target's rectangle
            if (RectTransformUtility.RectangleContainsScreenPoint(target, Input.mousePosition, canvas.worldCamera))
            {
                return target; // Return the connected target
            }
        }

        return null; // No target found
    }


    private void UpdateLineEndPosition()
    {
        // Update the end position of the line based on the draggable wire's position
        Vector3 worldEndPoint = GetWorldPosition(rectTransform);
        lineRenderer.SetPosition(1, worldEndPoint);
    }

    private Vector3 GetWorldPosition(RectTransform rect)
    {
        // Convert the RectTransform's position to world space
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position);
        return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, canvas.worldCamera.nearClipPlane));
    }
}
