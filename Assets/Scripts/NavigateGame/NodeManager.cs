using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    private List<Node> allNodes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Collect all nodes in the scene at the start
        allNodes = new List<Node>(Object.FindObjectsByType<Node>(FindObjectsSortMode.None));
    }

    public void ResetAllNodeStates()
    {
        foreach (var node in allNodes)
        {
            node.isRestricted = false;
            node.SetColor(Color.white); // Reset to default color
        }
    }

    // Reset only the highlight (visual color) for all nodes
    public void ResetHighlight()
    {
        foreach (var node in allNodes)
        {
            node.SetColor(Color.white); // Reset to default color
        }
    }
    public void SetRestrictedNodes(Node[] restrictedNodes)
    {
        foreach (var node in restrictedNodes)
        {
            if (node != null)
            {
                node.isRestricted = true;
                node.SetColor(Color.red); // Set restricted nodes to red for visual feedback
            }
        }
    }
}
