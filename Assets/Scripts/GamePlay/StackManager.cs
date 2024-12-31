using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] private GameObject stackElementPrefab; // Prefab for stack elements
    [SerializeField] private Transform stack; // Parent container for stack elements

    private GameObject[] stackElements;

    public void GenerateStackElements(int numObjects)
    {
        stackElements = new GameObject[numObjects];

        for (int i = 0; i < numObjects; i++)
        {
            stackElements[i] = Instantiate(stackElementPrefab, stack);
        }
    }
    public void MoveElementsToStack(GameObject[] gridElements)
    {
        for (int i = 0; i < gridElements.Length; i++)
        {
            DraggableItem draggableItem = gridElements[i].GetComponentInChildren<DraggableItem>();

            if (draggableItem != null)
            {
                Debug.Log($"Moving DraggableItem from Grid index {i} to Stack index {i}");

                // Move only the tube (child) to the stack
                draggableItem.transform.SetParent(stackElements[i].transform);

                // Update the drag logic for allowing tubes to return to the grid
                draggableItem.ParentAfterDrag = gridElements[i].transform;

                Debug.Log($"Stack index {i} now has {stackElements[i].transform.childCount} children.");
            }
            else
            {
                Debug.LogError($"Grid index {i}: DraggableItem not found. Please check prefab or setup.");
            }
        }

        // Confirm all stack elements after movement
        for (int i = 0; i < stackElements.Length; i++)
        {
            Debug.Log($"Stack index {i} has {stackElements[i].transform.childCount} children after move.");
        }
    }



    public bool IsOrderCorrect(Color[] initialColors)
    {
        for (int i = 0; i < stackElements.Length; i++)
        {
            // Check if there are any children in the stack slot
            if (stackElements[i].transform.childCount == 0)
            {
                Debug.LogError($"Stack index {i}: No child objects found in the stack slot.");
                return false;
            }

            // Get the DraggableItem inside the stack slot
            DraggableItem draggableItem = stackElements[i].GetComponentInChildren<DraggableItem>();

            if (draggableItem == null)
            {
                Debug.LogError($"Stack index {i}: No DraggableItem found.");
                return false;
            }

            // Compare colors
            Color expectedColor = initialColors[i];
            Color actualColor = draggableItem.Image.color;

            Debug.Log($"Stack index {i}: Expected Color = {expectedColor}, Actual Color = {actualColor}");

            if (!ColorsMatch(expectedColor, actualColor))
            {
                Debug.LogError($"Mismatch at index {i}: Expected {expectedColor}, Found {actualColor}");
                return false;
            }
        }

        Debug.Log("All colors match correctly!");
        return true;
    }

    // Helper method for color comparison (accounting for floating-point precision issues)
    private bool ColorsMatch(Color color1, Color color2)
    {
        return Mathf.Approximately(color1.r, color2.r) &&
               Mathf.Approximately(color1.g, color2.g) &&
               Mathf.Approximately(color1.b, color2.b);
    }


    private bool CompareColors(Color color1, Color color2, float tolerance = 0.01f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }


    public void ClearElements()
    {
        foreach (Transform child in stack)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Stack elements cleared.");
    }




    public void ResetStack()
    {
        ClearElements();
        stackElements = null;
        Debug.Log("Stack has been reset.");
    }

}
