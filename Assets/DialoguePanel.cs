using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialoguePanel : MonoBehaviour
{
    public GameObject dialoguePanel; // Reference to the dialogue panel
    public TextMeshProUGUI dialogueText; // Text to display dialogues
    public string[] dialogues; // Array of dialogue lines
    public float typingSpeed = 0.05f; // Speed for typing effect

    private int currentDialogueIndex = 0;
    private Action onDialogueComplete;

    public void StartDialogue(Action onComplete)
    {
        onDialogueComplete = onComplete;
        currentDialogueIndex = 0;
        dialoguePanel.SetActive(true); // Ensure the panel is active
        DisplayNextDialogue();
    }

    public void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
            currentDialogueIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void OnClick()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            DisplayNextDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false); // Deactivate the panel
        onDialogueComplete?.Invoke();
    }
}
