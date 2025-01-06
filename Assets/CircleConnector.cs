using UnityEngine;

public class CircleConnector : MonoBehaviour
{
    public GameObject[] circles; // Assign all circles in the order you want to connect them
    public Material lineMaterial; // Assign the material for the lines

    void Start()
    {
        ConnectCircles();
    }

    void ConnectCircles()
    {
        for (int i = 0; i < circles.Length - 1; i++)
        {
            // Create a new LineRenderer for each connection
            GameObject lineObject = new GameObject($"Line_{i}");
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

            // Set the line properties
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.positionCount = 2;

            // Set the start and end points of the line
            lineRenderer.SetPosition(0, circles[i].transform.position);
            lineRenderer.SetPosition(1, circles[i + 1].transform.position);
        }
    }
}
