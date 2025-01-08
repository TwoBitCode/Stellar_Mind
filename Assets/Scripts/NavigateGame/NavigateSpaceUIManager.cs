using System.Collections;
using UnityEngine;
using TMPro;

public class NavigateSpaceUIManager : MonoBehaviour
{
    public static NavigateSpaceUIManager Instance;

    [Header("UI Elements")]
    //public TextMeshProUGUI missionNameText; // Text field for displaying the mission name
    public TextMeshProUGUI missionInstructionText; // Text field for displaying the mission instructions
    public GameObject instructionPanel; // Panel containing mission details
    public GameObject startButton; // Reference to the Start button

    [Header("Highlight Settings")]
    [SerializeField] private float startDelay = 2.0f; // Delay before highlighting starts
    [SerializeField] private float highlightSpeed = 1.0f; // Speed of path highlighting (in seconds)
    [SerializeField] private Color highlightColor = Color.yellow; // Color used to highlight trajectory nodes

    private Coroutine highlightCoroutine;
    private Node[] currentTrajectoryPath; // Store the current mission's trajectory path

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the panel and button are hidden at the start
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }

    // Displays the instruction panel with mission details
    public void ShowMissionDetails(string missionName, string instruction, Node[] trajectoryPath)
    {
        currentTrajectoryPath = trajectoryPath;

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }

        //if (missionNameText != null)
        //{
        //    missionNameText.text = missionName;
        //}

        if (missionInstructionText != null)
        {
            missionInstructionText.text = instruction;
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }

    // Called by the Start button
    public void OnStartButtonClicked()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // Delay the start of highlighting using the serialized startDelay value
        if (currentTrajectoryPath != null)
        {
            StartCoroutine(DelayedHighlightStart(currentTrajectoryPath, startDelay));
        }
    }

    // Starts highlighting the trajectory path after a delay
    private IEnumerator DelayedHighlightStart(Node[] trajectoryPath, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartHighlightingPath(trajectoryPath);
    }

    // Starts highlighting the trajectory path
    public void StartHighlightingPath(Node[] trajectoryPath)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        highlightCoroutine = StartCoroutine(HighlightTrajectory(trajectoryPath));
    }

    // Coroutine for sequentially highlighting trajectory nodes
    private IEnumerator HighlightTrajectory(Node[] trajectoryPath)
    {
        foreach (var node in trajectoryPath)
        {
            if (node != null)
            {
                node.SetColor(highlightColor); // Highlight the node
                yield return new WaitForSeconds(highlightSpeed);
            }
        }

        NodeManager.Instance.ResetHighlight(); // Reset after highlighting
    }

    // Hides the instruction panel
    public void HideMissionDetails()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
}
