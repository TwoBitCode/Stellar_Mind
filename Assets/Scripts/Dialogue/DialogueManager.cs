using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    // Event to update the UI with the current dialogue line
    public event Action<string> OnDialogueUpdated;

    // Event triggered when the dialogue ends
    public event Action OnDialogueEnded;

    private string[] dialogueLines; // Array to store dialogue lines
    private int currentLineIndex;   // Index of the current dialogue line

    // Initializes the dialogue with the provided lines
    public void InitializeDialogue(string[] lines)
    {
        // If there are no lines, end the dialogue immediately
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Dialogue lines are empty or null. Ending dialogue.");
            EndDialogue();
            return;
        }

        // Store the dialogue lines and reset the index
        dialogueLines = lines;
        currentLineIndex = 0;

        // Show the first line of dialogue
        UpdateDialogue();
    }

    // Displays the next line of dialogue
    public void DisplayNextLine()
    {
        // Move to the next line
        currentLineIndex++;

        // If there are more lines, update the dialogue
        if (currentLineIndex < dialogueLines.Length)
        {
            UpdateDialogue();
        }
        else
        {
            // Otherwise, end the dialogue
            EndDialogue();
        }
    }

    // Updates the UI with the current line of dialogue
    private void UpdateDialogue()
    {
        // Trigger the event to update the UI with the current line
        OnDialogueUpdated?.Invoke(dialogueLines[currentLineIndex]);
    }

    // Ends the dialogue and triggers the end event
    private void EndDialogue()
    {
        OnDialogueEnded?.Invoke();
    }
}
