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

    public void PlayIntroduction(Action onIntroductionComplete)
    {
        onComplete = onIntroductionComplete; // Store callback
        startButton.SetActive(false); // Hide Start button initially

        // Start flying tubes and dialogue
        TriggerFlyingTubes();
        StartCoroutine(PlayDialogueSequence());
    }

    private IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(TypeDialogue(dialogueLines[i]));
            yield return new WaitForSeconds(1f); // Pause between lines
        }

        // Wait for flying tubes to finish, then show the Start button
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
        dialoguePanel.gameObject.SetActive(false); // Hide the intro panel
        // Stop the introduction music
        if (flyingSound != null && flyingSound.isPlaying)
        {
            flyingSound.Stop();
        }

        onComplete?.Invoke(); // Notify GameManager to show stage instructions
    }

}
