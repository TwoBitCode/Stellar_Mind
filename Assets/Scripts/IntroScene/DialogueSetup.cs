using UnityEngine;

public class DialogueSetup : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueUIHandler dialogueUIHandler;

    private string[] exampleDialogue = new string[]
    {
        "���� ��� ������ ������ ����!",
        "����� ���� �� ����� ���� ������� ���� ���� ����.",
        "����� ������ ������� ������� ��� ����� ������� ����.",
        "�� �����, ���� ����� �� ����� ���� ������ �����!"
    };

    private void Start()
    {
        dialogueUIHandler.Setup(dialogueManager);
        dialogueManager.InitializeDialogue(exampleDialogue);
    }
}
