using System.Collections;
using UnityEngine;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy } // Enum for character types

    public static CharacterSelectionManager Instance { get; private set; }

    private CharacterDataManager characterDataManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private GameObject boyButton; // Button for boy astronaut
    [SerializeField] private GameObject girlButton; // Button for girl astronaut
    [SerializeField] private TMP_Text dialogueText; // Text for dialogue
    [SerializeField] private GameObject dialogueBubble; // Bubble for the dialogue text
    [SerializeField] private AudioSource audioSource; // AudioSource for dialogue
    [SerializeField] private AudioClip crashDialogue; // Audio clip for the crash dialogue
    [TextArea(3, 5)] public string crashDialogueText; // Editable text for dialogue in Inspector
    [SerializeField] private float timeBetweenLines = 2f; // Time to wait between lines

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize CharacterDataManager
        characterDataManager = new CharacterDataManager(); // Replace with FindObjectOfType if needed
    }

    private void Start()
    {
        // Initially hide the buttons and keep the bubble visible
        boyButton.SetActive(false);
        girlButton.SetActive(false);
        dialogueBubble.SetActive(true);

        // Start the crash dialogue sequence
        StartCoroutine(PlayCrashDialogue());
    }

    private IEnumerator PlayCrashDialogue()
    {
        // Split the dialogue text into lines
        string[] lines = crashDialogueText.Split('\n');

        // Play each line
        foreach (string line in lines)
        {
            dialogueText.text = line; // Display the current line
            if (crashDialogue != null && audioSource != null)
            {
                audioSource.clip = crashDialogue;
                audioSource.Play();
            }
            yield return new WaitForSeconds(timeBetweenLines); // Wait before showing the next line
        }

        // Clear the dialogue text
        dialogueText.text = "";

        // Hide the dialogue bubble
        dialogueBubble.SetActive(false);

        // Show the buttons
        boyButton.SetActive(true);
        girlButton.SetActive(true);
    }

    public void SelectGirlAstronaut()
    {
        characterDataManager.SaveCharacterSelection(CharacterType.Girl);
        Debug.Log("Girl Astronaut selected.");
        TransitionToGame();
    }

    public void SelectBoyAstronaut()
    {
        characterDataManager.SaveCharacterSelection(CharacterType.Boy);
        Debug.Log("Boy Astronaut selected.");
        TransitionToGame();
    }

    private void TransitionToGame()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadNextScene();
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned to CharacterSelectionManager!");
        }
    }

    public CharacterType GetSelectedCharacter()
    {
        return characterDataManager.LoadCharacterSelection();
    }
}
