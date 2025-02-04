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
    public float elbowOffset = 1f; // Offset for the elbow position

    private Vector3 fixedStartPoint; // Cached start point world position

    void Start()
    {
        // Initialize required components
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Cache the start point world position
        if (lineRenderer != null && startPoint != null)
        {
            lineRenderer.positionCount = 4; // Start, elbow, and end positions
            UpdateLineEndPosition();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Dragging Started");

        // Play the drag sound only once when dragging starts
        if (CableAudioManager.Instance != null && CableAudioManager.Instance.dragSound != null)
        {
            CableAudioManager.Instance.PlayOneShot(CableAudioManager.Instance.dragSound);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert the mouse position to world space using the canvas camera
        Vector3 newPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, canvas.worldCamera, out newPosition);

        // Reduce the offset to bring it even closer to the mouse
        newPosition.y += 0.1f; // Try lowering this value further if needed
        newPosition.x += 0.05f; // Slight horizontal adjustment (optional)

        // Apply the new position
        rectTransform.position = newPosition;

        // Update the wire positions during drag
        UpdateWireTipsPosition();
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Dragging Ended");

        // Stop any looping sound or reset if necessary
        if (CableAudioManager.Instance != null)
        {
            CableAudioManager.Instance.StopSound();
        }

        // Check for the closest target
        RectTransform connectedTarget = GetClosestTarget();

        if (connectedTarget != null && connectionManager != null)
        {
            // Snap to the closest target
            SnapToTarget(connectedTarget);

            // Play snap sound
            if (CableAudioManager.Instance != null && CableAudioManager.Instance.snapSound != null)
            {
                CableAudioManager.Instance.PlayOneShot(CableAudioManager.Instance.snapSound);
            }

            // Notify the connection manager about the connection
            connectionManager.OnCableConnected(this, connectedTarget);
        }
        else
        {
            // Play reset sound
            if (CableAudioManager.Instance != null && CableAudioManager.Instance.resetSound != null)
            {
                CableAudioManager.Instance.PlayOneShot(CableAudioManager.Instance.resetSound);
            }

            ResetToStartPosition();
        }
    }

    public void ResetToStartPosition()
    {
        // Move the cable back to its starting position
        rectTransform.anchoredPosition = startPoint.anchoredPosition;

        // Update the LineRenderer to reflect the reset position
        if (lineRenderer != null)
        {
            Vector3 worldEndPoint = GetWorldPosition(rectTransform);
            worldEndPoint.z = 0; // Ensure the z position remains consistent

            // Update the LineRenderer positions with fixed z values
            Vector3 startPosition = GetWorldPosition(startPoint);
            startPosition.z = 0;

            lineRenderer.SetPosition(2, startPosition); // Secondary elbow (directly to endpoint)
            lineRenderer.SetPosition(3, startPosition); // Endpoint
        }

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
            UpdateWireTipsPosition();

            Debug.Log($"Snapped to target: {target.name}");
        }
        else
        {
            Debug.Log("No valid target to snap to.");
        }
    }

    private void UpdateLineEndPosition()
    {
        if (lineRenderer != null)
        {
            Vector3 draggablePosition = rectTransform.position;
            draggablePosition.z = 0; // Ensure z remains consistent

            // Update the LineRenderer's endpoint (Point 3)
            lineRenderer.SetPosition(3, draggablePosition);

            // Get the position of the first elbow (Point 1)
            Vector3 firstElbow = lineRenderer.GetPosition(1);

            // Calculate the direction vector from the first elbow to the draggable endpoint
            Vector3 directionToEnd = (draggablePosition - firstElbow).normalized;

            // Calculate the position of the second elbow (Point 2) slightly before the endpoint
            Vector3 secondElbow = draggablePosition - directionToEnd * 0.5f; // Adjust 0.5f for proximity to Point 3
            secondElbow.z = 0; // Ensure consistent z position

            // Update the secondary elbow (Point 2)
            lineRenderer.SetPosition(2, secondElbow);

        }
    }


    private Vector3 GetWorldPosition(RectTransform rect)
    {
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position);
        return canvas.worldCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, canvas.worldCamera.nearClipPlane));
    }

    private void UpdateWireTipsPosition()
    {
        if (lineRenderer != null)
        {
            // Get the draggable object's position (Point 3)
            Vector3 draggablePosition = rectTransform.position;
            draggablePosition.z = 0; // Ensure z remains consistent

            // Update the LineRenderer's endpoint (Point 3)
            lineRenderer.SetPosition(3, draggablePosition);

            // Get the position of Point 1 (First Elbow)
            Vector3 firstElbow = lineRenderer.GetPosition(1);

            // Calculate the direction vector from Point 1 to Point 3
            Vector3 directionToEnd = (draggablePosition - firstElbow).normalized;

            // Calculate the position of the secondary elbow (Point 2)
            Vector3 secondElbow = draggablePosition - directionToEnd * 0.5f; // Place the elbow closer to the endpoint
            secondElbow.x -= elbowOffset; // Subtract from x to bend left
            secondElbow.y = draggablePosition.y; // Align the y-coordinate to Point 3
            secondElbow.z = 0; // Ensure consistent z position

            // Update the secondary elbow position (Point 2)
            lineRenderer.SetPosition(2, secondElbow);

            // Debug all positions to verify calculations
            //Debug.Log($"P0: {lineRenderer.GetPosition(0)} | P1: {lineRenderer.GetPosition(1)} | P2: {lineRenderer.GetPosition(2)} | P3: {lineRenderer.GetPosition(3)}");
        }
    }
}
