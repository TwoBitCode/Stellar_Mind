using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public string nextSceneName; // ניתן לעריכה ב-Inspector
    public bool enableLandingShake = false; // בודק האם צריך רעידה

    private UILandingShakeEffect shakeEffect;
    private int currentLine = 0;

    void Start()
    {
        if (endButton != null)
        {
            endButton.SetActive(false);
        }

        // מחפש את הרעידה אם היא קיימת
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
        textComponent.text = ""; // Clear the text before typing
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
                audioSource.clip = dialogueAudio[currentLine];
                yield return StartCoroutine(TypeText(lioText, lioDialogue[currentLine / 2]));
            }
            else
            {
                mayaBubble.SetActive(true);
                lioBubble.SetActive(false);
                audioSource.clip = dialogueAudio[currentLine];
                yield return StartCoroutine(TypeText(mayaText, mayaDialogue[currentLine / 2]));
            }

            audioSource.Play();
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
            shakeEffect.StartShake(); // מפעיל רעידה
            yield return new WaitForSeconds(2f); // מחכה קצת כדי שהרעידה תהיה מורגשת
        }
        else
        {
            Debug.LogWarning("hake effect skipped: Either disabled or not found.");
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

}
