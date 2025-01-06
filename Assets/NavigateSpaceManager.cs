using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NavigateSpaceManager : MonoBehaviour
{
    public static NavigateSpaceManager Instance; // Singleton instance

    public Node startNode; // The starting node
    public GameObject player; // Player object
    private Node currentNode; // The node the player is currently on
    private Stack<Node> pathStack = new Stack<Node>(); // For reverse path validation
    private void Start()
    {
        StartNavigation();
    }

    private void Awake()
    {
        Instance = this;
    }

    public void StartNavigation()
    {
        if (startNode == null)
        {
            Debug.LogError("Start Node is not assigned in NavigateSpaceManager!");
            return;
        }

        // Set the current node and move the player to the starting position
        currentNode = startNode;
        player.transform.position = startNode.transform.position;
        Debug.Log($"Player moved to start node: {startNode.name} at position {startNode.transform.position}");

        // Highlight valid neighbors for the initial position
        UpdateClickableNodes();
    }




    public void OnNodeClicked(Node clickedNode)
    {
        Debug.Log($"Clicked Node: {clickedNode.name}");
        if (IsValidMove(clickedNode))
        {
            Debug.Log($"Valid Move to {clickedNode.name}");
            MoveToNode(clickedNode);
        }
        else
        {
            Debug.Log($"Invalid Move to {clickedNode.name}");
        }
    }

    private bool IsValidMove(Node newNode)
    {
        if (currentNode == null)
        {
            Debug.LogError("currentNode is null!");
            return false;
        }

        if (currentNode.neighbors == null || currentNode.neighbors.Length == 0)
        {
            Debug.LogWarning($"Node '{currentNode.name}' has no neighbors.");
            return false;
        }

        return System.Array.Exists(currentNode.neighbors, node => node == newNode) && !newNode.isRestricted;
    }



    private void MoveToNode(Node newNode)
    {
        // Move the player
        StartCoroutine(MovePlayerToNode(newNode));

        // Update current node
        currentNode = newNode;

        // Update clickable nodes
        UpdateClickableNodes();
    }

    private System.Collections.IEnumerator MovePlayerToNode(Node newNode)
    {
        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = newNode.transform.position;
        float duration = 0.5f; // Adjust speed of movement
        float elapsed = 0f;

        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = endPosition;
    }

    private void UpdateClickableNodes()
    {
        // Disable all nodes
        foreach (Node node in Object.FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            node.Highlight(false);
        }

        // Enable only neighbors of the current node
        foreach (Node neighbor in currentNode.neighbors)
        {
            if (!neighbor.isRestricted)
            {
                neighbor.Highlight(true);
            }
        }
    }

    public void ShowPath(Node[] path)
    {
        StartCoroutine(AnimatePath(path));
    }

    private System.Collections.IEnumerator AnimatePath(Node[] path)
    {
        foreach (Node node in path)
        {
            node.Highlight(true);
            yield return new WaitForSeconds(0.5f); // Adjust highlight duration
            node.Highlight(false);
        }
    }

    public void ReversePath(Node[] path)
    {
        pathStack = new Stack<Node>(path);
    }

    public bool ValidateReversePath(Node clickedNode)
    {
        if (pathStack.Count > 0 && pathStack.Peek() == clickedNode)
        {
            pathStack.Pop();
            return true;
        }
        return false;
    }
}
