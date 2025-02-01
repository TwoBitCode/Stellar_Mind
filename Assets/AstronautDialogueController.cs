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

    private int currentLine = 0;

    void Start()
    {
        if (endButton != null)
        {
            endButton.SetActive(false);
        }

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
        while (currentLine < dialogueAudio.Length)
        {
            if (currentLine % 2 == 0)
            {
                lioBubble.SetActive(true);
                mayaBubble.SetActive(false);
                lioText.text = lioDialogue[currentLine / 2];
                audioSource.clip = dialogueAudio[currentLine];
            }
            else
            {
                mayaBubble.SetActive(true);
                lioBubble.SetActive(false);
                mayaText.text = mayaDialogue[currentLine / 2];
                audioSource.clip = dialogueAudio[currentLine];
            }

            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + 1f);

            currentLine++;
        }

        // סיום הדיאלוג
        lioText.text = "";
        mayaText.text = "";
        lioBubble.SetActive(false);
        mayaBubble.SetActive(false);

        if (endButton != null)
        {
            endButton.SetActive(true);
        }
    }

    // פונקציה שמופעלת על ידי הלחצן
    public void ContinueToNextScene()
    {
        SceneManager.LoadScene("SelectPlayerScene-vivi"); // שינוי שם הסצנה לסצנה הבאה בפועל
    }
}
