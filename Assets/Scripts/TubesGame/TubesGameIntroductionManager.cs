using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class TubesGameIntroductionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panel; // Intro panel
    [SerializeField] private GameObject dialoguePanel; // Dialogue panel (if not part of `panel`)
    [SerializeField] private TextMeshProUGUI dialogueText; // Text for dialogue
    [SerializeField] private GameObject startButton; // Button for starting the game
    [SerializeField] private GameObject tubePrefab; // Tube prefab for flying effect

    [Header("Introduction Settings")]
    [SerializeField] private string[] dialogueLines; // Dialogue lines for astronaut
    [SerializeField] private float typingSpeed = 0.05f; // Speed of typing effect
    [SerializeField] private int numberOfTubes = 10; // Number of tubes to spawn
    [SerializeField] private AudioSource flyingSound; // Reference to the AudioSource
    private Action onComplete;

    [Header("Dialogue Audio")]
    [SerializeField] private AudioSource dialogueAudioSource; // NEW: separate audio for dialogue
    [SerializeField] private AudioClip[] girlDialogueAudioClips; // Girl voice lines
    [SerializeField] private AudioClip[] boyDialogueAudioClips;  // Boy voice lines
    private string selectedCharacter; // "Girl" or "Boy"


    public void PlayIntroduction(Action onIntroductionComplete)
    {
        selectedCharacter = GameProgressManager.Instance?.playerProgress?.selectedCharacter ?? "Girl"; // Default to Girl

        onComplete = onIntroductionComplete;

        // If returning from the map, SKIP INTRO
        if (GameProgressManager.Instance.playerProgress.lastPlayedStage > 0)
        {
            Debug.Log("Skipping introduction because player is returning.");
            SkipIntroduction();
            return;
        }

        // Otherwise, play the introduction normally
        startButton.SetActive(false);
        TriggerFlyingTubes();
        StartCoroutine(PlayDialogueSequence());
    }


    public void SkipIntroduction()
    {
        Debug.Log("Skipping introduction UI.");

        // Hide all intro UI elements
        if (panel != null) panel.gameObject.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false); // Hide dialogue panel
        if (startButton != null) startButton.SetActive(false); // Hide the start button

        // Ensure `DialoguePanelIntro` is also hidden if it exists
        GameObject dialogueIntroPanel = GameObject.Find("DialoguePanelIntro");
        if (dialogueIntroPanel != null) dialogueIntroPanel.SetActive(false);

        // Stop any intro sounds
        if (flyingSound != null && flyingSound.isPlaying)
        {
            flyingSound.Stop();
        }

        // Notify GameManager to start the stage immediately
        onComplete?.Invoke();
    }


    private IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            PlayCurrentDialogueAudio(i); // <-- NEW: Play audio first
            yield return StartCoroutine(TypeDialogue(dialogueLines[i])); // Then type text
            yield return new WaitForSeconds(1f); // Pause between lines
        }

        yield return new WaitForSeconds(1f);
        startButton.SetActive(true);
    }

    private IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = ""; // Clear text
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Typing effect
        }
    }

    private void TriggerFlyingTubes()
    {
        if (flyingSound != null)
        {
            flyingSound.Play(); // Play sound once for all tubes
        }

        for (int i = 0; i < numberOfTubes; i++)
        {
            GameObject tube = Instantiate(tubePrefab, panel);
            RectTransform rectTransform = tube.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector2(
                UnityEngine.Random.Range(-panel.rect.width / 2, panel.rect.width / 2),
                UnityEngine.Random.Range(-panel.rect.height / 2, panel.rect.height / 2)
            );

            tube.AddComponent<TubeFlyAwayUI>();
        }
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Transitioning to game...");

        startButton.SetActive(false); // Hide the Start button
        panel.gameObject.SetActive(false); // Hide the intro panel
        dialoguePanel.gameObject.SetActive(false); // Hide the dialogue panel

        if (dialogueAudioSource != null && dialogueAudioSource.isPlaying)
        {
            dialogueAudioSource.Stop();
            Debug.Log("Stopped dialogue voice on Start Button click.");
        }

        // Stop the introduction flying sound
        if (flyingSound != null && flyingSound.isPlaying)
        {
            flyingSound.Stop();
        }

        onComplete?.Invoke(); // Notify GameManager to show stage instructions
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
