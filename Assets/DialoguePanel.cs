using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class DialoguePanel : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Text to display dialogues
    public string[] dialogues; // Array of dialogue lines
    public float typingSpeed = 0.05f; // Speed for typing effect
    public GameObject startButton; // Reference to the start button (hide initially)

    private int currentDialogueIndex = 0;
    private Action onDialogueComplete;
    private bool isTyping = false;

    void Start()
    {
        if (startButton != null)
        {
            startButton.SetActive(false); // Hide start button initially
        }
    }

    public void StartDialogue(Action onComplete)
    {
        onDialogueComplete = onComplete;
        currentDialogueIndex = 0;
        gameObject.SetActive(true);
        DisplayNextDialogue();
    }

    private void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            StartCoroutine(TypeDialogue(dialogues[currentDialogueIndex]));
            currentDialogueIndex++;
        }
        else
        {
            ShowStartButton();
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        // Automatically move to the next dialogue after 1 second
        yield return new WaitForSeconds(1f);
        DisplayNextDialogue();
    }

    public void OnClick()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogues[currentDialogueIndex - 1];
            isTyping = false;
        }
        else
        {
            DisplayNextDialogue();
        }
    }

    private void ShowStartButton()
    {
        if (startButton != null)
        {
            startButton.SetActive(true); // Show the start button
        }
    }
}
