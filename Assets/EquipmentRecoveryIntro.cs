using UnityEngine;
using TMPro;

public class EquipmentRecoveryIntro : MonoBehaviour
{
    public GameObject dialoguePanel; // Intro panel with astronaut dialogue
    public TextMeshProUGUI dialogueText; // Dialogue text component
    public GameObject fullRobot; // Full robot before it breaks
    public GameObject robotPartsParent; // Parent object containing all parts
    public GameObject workspacePanel; // Workspace panel for the mini-game
    public Animator robotAnimator; // Animator for robot animations

    public float partsScatterDelay = 1.5f; // Delay after shaking before scattering parts
    public float transitionDelay = 3f; // Delay before switching to the workspace panel

    public void OnStartButtonClicked()
    {
        // Disable the dialogue button
        dialoguePanel.GetComponentInChildren<UnityEngine.UI.Button>().interactable = false;

        // Play the robot shaking animation
        robotAnimator.SetTrigger("Shake");

        // Hide the full robot and scatter parts after the shaking
        Invoke(nameof(HideFullRobotAndShowParts), partsScatterDelay);
        Invoke(nameof(ScatterParts), partsScatterDelay);
        Invoke(nameof(StartWorkspace), partsScatterDelay + transitionDelay);
    }

    private void HideFullRobotAndShowParts()
    {
        fullRobot.SetActive(false); // Hide the full robot
        robotPartsParent.SetActive(true); // Show the scattered parts
    }

    private void ScatterParts()
    {
        foreach (Transform part in robotPartsParent.transform)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-500, 500), // Adjust X range
                Random.Range(-300, 300), // Adjust Y range
                0 // Keep Z at 0
            );

            part.localPosition = randomPosition; // Move part to random position
        }
    }

    private void StartWorkspace()
    {
        dialoguePanel.SetActive(false); // Hide the intro panel
        workspacePanel.SetActive(true); // Show the workspace
    }
}
