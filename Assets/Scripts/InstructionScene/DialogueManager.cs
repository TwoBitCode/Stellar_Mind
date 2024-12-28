using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText; // Text field for dialogue
    [SerializeField] private string[] dialogueLines; // Array of dialogue lines
    [SerializeField] private float typingSpeed = 0.05f; // Speed of typing effect

    private int currentLineIndex = 0; // Tracks the current dialogue line
    private bool isTyping = false; // Flag to check if typing is in progress

    public delegate void DialogueComplete();
    public event DialogueComplete OnDialogueComplete;

    private void Start()
    {
        StartCoroutine(TypeDialogue(dialogueLines[currentLineIndex]));

    }

    public void DisplayNextLine()
    {
        if (isTyping) return; // Prevent skipping while typing

        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeDialogue(dialogueLines[currentLineIndex]));
        }
        else
        {
            OnDialogueComplete?.Invoke(); // Notify that dialogue is complete
        }
    }

    private IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
