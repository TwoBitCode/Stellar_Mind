using UnityEngine;

public class DialogueSetup : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUIHandler dialogueUIHandler;

    private string[] exampleDialogue = new string[]
    {
        "ברוך הבא למשימה החשובה שלנו!",
        "נצטרך לתקן את מערכת השמש ולהחזיר אותה למצב תקין.",
        "נצטרך להשתמש בזיכרון ובריכוז כדי לעמוד במשימות שלנו.",
        "אם נצליח, נוכל להציל את מערכת השמש ולחזור הביתה!"
    };

    private void Start()
    {
        dialogueUIHandler.Setup(dialogueManager);
        dialogueManager.InitializeDialogue(exampleDialogue);
    }
}
