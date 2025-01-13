using UnityEngine;

public class LineRendererAnimator : MonoBehaviour
{
    public LineRenderer lineRenderer; // The LineRenderer to animate
    public Transform startPoint; // Fixed start point of the line
    public Transform endPoint; // Current end point of the line
    public float animationDuration = 10f; // Duration of the disconnect effect

    private Vector3 initialEndPoint; // The original end point position
    private Vector3 targetEndPoint; // The target position for the disconnect
    private float elapsedTime = 0f; // Time elapsed during animation
    private bool isAnimating = false; // Whether the animation is running

    void Start()
    {
        if (lineRenderer == null || startPoint == null || endPoint == null)
        {
            Debug.LogError("Please assign the LineRenderer, StartPoint, and EndPoint!");
            return;
        }

        // Initialize LineRenderer positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // Ensure the LineRenderer's colors are set correctly
        Color lineColor = Color.red; // Replace with your desired color
        lineColor.a = 1f; // Ensure full opacity
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
    }


    public void StartDisconnectAnimation(Vector3 disconnectTarget)
    {
        // Set the target position for the end point
        initialEndPoint = endPoint.position;
        targetEndPoint = disconnectTarget; // Where the end point will "retract" to
        elapsedTime = 0f;
        isAnimating = true;
    }

    void Update()
    {
        if (isAnimating)
        {
            elapsedTime += Time.deltaTime;

            // Animate the end point position
            Vector3 currentEndPoint = Vector3.Lerp(initialEndPoint, targetEndPoint, elapsedTime / animationDuration);
            lineRenderer.SetPosition(1, currentEndPoint);

            // Stop the animation when it's complete
            if (elapsedTime >= animationDuration)
            {
                isAnimating = false;
                Debug.Log("Disconnect animation completed!");
            }
        }

        // Keep the start point updated (in case it moves dynamically)
        lineRenderer.SetPosition(0, startPoint.position);
    }
}
