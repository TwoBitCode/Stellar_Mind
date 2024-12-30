using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public event Action<string> OnDialogueUpdated; // Event to update UI
    public event Action OnDialogueEnded; // Event triggered when dialogue ends

    private string[] dialogueLines;
    private int currentLineIndex;

    public void InitializeDialogue(string[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;

        if (dialogueLines.Length > 0)
        {
            UpdateDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void DisplayNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            UpdateDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void UpdateDialogue()
    {
        OnDialogueUpdated?.Invoke(dialogueLines[currentLineIndex]); // Trigger UI update
    }

    private void EndDialogue()
    {
        OnDialogueEnded?.Invoke(); // Trigger dialogue end
    }
}
