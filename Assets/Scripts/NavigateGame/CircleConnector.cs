using UnityEngine;

public class CircleConnector : MonoBehaviour
{
    [Header("Circles and Line Settings")]
    public GameObject[] circles; // Assign all circles in the desired connection order
    public Material lineMaterial; // Material for the lines
    [SerializeField] private float lineWidth = 0.05f; // Width of the lines

    void Start()
    {
        if (circles == null || circles.Length < 2)
        {
            Debug.LogError("Insufficient circles assigned to connect. At least two are required.");
            return;
        }

        ConnectCircles();
    }

    void ConnectCircles()
    {
        for (int i = 0; i < circles.Length - 1; i++)
        {
            // Create a new GameObject for the line
            GameObject lineObject = new GameObject($"Line_{i}");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

            // Set the line renderer properties
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;

            // Set the line's start and end positions
            lineRenderer.SetPosition(0, circles[i].transform.position);
            lineRenderer.SetPosition(1, circles[i + 1].transform.position);
        }
    }
}
