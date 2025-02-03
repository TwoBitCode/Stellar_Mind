using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy }

    public static CharacterSelectionManager Instance { get; private set; }

    [SerializeField] private GameObject boyButton;
    [SerializeField] private GameObject girlButton;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueBubble;
    [SerializeField] private Button nextButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crashDialogue;
    [TextArea(3, 5)] public string crashDialogueText;

    private string[] dialogueLines;
    private int currentLine = 0;
    private bool isTyping = false;

    private void Awake()
    {
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
        if (!string.IsNullOrEmpty(GameProgressManager.Instance.playerProgress.selectedCharacter))
        {
            Debug.Log($"Character already selected: {GameProgressManager.Instance.playerProgress.selectedCharacter}. Skipping selection.");
            TransitionToGame();
            return;
        }
       // FindFirstObjectByType<ShakeUI>().StartShake();

        Debug.Log("No valid character found. Showing selection.");
        boyButton.SetActive(false);
        girlButton.SetActive(false);
        dialogueBubble.SetActive(true);

        dialogueLines = crashDialogueText.Split('\n');
        nextButton.onClick.AddListener(ShowNextLine);
        nextButton.gameObject.SetActive(true);

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (isTyping) return; // Prevent skipping mid-typing

        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeText(dialogueLines[currentLine]));
            currentLine++;
        }
        else
        {
            // End of dialogue - Show character selection buttons
            dialogueText.text = "";
            dialogueBubble.SetActive(false);
            nextButton.gameObject.SetActive(false);
            boyButton.SetActive(true);
            girlButton.SetActive(true);
        }
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear text before typing

        // **Play audio at the start of the line**
        if (crashDialogue != null && audioSource != null)
        {
            audioSource.clip = crashDialogue;
            audioSource.Play();
        }

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // Adjust speed here
        }

        isTyping = false;
    }

    public void SelectGirlAstronaut()
    {
        GameProgressManager.Instance.playerProgress.selectedCharacter = "Girl";
        GameProgressManager.Instance.SaveProgress();
        Debug.Log("Girl Astronaut selected!");
        TransitionToGame();
    }

    public void SelectBoyAstronaut()
    {
        GameProgressManager.Instance.playerProgress.selectedCharacter = "Boy";
        GameProgressManager.Instance.SaveProgress();
        Debug.Log("Boy Astronaut selected!");
        TransitionToGame();
    }

    private void TransitionToGame()
    {
        SceneManager.LoadScene("GameMapScene-V");
    }
}
