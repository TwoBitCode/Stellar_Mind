using System.Collections.Generic;
using UnityEngine;

public class RadialStructureWithLayerConnections : MonoBehaviour
{
    [Header("Circle and Line Settings")]
    public GameObject circlePrefab; // Prefab for the circles
    public Material lineMaterial;   // Material for the lines
    [SerializeField] private float lineWidth = 0.05f; // Line thickness

    [Header("Layer Settings")]
    [SerializeField] private float firstLayerRadius = 3f;   // Radius for the first layer
    [SerializeField] private float secondLayerRadius = 1.5f; // Radius for the second layer
    [SerializeField] private int firstLayerBranches = 4;    // Number of branches in the first layer
    [SerializeField] private int secondLayerBranches = 2;   // Number of sub-branches per branch in the second layer
    [SerializeField] private float secondLayerSpreadAngle = Mathf.PI / 4; // Spread angle for second-layer branches
    [SerializeField] private float branchCenteringFactor = 2f; // Factor for centering sub-branches around the parent branch

    private List<GameObject> firstLayerCircles = new List<GameObject>();
    private List<List<GameObject>> secondLayerCircles = new List<List<GameObject>>();

    void Start()
    {
        GenerateStructure();
    }

    // Generates the entire radial structure
    void GenerateStructure()
    {
        // Create the central circle
        GameObject center = Instantiate(circlePrefab, transform.position, Quaternion.identity, transform);
        center.name = "Center Circle";

        // Generate the first layer
        CreateFirstLayer(center.transform.position);

        // Connect second-layer circles between branches
        ConnectSecondLayerInCycle();
    }

    // Generates the first layer of circles around the center
    void CreateFirstLayer(Vector3 centerPosition)
    {
        for (int i = 0; i < firstLayerBranches; i++)
        {
            float angle = i * Mathf.PI * 2 / firstLayerBranches; // Evenly space branches
            Vector3 position = centerPosition + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * firstLayerRadius;

            // Create the first-layer circle
            GameObject firstLayerCircle = Instantiate(circlePrefab, position, Quaternion.identity, transform);
            firstLayerCircle.name = $"Layer 1 - Circle {i + 1}";
            firstLayerCircles.Add(firstLayerCircle);

            // Draw a line between the center and this circle
            DrawLine(centerPosition, position, transform);

            // Generate the second layer for this branch
            CreateSecondLayer(position, angle, i);
        }

        // Connect all circles in the first layer
        ConnectLayerCircles(firstLayerCircles);
    }

    // Generates the second layer of circles for each branch
    void CreateSecondLayer(Vector3 branchPosition, float branchAngle, int branchIndex)
    {
        List<GameObject> branchCircles = new List<GameObject>();

        for (int i = 0; i < secondLayerBranches; i++)
        {
            // Calculate the offset angle for each sub-branch
            float offsetAngle = branchAngle + (i - (secondLayerBranches - 1) / branchCenteringFactor) * secondLayerSpreadAngle;

            // Determine the position of the sub-circle
            Vector3 position = branchPosition + new Vector3(Mathf.Cos(offsetAngle), Mathf.Sin(offsetAngle), 0) * secondLayerRadius;

            // Create the second-layer circle
            GameObject secondLayerCircle = Instantiate(circlePrefab, position, Quaternion.identity, transform);
            secondLayerCircle.name = $"Layer 2 - Branch {branchIndex + 1} - Circle {i + 1}";
            branchCircles.Add(secondLayerCircle);

            // Draw a line between the branch and this sub-circle
            DrawLine(branchPosition, position, transform);
        }

        secondLayerCircles.Add(branchCircles);

        // Connect all circles within the same branch
        ConnectLayerCircles(branchCircles);
    }

    // Connects all circles in a single layer
    void ConnectLayerCircles(List<GameObject> circles)
    {
        for (int i = 0; i < circles.Count; i++)
        {
            Vector3 start = circles[i].transform.position;
            Vector3 end = circles[(i + 1) % circles.Count].transform.position; // Connect to the next circle, looping back to the first
            DrawLine(start, end, transform);
        }
    }

    // Connects second-layer circles in a proper cycle
    void ConnectSecondLayerInCycle()
    {
        for (int i = 0; i < firstLayerBranches; i++)
        {
            List<GameObject> currentBranch = secondLayerCircles[i];
            List<GameObject> nextBranch = secondLayerCircles[(i + 1) % firstLayerBranches]; // Wrap to the first branch

            // Connect the last circle of the current branch to the first circle of the next branch
            GameObject lastCircle = currentBranch[currentBranch.Count - 1];
            GameObject firstCircle = nextBranch[0];

            DrawLine(lastCircle.transform.position, firstCircle.transform.position, transform);
        }
    }

    // Draws a line between two points
    void DrawLine(Vector3 start, Vector3 end, Transform parent)
    {
        // Create a new GameObject for the line
        GameObject lineObject = new GameObject("Line");

        // Add a LineRenderer component
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the line properties
        lineRenderer.material = lineMaterial; // Assign the line material
        lineRenderer.startWidth = lineWidth;  // Set the start width
        lineRenderer.endWidth = lineWidth;    // Set the end width
        lineRenderer.positionCount = 2;       // Two points: start and end
        lineRenderer.SetPosition(0, start);   // Start of the line
        lineRenderer.SetPosition(1, end);     // End of the line

        // Parent the line to the graph structure
        lineObject.transform.SetParent(parent);

        // Optional: Reset local position
        lineObject.transform.localPosition = Vector3.zero;
    }
}
