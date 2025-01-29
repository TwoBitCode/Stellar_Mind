using System.Collections;
using UnityEngine;
using TMPro;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy } // Enum for character types

    public static CharacterSelectionManager Instance { get; private set; }

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        string savedCharacter = GameProgressManager.Instance.GetPlayerProgress().selectedCharacter;

        if (!string.IsNullOrEmpty(savedCharacter) && (savedCharacter == "Boy" || savedCharacter == "Girl"))
        {
            Debug.Log($"Character already selected: {savedCharacter}. Skipping selection.");
            TransitionToGame();
            return;
        }

        Debug.Log("No valid character found. Showing selection.");
        boyButton.SetActive(false);
        girlButton.SetActive(false);
        dialogueBubble.SetActive(true);
        StartCoroutine(PlayCrashDialogue());
    }


    private IEnumerator PlayCrashDialogue()
    {
        string[] lines = crashDialogueText.Split('\n');

        foreach (string line in lines)
        {
            dialogueText.text = line;
            if (crashDialogue != null && audioSource != null)
            {
                audioSource.clip = crashDialogue;
                audioSource.Play();
            }
            yield return new WaitForSeconds(timeBetweenLines);
        }

        dialogueText.text = "";
        dialogueBubble.SetActive(false);
        boyButton.SetActive(true);
        girlButton.SetActive(true);
    }

    public void SelectGirlAstronaut()
    {
        GameProgressManager.Instance.GetPlayerProgress().selectedCharacter = "Girl";
        GameProgressManager.Instance.SaveProgress();
        Debug.Log("Girl Astronaut selected.");
        TransitionToGame();
    }

    public void SelectBoyAstronaut()
    {
        GameProgressManager.Instance.GetPlayerProgress().selectedCharacter = "Boy";
        GameProgressManager.Instance.SaveProgress();
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
}
