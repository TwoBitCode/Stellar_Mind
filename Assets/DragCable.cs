using UnityEngine;
using UnityEngine.EventSystems;

public class DragCable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform; // The draggable wire
    private Canvas canvas; // The canvas the wire belongs to
    public LineRenderer lineRenderer; // The line representing the cable
    public RectTransform startPoint; // The fixed start point of the cable
    public CableConnectionManager connectionManager; // Reference to the manager script

    public float snapDistance = 5f; // Distance threshold for snapping to a target

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

        // Check for the closest target
        RectTransform connectedTarget = GetClosestTarget();

        if (connectedTarget != null && connectionManager != null)
        {
            // Snap to the closest target
            SnapToTarget(connectedTarget);

            // Notify the connection manager about the connection
            connectionManager.OnCableConnected(this, connectedTarget);
        }
        else
        {
            Debug.Log("No target connected. Resetting to start position.");
            ResetToStartPosition();
        }
    }

    private void ResetToStartPosition()
    {
        // Move the cable back to its starting position
        rectTransform.anchoredPosition = startPoint.anchoredPosition;

        // Update the LineRenderer to reflect the reset position
        UpdateLineEndPosition();

        Debug.Log("Cable reset to start position.");
    }


    private RectTransform GetClosestTarget()
    {
        RectTransform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (RectTransform target in connectionManager.stages[connectionManager.currentStage].targets)
        {
            // Convert target and cable positions to world space for distance calculation
            Vector3 cableWorldPos = rectTransform.position;
            Vector3 targetWorldPos = target.position;

            float distance = Vector3.Distance(cableWorldPos, targetWorldPos);

            // Debug log to verify distances
            Debug.Log($"Distance to {target.name}: {distance}");

            // Check if the target is within the snapping range
            CableTarget targetScript = target.GetComponent<CableTarget>();
            if (distance < snapDistance && distance < closestDistance && targetScript != null && targetScript.targetID == this.name)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }



    private void SnapToTarget(RectTransform target)
    {
        if (target != null)
        {
            // Snap the draggable cable to the target's world position
            rectTransform.position = target.position;

            // Update the LineRenderer to reflect the snapped position
            UpdateLineEndPosition();

            Debug.Log($"Snapped to target: {target.name}");
        }
        else
        {
            Debug.Log("No valid target to snap to.");
        }
    }


    private void UpdateLineStartPosition()
    {
        if (lineRenderer != null && startPoint != null)
        {
            Vector3 worldStartPoint = GetWorldPosition(startPoint);
            lineRenderer.SetPosition(0, worldStartPoint);
        }
    }
    private void UpdateLineEndPosition()
    {
        if (lineRenderer != null)
        {
            Vector3 worldEndPoint = GetWorldPosition(rectTransform);
            lineRenderer.SetPosition(1, worldEndPoint);

            // Update the rotation of the wire tips
            UpdateWireTipsRotation();
        }
    }


    private Vector3 GetWorldPosition(RectTransform rect)
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position);
        return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, canvas.worldCamera.nearClipPlane));
    }
    private void UpdateWireTipsRotation()
    {
        if (lineRenderer != null)
        {
            // Get the start and end positions of the LineRenderer
            Vector3 startPoint = lineRenderer.GetPosition(0);
            Vector3 endPoint = lineRenderer.GetPosition(1);

            // Calculate the direction vector and the angle
            Vector2 direction = (endPoint - startPoint).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotate the RectTransform of this wire to match the angle
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }


}
