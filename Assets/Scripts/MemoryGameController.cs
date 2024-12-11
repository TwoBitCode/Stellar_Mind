using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;  // Required for coroutines

public class GameManager : MonoBehaviour
{
    [Tooltip("Default time delay in seconds before shuffling")]
    [SerializeField] private float DEFAULT_SHUFFLE_DELAY = 2f; // Default time delay in seconds before shuffling
    [Tooltip("Text shown when the order is incorrect")]
    [SerializeField] private string INCORRECT_ORDER_TEXT = "Incorrect Order!";
    [Tooltip("Text shown when the order is correct")]
    [SerializeField] private string CORRECT_ORDER_TEXT = "Correct Order!";
    private const string GAME_OVER_SCENE_NAME = "GameOverScene"; // Game Over scene name

    [Header("Grid Settings")]
    public GameObject gridElementPrefab;  // Prefab for the grid element (containing the capsule)
    public Transform grid;  // The parent container for the grid elements
    [Tooltip("Default number of capsules to create")]
    [SerializeField] public int numObjects = 4;  // Number of capsules to create
    private GameObject[] gridElements;  // To store references to the grid elements
    private Color[] initialColors;  // To store the initial order of capsule colors

    [Header("Stack Settings")]
    public GameObject stackElementPrefab;  // Prefab for the stack element (containing the capsule)
    public Transform stack; // The parent container for the stack grid elements
    private GameObject[] stackElements; // To store references to the stack grid elements

    [Header("UI Elements")]
    public TextMeshProUGUI resultText; // UI Text element to display the result
    public Button restartButton; // Restart button
    public Button checkAnswerButton; // Check Answer button
    public Button returnToMenuButton; // Return to Menu button

    private void Start()
    {
        GenerateGridElements();
        GenerateStackElements();
        // Start the shuffle after the specified delay
        StartCoroutine(ShuffleAfterDelay(DEFAULT_SHUFFLE_DELAY));

        // Ensure Restart button is hidden at the start
        restartButton.gameObject.SetActive(false);
    }

    void GenerateGridElements()
    {
        gridElements = new GameObject[numObjects];
        initialColors = new Color[numObjects];

        for (int i = 0; i < numObjects; i++)
        {
            GameObject gridElement = Instantiate(gridElementPrefab, grid);  // Create gridElement
            DraggableItem draggableItem = gridElement.GetComponentInChildren<DraggableItem>();
            Image capsuleImage = draggableItem.image;  // Access the Image component of the capsule

            // Set a random color for the capsule inside the grid element
            Color randomColor = GetRandomColor();
            capsuleImage.color = randomColor;
            initialColors[i] = randomColor;  // Store the initial color for later comparison

            gridElements[i] = gridElement;  // Store reference to the gridElement
        }
    }

    Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);  // Return a random color
    }

    public void ShuffleCapsules()
    {
        // Get all capsules (DraggableItem components) in the grid
        DraggableItem[] capsules = grid.GetComponentsInChildren<DraggableItem>();

        // Create a list of their current parents
        Transform[] originalParents = new Transform[capsules.Length];
        for (int i = 0; i < capsules.Length; i++)
        {
            originalParents[i] = capsules[i].transform.parent; // Store the original gridElement parent
        }

        // Shuffle the capsules by reassigning their parents randomly
        for (int i = 0; i < capsules.Length; i++)
        {
            int randomIndex = Random.Range(0, capsules.Length);

            // Swap the capsules' parent containers
            Transform tempParent = capsules[i].transform.parent;
            capsules[i].transform.SetParent(capsules[randomIndex].transform.parent);
            capsules[randomIndex].transform.SetParent(tempParent);
        }

        // Update the parentAfterDrag for each capsule
        for (int i = 0; i < capsules.Length; i++)
        {
            capsules[i].parentAfterDrag = capsules[i].transform.parent;
        }
    }

    public void CheckAnswer()
    {
        // Compare the colors of the capsules in the stack with the original order
        for (int i = 0; i < gridElements.Length; i++)
        {
            DraggableItem capsule = gridElements[i].GetComponentInChildren<DraggableItem>();
            if (capsule == null || capsule.image.color != initialColors[i])
            {
                // Display result
                resultText.text = INCORRECT_ORDER_TEXT;

                // Freeze capsules and show Restart button
                FreezeCapsules(true);

                checkAnswerButton.gameObject.SetActive(false);
                restartButton.gameObject.SetActive(true); // Show Restart button
                return;
            }
        }

        // If all colors match the original order
        resultText.text = CORRECT_ORDER_TEXT;
        restartButton.gameObject.SetActive(false); // Hide Restart button if correct
    }

    IEnumerator ShuffleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShuffleCapsules();

        // Move capsules to stack after shuffling
        MoveCapsulesToStack();
    }

    void GenerateStackElements()
    {
        stackElements = new GameObject[numObjects];

        for (int i = 0; i < numObjects; i++)
        {
            // Create stack grid elements (similar to grid elements)
            GameObject stackElement = Instantiate(stackElementPrefab, stack);
            stackElements[i] = stackElement;
        }
    }

    public void MoveCapsulesToStack()
    {
        DraggableItem[] capsules = grid.GetComponentsInChildren<DraggableItem>();

        for (int i = 0; i < capsules.Length; i++)
        {
            // Move each capsule to the corresponding stack grid element
            capsules[i].transform.SetParent(stackElements[i].transform);
            capsules[i].parentAfterDrag = stackElements[i].transform; // Update drag logic
        }
    }

    public void RestartGame()
    {
        // Clear the current grid and stack
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in stack)
        {
            Destroy(child.gameObject);
        }

        // Reset result message and hide Restart button
        resultText.text = "";
        restartButton.gameObject.SetActive(false);
        checkAnswerButton.gameObject.SetActive(true);

        // Generate new grid and stack with new colors
        GenerateGridElements();
        GenerateStackElements();

        // Unfreeze the capsules
        FreezeCapsules(false);

        // Shuffle and move capsules to the stack
        StartCoroutine(ShuffleAfterDelay(DEFAULT_SHUFFLE_DELAY));
    }

    public void FreezeCapsules(bool freeze)
    {
        // Freeze or unfreeze all capsules in the grid and stack
        foreach (Transform child in grid)
        {
            DraggableItem draggable = child.GetComponentInChildren<DraggableItem>();
            if (draggable != null)
            {
                draggable.enabled = !freeze; // Disable or enable the DraggableItem script
            }
        }

        foreach (Transform child in stack)
        {
            DraggableItem draggable = child.GetComponentInChildren<DraggableItem>();
            if (draggable != null)
            {
                draggable.enabled = !freeze; // Disable or enable the DraggableItem script
            }
        }
    }
}
