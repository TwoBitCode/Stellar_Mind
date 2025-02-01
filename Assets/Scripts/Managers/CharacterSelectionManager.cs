using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy } // Enum for character types

    public static CharacterSelectionManager Instance { get; private set; }

    [SerializeField] private GameObject boyButton;
    [SerializeField] private GameObject girlButton;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueBubble;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crashDialogue;
    [TextArea(3, 5)] public string crashDialogueText;
    [SerializeField] private float timeBetweenLines = 2f;

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
        // לבדוק אם כבר נבחרה דמות
        if (!string.IsNullOrEmpty(GameProgressManager.Instance.playerProgress.selectedCharacter))
        {
            Debug.Log($"Character already selected: {GameProgressManager.Instance.playerProgress.selectedCharacter}. Skipping selection.");
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
        GameProgressManager.Instance.playerProgress.selectedCharacter = "Girl";
        GameProgressManager.Instance.SaveProgress();
        Debug.Log(" Girl Astronaut selected!");
        TransitionToGame();
    }

    public void SelectBoyAstronaut()
    {
        GameProgressManager.Instance.playerProgress.selectedCharacter = "Boy";
        GameProgressManager.Instance.SaveProgress();
        Debug.Log(" Boy Astronaut selected!");
        TransitionToGame();
    }

    private void TransitionToGame()
    {
        SceneManager.LoadScene("GameMapScene-V"); // משנה לסצנה הבאה
    }
}
