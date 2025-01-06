using UnityEngine;

public class RadialStructureWithLines : MonoBehaviour
{
    public GameObject circlePrefab; // Prefab for circles
    public Material lineMaterial;   // Material for the lines
    public float firstLayerRadius = 3f;   // Radius for the first layer
    public float secondLayerRadius = 1.5f; // Radius for the second layer
    public int firstLayerBranches = 4;    // Number of branches in the first layer
    public int secondLayerBranches = 2;   // Number of sub-branches per branch in the second layer

    void Start()
    {
        GenerateStructure();
    }

    void GenerateStructure()
    {
        // Create the central circle
        GameObject center = Instantiate(circlePrefab, transform.position, Quaternion.identity, transform);
        center.name = "Center Circle";

        // Create a parent container for the first layer
        GameObject firstLayerParent = new GameObject("First Layer");
        firstLayerParent.transform.SetParent(transform);

        // Generate the first layer
        CreateFirstLayer(center.transform.position, firstLayerParent.transform);
    }

    void CreateFirstLayer(Vector3 centerPosition, Transform parent)
    {
        for (int i = 0; i < firstLayerBranches; i++)
        {
            float angle = i * Mathf.PI * 2 / firstLayerBranches; // Evenly space branches
            Vector3 position = centerPosition + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * firstLayerRadius;

            // Create the first-layer circle
            GameObject firstLayerCircle = Instantiate(circlePrefab, position, Quaternion.identity, parent);
            firstLayerCircle.name = $"Layer 1 - Circle {i + 1}";

            // Draw a line between the center and this circle
            DrawLine(centerPosition, position, parent);

            // Create a parent container for the second layer
            GameObject secondLayerParent = new GameObject($"Layer 1 - Circle {i + 1} Sub-Circles");
            secondLayerParent.transform.SetParent(firstLayerCircle.transform);

            // Generate the second layer for this branch
            CreateSecondLayer(position, angle, secondLayerParent.transform);
        }
    }

    void CreateSecondLayer(Vector3 branchPosition, float branchAngle, Transform parent)
    {
        for (int i = 0; i < secondLayerBranches; i++)
        {
            float spreadAngle = Mathf.PI / 4; // Adjust spacing
            float offsetAngle = branchAngle + (i - 0.5f) * spreadAngle; // Spread out the sub-circles
            Vector3 position = branchPosition + new Vector3(Mathf.Cos(offsetAngle), Mathf.Sin(offsetAngle), 0) * secondLayerRadius;

            // Create the second-layer circle
            GameObject secondLayerCircle = Instantiate(circlePrefab, position, Quaternion.identity, parent);
            secondLayerCircle.name = $"Layer 2 - Circle {i + 1}";

            // Draw a line between the branch and this sub-circle
            DrawLine(branchPosition, position, parent);
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Transform parent)
    {
        // Create a new GameObject for the line
        GameObject lineObject = new GameObject("Line");

        // Add a LineRenderer component
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set the line properties
        lineRenderer.material = lineMaterial; // Assign the line material
        lineRenderer.startWidth = 0.05f;      // Thickness at the start
        lineRenderer.endWidth = 0.05f;        // Thickness at the end
        lineRenderer.positionCount = 2;       // Two points: start and end
        lineRenderer.SetPosition(0, start);   // Start of the line
        lineRenderer.SetPosition(1, end);     // End of the line

        // Parent the line to the graph structure
        lineObject.transform.SetParent(parent);

        // Optional: Reset local position
        lineObject.transform.localPosition = Vector3.zero;
    }
}
