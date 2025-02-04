using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Audio")]
    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip shakeSound; // Sound for the robot shaking
    public AudioClip explodeSound; // Sound for the robot exploding

    private int currentLineIndex = 0;
    [Header("Transitions")]
    public PanelTransitionManager panelTransitionManager; // Reference to the transition manager

    private void Start()
    {
        int lastPlayedStage = GameProgressManager.Instance?.GetLastPlayedStage() ?? 0;

        if (lastPlayedStage > 0)
        {
            // **Skip the intro and activate the correct stage panel**
            Debug.Log($"Skipping intro, starting at stage {lastPlayedStage}");

            introPanel.SetActive(false);
            workspacePanel.SetActive(true);

            // **Ensure we activate the correct panel for last played stage**
            EquipmentRecoveryGameManager.Instance?.ActivateStagePanel(lastPlayedStage);

            StartCoroutine(ShowRobotBeforeTurningBlack());
        }
        else
        {
            // **Normal intro flow for first-time play**
            startButton.onClick.AddListener(OnStartButtonClicked);
            introPanel.SetActive(true);
            workspacePanel.SetActive(false);
            robotPartsParent.SetActive(false);
            PlayShakeSoundLoop();
            InitializeDialogue();
        }
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

    private void OnStartButtonClicked()
    {
        // Cancel any existing Invokes
        CancelInvoke();

        // Proceed with starting the intro
        startButton.interactable = false;
        startButton.gameObject.SetActive(false);

        ShowNextDialogueLine();
        PlayShakeSoundLoop();
        robotAnimator.SetTrigger("Shake");

        Invoke(nameof(HideFullRobotAndShowParts), partsScatterDelay);
        Invoke(nameof(SlidePartsOut), partsScatterDelay);
        Invoke(nameof(PlayExplodeSound), partsScatterDelay);
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

    private void PlayShakeSoundLoop()
    {
        if (audioSource != null && shakeSound != null)
        {
            audioSource.clip = shakeSound;
            audioSource.loop = true; // Loop the shake sound
            audioSource.Play();
        }
    }

    private void PlayExplodeSound()
    {
        if (audioSource != null && explodeSound != null)
        {
            audioSource.loop = false; // Ensure the explode sound doesn't loop
            audioSource.PlayOneShot(explodeSound);
        }
    }

    private void StopAllSounds()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Stop all sounds immediately
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

    private bool transitionStarted = false;

    private void TransitionToWorkspace()
    {
        if (transitionStarted) return; // Prevent multiple transitions
        transitionStarted = true;

        // Stop all sounds immediately before transitioning
        StopAllSounds();

        // Transition to the workspace panel
        panelTransitionManager.TransitionPanels(introPanel, workspacePanel);

        // Ensure the workspace start button is visible so the player can begin the stage
        if (EquipmentRecoveryUIManager.Instance != null && EquipmentRecoveryUIManager.Instance.workspaceStartButton != null)
        {
            EquipmentRecoveryUIManager.Instance.workspaceStartButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowRobotBeforeTurningBlack()
    {
        Debug.Log("Showing the robot for memory phase before turning black...");

        fullRobot.SetActive(true);
        robotPartsParent.SetActive(false);

        yield return new WaitForSeconds(5f); // Keep the robot visible for 5 seconds

        Debug.Log("Turning the robot black now.");
        EquipmentRecoveryGameManager.Instance?.StartCoroutine(EquipmentRecoveryGameManager.Instance.DelayedTurnBlack());
    }


}
