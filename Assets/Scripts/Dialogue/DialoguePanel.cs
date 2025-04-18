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
    public AudioSource typingAudioSource; // AudioSource to play the typing sound
    public AudioClip typingSound; // Sound effect for each line of dialogue

    private int currentDialogueIndex = 0;
    private Action onDialogueComplete;
    //private bool isTyping = false;

    [Header("Dialogue Audio")]
    [SerializeField] private AudioSource dialogueAudioSource; // NEW: Separate audio for dialogue
    [SerializeField] private AudioClip[] girlDialogueAudioClips; // Girl voice clips
    [SerializeField] private AudioClip[] boyDialogueAudioClips;  // Boy voice clips
    private string selectedCharacter; // "Girl" or "Boy"


    void Start()
    {
        selectedCharacter = GameProgressManager.Instance?.playerProgress?.selectedCharacter ?? "Girl"; // Default to Girl

        if (startButton != null)
        {
            startButton.SetActive(false); // Hide start button initially
        }

        if (typingAudioSource == null)
        {
            Debug.LogWarning("Typing AudioSource is not assigned.");
        }

        // Check if we are resuming a later stage, and disable dialogue panel immediately!
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (playerProgress != null && playerProgress.lastPlayedStage > 0)
        {
            Debug.Log("Returning to a later stage, hiding dialogue panel.");
            gameObject.SetActive(false); //  Ensure the panel is turned off when returning
        }
    }


    public void StartDialogue(Action onComplete)
    {
        selectedCharacter = GameProgressManager.Instance?.playerProgress?.selectedCharacter ?? "Girl";

        onDialogueComplete = onComplete;
        currentDialogueIndex = 0;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Canvas dialogueCanvas = GetComponentInParent<Canvas>();
        if (dialogueCanvas != null && !dialogueCanvas.gameObject.activeSelf)
        {
            dialogueCanvas.gameObject.SetActive(true);
        }

        Debug.Log("Starting dialogue sequence.");
        StartCoroutine(AutoPlayDialogue());
    }

    private IEnumerator AutoPlayDialogue()
    {
        while (currentDialogueIndex < dialogues.Length)
        {
            PlayCurrentDialogueAudio(currentDialogueIndex);
            dialogueText.text = dialogues[currentDialogueIndex];

            currentDialogueIndex++;

            yield return new WaitForSeconds(3f); // Wait 2 seconds before next line
        }

        ShowStartButton();
    }

    private void ShowStartButton()
    {
        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }


    private void DisplayNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            PlayCurrentDialogueAudio(currentDialogueIndex); // Play matching voice

            dialogueText.text = dialogues[currentDialogueIndex]; // Instantly show full line
            currentDialogueIndex++;
        }
        else
        {
            ShowStartButton();
        }
    }


    private IEnumerator TypeDialogue(string dialogue)
    {

        dialogueText.text = dialogue; // Instantly show full dialogue text

        yield return new WaitForSeconds(1f); // Wait 1 second before moving to next dialogue
        DisplayNextDialogue();
    }


    public void OnClick()
    {
        DisplayNextDialogue(); // On click, move to next line
    }


    private void PlayTypingSound()
    {
        if (typingAudioSource != null && typingSound != null)
        {
            if (!typingAudioSource.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Typing AudioSource is disabled, enabling now.");
                typingAudioSource.gameObject.SetActive(true); // Ensure the AudioSource is active
            }

            typingAudioSource.PlayOneShot(typingSound);
        }
    }
    private void PlayCurrentDialogueAudio(int index)
    {
        if (dialogueAudioSource == null) return;

        dialogueAudioSource.Stop();
        dialogueAudioSource.loop = false;

        if (selectedCharacter == "Boy")
        {
            if (boyDialogueAudioClips != null && index < boyDialogueAudioClips.Length)
            {
                dialogueAudioSource.clip = boyDialogueAudioClips[index];
                dialogueAudioSource.Play();
            }
        }
        else // Assume Girl
        {
            if (girlDialogueAudioClips != null && index < girlDialogueAudioClips.Length)
            {
                dialogueAudioSource.clip = girlDialogueAudioClips[index];
                dialogueAudioSource.Play();
            }
        }
    }

}