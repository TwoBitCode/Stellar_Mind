using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;      // Connected nodes
    public bool isRestricted = false; // If the node is restricted in the current level

    public void OnMouseDown()
    {
        Debug.Log($"{gameObject.name} clicked!");
        NavigateSpaceManager.Instance.OnNodeClicked(this);
    }


    public void Highlight(bool highlight)
    {
        // Change color or scale to indicate clickable nodes
        GetComponent<SpriteRenderer>().color = highlight ? Color.green : Color.white;
    }
}
