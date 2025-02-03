using UnityEngine;
using UnityEngine.EventSystems;

public class SnapToPlaceholder : MonoBehaviour, IDropHandler
{
    public string correctPartTag; // Tag for the correct part (e.g., "Arm", "Leg")

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop triggered!"); // Log to confirm method is called

        // Check if something is being dragged
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
           // Debug.Log($"Dropped object tag: {droppedObject.tag}"); // Log the tag of the dropped object
            if (droppedObject.CompareTag(correctPartTag))
            {
                droppedObject.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

               // Debug.Log("Correct part dropped and snapped!");
                EquipmentRecoveryGameManager.Instance?.PartPlacedCorrectly(droppedObject);

               // EquipmentRecoveryUIManager.Instance?.ShowFeedback("Correct!", Color.green);
                AudioFeedbackManager.Instance?.PlayCorrectSound();
            }
            else
            {
               // Debug.Log("Incorrect part dropped!");
                EquipmentRecoveryGameManager.Instance?.PartPlacedIncorrectly(droppedObject);

                //EquipmentRecoveryUIManager.Instance?.ShowFeedback("Try Again!", Color.red);
                AudioFeedbackManager.Instance?.PlayIncorrectSound();
            }

        }
    }
}
