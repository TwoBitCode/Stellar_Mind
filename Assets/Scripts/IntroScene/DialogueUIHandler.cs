using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUIHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text dialogueText; // Text field for displaying dialogue
    [SerializeField] private Button nextLineButton; // Button for showing the next line
    [SerializeField] private Button startGameButton; // Button for starting the game

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;

    private DialogueManager dialogueManager;

    private void Start()
    {
        startGameButton.gameObject.SetActive(false); // Hide the "Start Game" button initially
    }

    public void Setup(DialogueManager manager)
    {
        dialogueManager = manager;

        dialogueManager.OnDialogueUpdated += UpdateDialogueUI;
        dialogueManager.OnDialogueEnded += ShowStartGameButton;

        nextLineButton.onClick.AddListener(() => {
            PlayButtonClickSound();
            dialogueManager.DisplayNextLine();
        });

        startGameButton.onClick.AddListener(() => {
            PlayButtonClickSound();
            StartGame();
        });
    }

    private void UpdateDialogueUI(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    private void ShowStartGameButton()
    {
        nextLineButton.gameObject.SetActive(false); // Hide "Next Line" button
        startGameButton.gameObject.SetActive(true); // Show "Start Game" button
    }

    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void StartGame()
    {
        Debug.Log("Starting the game!");
        // Use SceneTransitionManager to transition to the game scene
    }
}
