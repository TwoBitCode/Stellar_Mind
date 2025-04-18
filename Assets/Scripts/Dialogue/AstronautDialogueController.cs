using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Added for Button support

public class AstronautDialogueController : MonoBehaviour
{
    public TMP_Text lioText;
    public TMP_Text mayaText;
    public GameObject lioBubble;
    public GameObject mayaBubble;
    public GameObject endButton;
    public AudioSource audioSource;
    public AudioClip[] dialogueAudio;
    public string[] lioDialogue;
    public string[] mayaDialogue;

    [Header("Scene Management")]
    public string nextSceneName; // Set in Inspector
    public bool enableLandingShake = false; // Determines if shake effect happens

    private UILandingShakeEffect shakeEffect;
    private int currentLine = 0;

    [Header("Skip Button")]
    public Button skipButton; // The button that always appears to skip everything

    void Start()
    {
        if (endButton != null)
        {
            endButton.SetActive(false);
        }

        // Ensure Skip Button is active and ready to skip
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(true); // Always visible
            skipButton.onClick.AddListener(SkipDialogue);
        }
        else
        {
            Debug.LogError("Skip Button is not assigned in the Inspector!");
        }

        // Find shake effect if available
        shakeEffect = FindFirstObjectByType<UILandingShakeEffect>();

        if (dialogueAudio.Length > 0 && audioSource != null)
        {
            StartCoroutine(PlayDialogue());
        }
        else
        {
            Debug.LogError("Dialogue audio or audio source is missing!");
        }
    }

    IEnumerator TypeText(TMP_Text textComponent, string dialogueLine)
    {
        textComponent.text = ""; // Clear text before typing
        foreach (char letter in dialogueLine.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(0.05f); // Adjust speed here (lower = faster)
        }
    }

    IEnumerator PlayDialogue()
    {
        while (currentLine < dialogueAudio.Length)
        {
            if (currentLine % 2 == 0)
            {
                lioBubble.SetActive(true);
                mayaBubble.SetActive(false);
                lioText.text = "";

                audioSource.clip = dialogueAudio[currentLine];
                audioSource.Play(); // Start speaking with the text
                yield return StartCoroutine(TypeText(lioText, lioDialogue[currentLine / 2]));
            }
            else
            {
                mayaBubble.SetActive(true);
                lioBubble.SetActive(false);
                mayaText.text = "";

                audioSource.clip = dialogueAudio[currentLine];
                audioSource.Play(); // Start speaking with the text
                yield return StartCoroutine(TypeText(mayaText, mayaDialogue[currentLine / 2]));
            }

            yield return new WaitForSeconds(audioSource.clip.length + 1f);
            currentLine++;
        }

        // End dialogue
        lioText.text = "";
        mayaText.text = "";
        lioBubble.SetActive(false);
        mayaBubble.SetActive(false);

        if (endButton != null)
        {
            endButton.SetActive(true);
        }
    }


    public void ContinueToNextScene()
    {
        StartCoroutine(ShakeAndChangeScene());
    }

    private IEnumerator ShakeAndChangeScene()
    {
        if (enableLandingShake && shakeEffect != null)
        {
            Debug.Log("Starting shake effect before scene transition...");
            shakeEffect.StartShake();
            yield return new WaitForSeconds(2f); // Wait for shake
        }
        else
        {
            Debug.Log("Skipping shake effect: Either disabled or not found.");
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"Transitioning to scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set in the Inspector!");
        }
    }

    // New method to skip dialogue and instantly move to the next scene
    public void SkipDialogue()
    {
        Debug.Log("Skipping dialogue and moving to next scene.");
        SceneManager.LoadScene(nextSceneName);
    }
}
