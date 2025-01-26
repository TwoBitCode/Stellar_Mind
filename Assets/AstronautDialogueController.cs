using System.Collections;
using UnityEngine;
using TMPro;

public class AstronautDialogueController : MonoBehaviour
{
    public TMP_Text lioText; // Reference to Lio's TextMesh Pro
    public TMP_Text mayaText; // Reference to Maya's TextMesh Pro
    public GameObject lioBubble; // Reference to Lio's speech bubble
    public GameObject mayaBubble; // Reference to Maya's speech bubble
    public GameObject endButton; // Reference to the button that appears after dialogue ends
    public AudioSource audioSource; // AudioSource for playing narration
    public AudioClip[] dialogueAudio; // Array of audio clips
    public string[] lioDialogue; // Dialogue lines for Lio
    public string[] mayaDialogue; // Dialogue lines for Maya

    private int currentLine = 0;

    void Start()
    {
        // Make sure the button is initially hidden
        if (endButton != null)
        {
            endButton.SetActive(false);
        }

        // Start the dialogue automatically
        if (dialogueAudio.Length > 0 && audioSource != null)
        {
            StartCoroutine(PlayDialogue());
        }
        else
        {
            Debug.LogError("Dialogue audio or audio source is missing!");
        }
    }

    IEnumerator PlayDialogue()
    {
        while (currentLine < dialogueAudio.Length) // Play all lines in the audio array
        {
            if (currentLine % 2 == 0)
            {
                // Lio speaks
                lioBubble.SetActive(true); // Show Lio's bubble
                mayaBubble.SetActive(false); // Hide Maya's bubble
                lioText.text = lioDialogue[currentLine / 2];
                audioSource.clip = dialogueAudio[currentLine];
            }
            else
            {
                // Maya speaks
                mayaBubble.SetActive(true); // Show Maya's bubble
                lioBubble.SetActive(false); // Hide Lio's bubble
                mayaText.text = mayaDialogue[currentLine / 2];
                audioSource.clip = dialogueAudio[currentLine];
            }

            // Play the audio clip
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + 1f); // Wait for the audio to finish

            // Move to the next line
            currentLine++;
        }

        // Clear texts and hide bubbles after the dialogue ends
        lioText.text = "";
        mayaText.text = "";
        lioBubble.SetActive(false);
        mayaBubble.SetActive(false);

        // Show the end button
        if (endButton != null)
        {
            endButton.SetActive(true);
        }
    }
}
