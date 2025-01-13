using UnityEngine;
using TMPro;

public class EquipmentRecoveryIntro : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject introPanel; // Entire panel for the intro (dialogue, robot shaking, etc.)
    public GameObject workspacePanel; // Panel for the workspace mini-game

    [Header("UI Elements")]
    public GameObject dialoguePanel; // Panel containing astronaut dialogue
    public TextMeshProUGUI dialogueText; // Text component for astronaut instructions
    public GameObject fullRobot; // Full robot before it breaks
    public GameObject robotPartsParent; // Parent object containing all parts
    public Animator robotAnimator; // Animator for robot animations
    public UnityEngine.UI.Button startButton; // Button to start the intro sequence

    [Header("Dialogue Settings")]
    [TextArea(2, 5)]
    public string[] dialogueLines; // Array of dialogue lines (editable in Inspector)
    public float textDisplayDuration = 2f; // Duration to display each dialogue line
    public string transitionDialogue = "Oh no!"; // Line to display before transitioning

    [Header("Timings")]
    public float partsScatterDelay = 1.5f; // Delay after shaking before scattering parts
    public float transitionDelay = 3f; // Delay before switching to the workspace panel

    [Header("Part Scatter Settings")]
    public float scatterDistance = 100f; // Max distance the parts should slide out
    public float scatterDuration = 0.5f; // Duration of the slide-out effect

    private int currentLineIndex = 0;

    private void Start()
    {
        // Ensure the start button is wired to its click event
        startButton.onClick.AddListener(OnStartButtonClicked);

        // Ensure only the intro panel is active at the start
        introPanel.SetActive(true);
        workspacePanel.SetActive(false);

        // Hide robot parts at the start
        robotPartsParent.SetActive(false);

        // Initialize the dialogue
        InitializeDialogue();
    }

    private void InitializeDialogue()
    {
        // Show the dialogue panel and the first line of text
        dialoguePanel.SetActive(true);

        if (dialogueLines.Length > 0)
        {
            dialogueText.text = dialogueLines[0]; // Set the first line
        }
        else
        {
            Debug.LogWarning("Dialogue lines are empty! Add dialogue in the Inspector.");
        }
    }

    public void OnStartButtonClicked()
    {
        // Disable the button to prevent multiple clicks
        startButton.interactable = false;

        // Start displaying dialogue and play the robot shaking animation
        ShowNextDialogueLine();
        robotAnimator.SetTrigger("Shake");

        // Schedule actions: Show parts, slide them out, display transition dialogue, and transition
        Invoke(nameof(HideFullRobotAndShowParts), partsScatterDelay);
        Invoke(nameof(SlidePartsOut), partsScatterDelay);
        Invoke(nameof(DisplayTransitionDialogue), partsScatterDelay + scatterDuration);
        Invoke(nameof(TransitionToWorkspace), partsScatterDelay + scatterDuration + transitionDelay);
    }

    private void ShowNextDialogueLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex]; // Update text
            currentLineIndex++;

            if (currentLineIndex < dialogueLines.Length)
            {
                Invoke(nameof(ShowNextDialogueLine), textDisplayDuration); // Schedule the next line
            }
        }
    }

    private void DisplayTransitionDialogue()
    {
        // Display the transition dialogue (e.g., "Oh no!")
        dialogueText.text = transitionDialogue;
    }

    private void HideFullRobotAndShowParts()
    {
        fullRobot.SetActive(false); // Hide the full robot
        robotPartsParent.SetActive(true); // Show the scattered parts
    }

    private void SlidePartsOut()
    {
        foreach (Transform part in robotPartsParent.transform)
        {
            // Calculate a direction vector pointing away from the robot
            Vector3 slideDirection = (part.position - fullRobot.transform.position).normalized;

            // Calculate the target position for the part
            Vector3 targetPosition = part.localPosition + (slideDirection * scatterDistance);

            // Smoothly move the part to the target position
            StartCoroutine(SlidePart(part, targetPosition));
        }
    }

    private System.Collections.IEnumerator SlidePart(Transform part, Vector3 targetPosition)
    {
        Vector3 initialPosition = part.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < scatterDuration)
        {
            part.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / scatterDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        part.localPosition = targetPosition; // Ensure the part reaches the final position
    }

    private void TransitionToWorkspace()
    {
        introPanel.SetActive(false); // Hide the intro panel
        workspacePanel.SetActive(true); // Show the workspace panel

        // Start the workspace instructions
        EquipmentRecoveryUIManager.Instance?.StartWorkspaceInstructions();
    }

}
