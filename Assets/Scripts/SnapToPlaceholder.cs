using UnityEngine;
using UnityEngine.EventSystems;

public class SnapToPlaceholder : MonoBehaviour, IDropHandler
{
    public string correctPartTag; // Tag for the correct part (e.g., "Arm", "Leg", etc.)

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop triggered!"); // Log to confirm method is called

        // Check if something is being dragged
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            Debug.Log($"Dropped object tag: {droppedObject.tag}"); // Log the tag of the dropped object
            if (droppedObject.CompareTag(correctPartTag))
            {
                // Snap the part to the placeholder
                droppedObject.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                Debug.Log("Correct part dropped and snapped!");
            }
            else
            {
                Debug.Log("Incorrect part dropped!");
            }
        }
    }
}
