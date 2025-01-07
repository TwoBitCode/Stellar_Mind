using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateSpaceManager : MonoBehaviour
{
    public static NavigateSpaceManager Instance; // Singleton instance

    [Header("Navigation Settings")]
    public Node startNode; // The starting node in the navigation graph
    public GameObject player; // The player object that moves between nodes

    [Header("Animation Settings")]
    [SerializeField] private float moveDuration = 0.5f; // Duration to move between nodes
    [SerializeField] private float highlightDuration = 0.5f; // Duration each node stays highlighted
    [SerializeField] private AnimationCurve movementCurve; //for non-linear movement
    private Node currentNode; // The node the player is currently on
    private Stack<Node> pathStack = new Stack<Node>(); // Stack for reverse path validation

    private void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of NavigateSpaceManager detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartNavigation();
    }

    // Starts the navigation process by placing the player at the starting node
    public void StartNavigation()
    {
        if (startNode == null)
        {
            Debug.LogError("Start Node is not assigned in NavigateSpaceManager!");
            return;
        }

        // Set the player's initial position
        currentNode = startNode;
        player.transform.position = startNode.transform.position;
        Debug.Log($"Player moved to start node: {startNode.name} at position {startNode.transform.position}");

        // Highlight valid neighbors for the starting node
        UpdateClickableNodes();
    }

    // Handles logic when a node is clicked by the player
    public void OnNodeClicked(Node clickedNode)
    {
        Debug.Log($"Clicked Node: {clickedNode.name}");
        if (IsValidMove(clickedNode))
        {
            MoveToNode(clickedNode);
        }
        else
        {
            Debug.Log($"Invalid move to {clickedNode.name}");
        }
    }

    // Checks if moving to the specified node is valid
    private bool IsValidMove(Node newNode)
    {
        if (currentNode == null)
        {
            Debug.LogError("Current node is null!");
            return false;
        }

        if (currentNode.neighbors == null || currentNode.neighbors.Length == 0)
        {
            Debug.LogWarning($"Node '{currentNode.name}' has no neighbors.");
            return false;
        }

        // Check if the new node is a valid neighbor and not restricted
        return System.Array.Exists(currentNode.neighbors, node => node == newNode) && !newNode.isRestricted;
    }

    // Moves the player to the specified node
    private void MoveToNode(Node newNode)
    {
        // Move the player to the new node over time
        StartCoroutine(MovePlayerToNode(newNode));

        // Update the current node to the new node
        currentNode = newNode;

        // Update the clickable nodes for the new position
        UpdateClickableNodes();
    }

    // Coroutine to smoothly move the player to the specified node
    private IEnumerator MovePlayerToNode(Node newNode)
    {
        Vector3 startPosition = player.transform.position; // Starting position of the player
        Vector3 endPosition = newNode.transform.position;  // Target position for the player

        float elapsedTime = 0f; // Time elapsed since movement began

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration; // Normalized time (0 to 1)
            float smoothT = movementCurve != null ? movementCurve.Evaluate(t) : t; // Use curve or linear

            // Interpolate the player's position
            player.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        player.transform.position = endPosition; // Ensure the player ends at the exact position
    }

    // Highlights the neighbors of the current node and disables others
    private void UpdateClickableNodes()
    {
        // Disable all nodes
        foreach (Node node in Object.FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            node.Highlight(false);
        }

        // Highlight neighbors of the current node
        foreach (Node neighbor in currentNode.neighbors)
        {
            if (!neighbor.isRestricted)
            {
                neighbor.Highlight(true);
            }
        }
    }

    // Shows a path by highlighting nodes in order
    public void ShowPath(Node[] path)
    {
        StartCoroutine(AnimatePath(path));
    }

    // Coroutine to animate highlighting nodes along a path
    private IEnumerator AnimatePath(Node[] path)
    {
        foreach (Node node in path)
        {
            node.Highlight(true); // Highlight the node
            yield return new WaitForSeconds(highlightDuration); // Use serialized highlight duration
            node.Highlight(false); // Remove the highlight
        }
    }

    // Stores a path in reverse order for validation
    public void ReversePath(Node[] path)
    {
        pathStack = new Stack<Node>(path);
    }

    // Validates if the clicked node is the correct one in the reverse path
    public bool ValidateReversePath(Node clickedNode)
    {
        if (pathStack.Count > 0 && pathStack.Peek() == clickedNode)
        {
            pathStack.Pop(); // Remove the validated node
            return true;
        }
        return false;
    }
}
