using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;  // If using TextMeshPro
using UnityEngine.EventSystems;


public class CharacterIntroduction : MonoBehaviour
{
    [SerializeField] private GameObject characterImage; // Assign in the Inspector
    [SerializeField] private TMP_Text dialogueText;     // Assign in the Inspector
    [SerializeField] private string[] dialogue;         // The dialogue lines
    private int dialogueIndex = 0;
    [SerializeField] private GameObject arrowButton;    // The arrow button to continue
    [SerializeField] private GameObject backgroundImage; // The background image for the text
    [SerializeField] private float typingSpeed = 0.05f;

    [SerializeField] private GameObject NextScene;

    [SerializeField] private bool EnableNextSceneButton = true;

    [SerializeField] private float delay;

    void Start()
    {
        dialogueIndex = 0;
        characterImage.SetActive(true); // Show the character when the scene starts
        arrowButton.SetActive(false);   // Hide the arrow initially
        backgroundImage.SetActive(true); // Ensure background is visible initially
        NextScene.SetActive(false);
        StartCoroutine(DisplayDialogue());
    }

    // Display the dialogue and wait for the user to click the arrow
    IEnumerator DisplayDialogue()
    {
        dialogueText.text = ""; // Clear any previous text
        foreach (char letter in dialogue[dialogueIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // After finishing the current dialogue line, show the arrow button
        if ((dialogueIndex == dialogue.Length - 1) && EnableNextSceneButton)
        {
            NextScene.SetActive(true);
            //yield return new WaitForSeconds(delay);

            // Wait for the NextScene button to be clicked
            yield return StartCoroutine(WaitForNextSceneButton());
        }
        else
        {
            arrowButton.SetActive(true);
            // Wait for the user to click the arrow button
            yield return new WaitUntil(() => ButtonClicked(arrowButton));  // Left-click detection
        }

        // Hide the arrow button and move to the next dialogue line
        arrowButton.SetActive(false);
        dialogueIndex++;

        if (dialogueIndex < dialogue.Length)
        {
            StartCoroutine(DisplayDialogue()); // Start showing the next dialogue line
        }
        else
        {
            // End of introduction, you can hide the character or transition
            characterImage.SetActive(false); // Hide character after intro
            backgroundImage.SetActive(false); // Hide the background image after the dialogue is done
            dialogueText.text = ""; // Clear any previous text
        }
    }

    // Check if the arrow button is clicked
    private bool ButtonClicked(GameObject button)
    {
        // Use the EventSystem to detect if the arrow button was clicked
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == button)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); // Clear selection
            return true;
        }
        return false;
    }

    private IEnumerator WaitForNextSceneButton()
    {
        // Enable the NextScene button
        NextScene.SetActive(true);

        // Wait for the NextScene button to be clicked
        yield return new WaitUntil(() => EventSystem.current.currentSelectedGameObject == NextScene);

        // Allow the button's OnClick function to execute normally
        UnityEngine.UI.Button button = NextScene.GetComponent<UnityEngine.UI.Button>();
        button.onClick.Invoke(); // This triggers the assigned OnClick event function
    }
}
