using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors; // Connected neighbors
    public bool isRestricted; // Whether this node is restricted

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"Node {name} is missing a SpriteRenderer!");
        }
    }

    // Change the color of the node
    public void SetColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    // Handle click events
    private void OnMouseDown()
    {
        if (isRestricted)
        {
            Debug.Log($"Node {name} is restricted and cannot be clicked.");
            return;
        }

        if (NavigateSpaceManager.Instance != null)
        {
            NavigateSpaceManager.Instance.OnNodeClicked(this);
        }
        else
        {
            Debug.LogError("NavigateSpaceManager instance is missing!");
        }
    }
}
