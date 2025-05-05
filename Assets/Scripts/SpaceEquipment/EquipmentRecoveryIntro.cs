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
    public AudioSource robotAudioSource; // NEW: For robot shake and explode sounds
    public AudioSource dialogueAudioSource; // NEW: For dialogues (girl/boy)

    public AudioClip shakeSound; // (no change)
    public AudioClip explodeSound; // (no change)


    private string selectedCharacter; // Who the player chose: "Girl" or "Boy"

    private int currentLineIndex = 0;
    [Header("Transitions")]
    public PanelTransitionManager panelTransitionManager; // Reference to the transition manager


    [Header("Dialogue Audio Clips")]
    public AudioClip[] girlDialogueAudioClips; // Audio clips for girl lines
    public AudioClip[] boyDialogueAudioClips;  // Audio clips for boy lines
    [Header("Transition Dialogue Audio")]
    public AudioClip girlTransitionDialogueAudioClip; // Girl's transition voice
    public AudioClip boyTransitionDialogueAudioClip;  // Boy's transition voice



    private void Start()
    {
        selectedCharacter = GameProgressManager.Instance?.playerProgress?.selectedCharacter ?? "Girl"; // Default to Girl
        int lastPlayedStage = GameProgressManager.Instance?.GetLastPlayedStage() ?? 0;

        if (lastPlayedStage > 0)
        {
            Debug.Log($"Skipping intro, starting at stage {lastPlayedStage}");
            introPanel.SetActive(false);
            workspacePanel.SetActive(true);
            EquipmentRecoveryGameManager.Instance?.ActivateStagePanel(lastPlayedStage);
            // Instruction panel will be shown inside StartStage() and wait for time selection
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
        dialoguePanel.SetActive(true);

        if (dialogueLines.Length > 0)
        {
            dialogueText.text = dialogueLines[0]; // Set the first dialogue line

            // Play the first dialogue audio immediately
            if (selectedCharacter == "Boy")
            {
                if (boyDialogueAudioClips != null && boyDialogueAudioClips.Length > 0)
                {
                    PlayDialogueAudio(boyDialogueAudioClips[0]);
                }
            }
            else // Assume Girl
            {
                if (girlDialogueAudioClips != null && girlDialogueAudioClips.Length > 0)
                {
                    PlayDialogueAudio(girlDialogueAudioClips[0]);
                }
            }
        }
        else
        {
            Debug.LogWarning("Dialogue lines are empty! Add dialogue in the Inspector.");
        }
    }


    private void OnStartButtonClicked()
    {
        CancelInvoke();

        startButton.interactable = false;
        startButton.gameObject.SetActive(false);

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
            dialogueText.text = dialogueLines[currentLineIndex]; // Show text

            // Play the correct voice based on selected character
            if (selectedCharacter == "Boy")
            {
                if (boyDialogueAudioClips != null && currentLineIndex < boyDialogueAudioClips.Length)
                {
                    PlayDialogueAudio(boyDialogueAudioClips[currentLineIndex]);
                }
            }
            else // Assume "Girl"
            {
                if (girlDialogueAudioClips != null && currentLineIndex < girlDialogueAudioClips.Length)
                {
                    PlayDialogueAudio(girlDialogueAudioClips[currentLineIndex]);
                }
            }

            currentLineIndex++;

            if (currentLineIndex < dialogueLines.Length)
            {
                Invoke(nameof(ShowNextDialogueLine), textDisplayDuration);
            }
        }
    }


    private void PlayShakeSoundLoop()
    {
        if (robotAudioSource != null && shakeSound != null)
        {
            robotAudioSource.clip = shakeSound;
            robotAudioSource.loop = true;
            robotAudioSource.Play();
        }
    }

    private void PlayExplodeSound()
    {
        if (robotAudioSource != null && explodeSound != null)
        {
            robotAudioSource.loop = false;
            robotAudioSource.PlayOneShot(explodeSound);
        }
    }

    private void StopAllSounds()
    {
        if (robotAudioSource != null)
        {
            robotAudioSource.Stop();
        }
        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }
    }


    private void DisplayTransitionDialogue()
    {
        dialogueText.text = transitionDialogue; // Show "Oh no!" or similar

        // Play correct transition voice based on selected character
        if (selectedCharacter == "Boy")
        {
            if (boyTransitionDialogueAudioClip != null)
            {
                PlayDialogueAudio(boyTransitionDialogueAudioClip);
            }
        }
        else // Assume girl
        {
            if (girlTransitionDialogueAudioClip != null)
            {
                PlayDialogueAudio(girlTransitionDialogueAudioClip);
            }
        }
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
    private void PlayDialogueAudio(AudioClip clip)
    {
        if (dialogueAudioSource != null && clip != null)
        {
            dialogueAudioSource.Stop();
            dialogueAudioSource.loop = false;
            dialogueAudioSource.clip = clip;
            dialogueAudioSource.Play();
        }
    }


}
