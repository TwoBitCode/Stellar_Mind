using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject gridElementPrefab; // Prefab for grid elements
    [SerializeField] private Transform grid; // Parent container for grid elements
    [SerializeField] private Color[] tubeColors; // Array of colors for visualizing the tubes (adjustable from the inspector)

    private GameObject[] gridElements;
    private int[] originalTubeIDs; // Stores the original TubeIDs for logic
    private int[] shuffledTubeIDs; // Stores the shuffled TubeIDs for debugging or logging

    public GameObject[] GridElements => gridElements;
    public int[] OriginalTubeIDs => originalTubeIDs;

    public void GenerateGridElements(int numObjects)
    {
        // Initialize grid elements and TubeIDs
        gridElements = new GameObject[numObjects];
        originalTubeIDs = new int[numObjects];

        // Ensure there are enough colors assigned in the inspector
        if (tubeColors == null || tubeColors.Length < numObjects)
        {
            Debug.LogError($"Not enough colors assigned in the inspector! Assigned colors: {tubeColors.Length}, Required colors: {numObjects}");
            return;
        }

        // Shuffle colors to ensure they are randomized each play session
        Color[] randomizedColors = ShuffleColors(tubeColors, numObjects);

        for (int i = 0; i < numObjects; i++)
        {
            // Instantiate grid element as a child of the grid container
            GameObject gridElement = Instantiate(gridElementPrefab, grid);
            DraggableItem draggableItem = gridElement.GetComponentInChildren<DraggableItem>();

            if (draggableItem == null)
            {
                Debug.LogError($"DraggableItem component missing in GridElementPrefab at index {i}!");
                continue;
            }

            // Assign TubeID for logic
            draggableItem.TubeID = i;
            originalTubeIDs[i] = i;

            // Assign a randomized color for display
            draggableItem.Image.color = randomizedColors[i];

            // Add the grid element to the array
            gridElements[i] = gridElement;
        }

        Debug.Log($"Original Tube Order (TubeIDs): {string.Join(", ", originalTubeIDs)}");
    }

    // Helper method to shuffle colors
    private Color[] ShuffleColors(Color[] colors, int numObjects)
    {
        Color[] shuffledColors = new Color[numObjects];
        List<Color> colorPool = new List<Color>(colors);

        // Shuffle the list and pick the required number of colors
        for (int i = 0; i < numObjects; i++)
        {
            int randomIndex = Random.Range(0, colorPool.Count);
            shuffledColors[i] = colorPool[randomIndex];
            colorPool.RemoveAt(randomIndex); // Remove the selected color to avoid duplicates
        }

        return shuffledColors;
    }



    public void ShuffleGridElements()
    {
        shuffledTubeIDs = (int[])originalTubeIDs.Clone();

        for (int i = shuffledTubeIDs.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap TubeIDs in the shuffled array
            int temp = shuffledTubeIDs[i];
            shuffledTubeIDs[i] = shuffledTubeIDs[randomIndex];
            shuffledTubeIDs[randomIndex] = temp;
        }

        Debug.Log($"Shuffled Tube Order (TubeIDs): {string.Join(", ", shuffledTubeIDs)}");

        // Update gridElements to match the shuffled order
        GameObject[] shuffledGridElements = new GameObject[gridElements.Length];

        for (int i = 0; i < shuffledTubeIDs.Length; i++)
        {
            shuffledGridElements[i] = gridElements[shuffledTubeIDs[i]];
            shuffledGridElements[i].transform.SetSiblingIndex(i);
        }

        gridElements = shuffledGridElements;
    }

    public void ClearElements()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }
}
