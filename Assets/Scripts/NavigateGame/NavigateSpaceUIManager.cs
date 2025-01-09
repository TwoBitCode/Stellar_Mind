using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NavigateSpaceUIManager : MonoBehaviour
{
    public static NavigateSpaceUIManager Instance;

    [Header("UI Elements")]
    public GameObject instructionPanel;
    public GameObject startButton;
    public TextMeshProUGUI missionInstructionText;

    [Header("Highlight Settings")]
    [SerializeField] private float startDelay = 2.0f;
    [SerializeField] private float highlightSpeed = 1.0f;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Material glowMaterial;
    [SerializeField] private AudioClip highlightSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float highlightScaleFactor = 1.5f;
    [SerializeField] private float delayToNextStage = 2.0f; // Time before advancing to the next stage
    [SerializeField] private float delayBeforeNextMission = 2.0f;
    [SerializeField] private float targetHighlightDuration = 3.0f; // Duration to keep the target highlighted

    private Coroutine highlightCoroutine;
    private Node[] currentTrajectoryPath;
    private bool isPathShowing = false;
    [Header("Strategy Panel")]
    public GameObject strategyPanel; // The panel to show strategy tips
    public TextMeshProUGUI strategyText; // Text for the strategy message
    public Button continueButton; // Button to restart the mission

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

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }

    public void ShowMissionDetails(string missionName, string instruction, Node[] trajectoryPath)
    {
        currentTrajectoryPath = trajectoryPath;

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }

        if (missionInstructionText != null)
        {
            missionInstructionText.text = instruction; // Set the mission-specific instruction
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }


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

        if (currentTrajectoryPath != null)
        {
            AlienGuideManager.Instance.NotifyHighlightStart(); // Alien feedback
            isPathShowing = true;
            StartCoroutine(DelayedHighlightStart(currentTrajectoryPath, startDelay));
        }
    }

    private IEnumerator DelayedHighlightStart(Node[] trajectoryPath, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartHighlightingPath(trajectoryPath);
    }

    public void StartHighlightingPath(Node[] trajectoryPath)
    {
        if (trajectoryPath == null || trajectoryPath.Length == 0)
        {
            Debug.LogWarning("No trajectory path to highlight.");
            return;
        }

        Debug.Log("Starting to highlight the trajectory path...");
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }

        highlightCoroutine = StartCoroutine(HighlightTrajectory(trajectoryPath));
    }


    public void HighlightTargetNode(Node targetNode)
    {
        if (targetNode != null)
        {
            Debug.Log("Highlighting target node: " + targetNode.name);

            // Block interaction during highlighting
            isPathShowing = true;

            StartCoroutine(HighlightSingleNode(targetNode, () =>
            {
                // Enable interaction after highlighting
                isPathShowing = false;
                Debug.Log("Target highlighting complete. Player can now interact.");
            }));
        }
        else
        {
            Debug.LogWarning("Target node is null, cannot highlight.");
        }
    }



    private IEnumerator HighlightSingleNode(Node node, System.Action onComplete)
    {
        SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log("Applying highlight to node: " + node.name);
            Material originalMaterial = spriteRenderer.material;
            Vector3 originalScale = node.transform.localScale;
            Vector3 highlightedScale = originalScale * highlightScaleFactor;

            spriteRenderer.material = glowMaterial;
            spriteRenderer.material.SetColor("_EmissionColor", highlightColor);

            float elapsedTime = 0f;

            while (elapsedTime < targetHighlightDuration)
            {
                node.transform.localScale = Vector3.Lerp(originalScale, highlightedScale, Mathf.PingPong(elapsedTime * 2f, 1f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.material = originalMaterial;
            node.transform.localScale = originalScale;
            Debug.Log("Finished highlighting node: " + node.name);

            onComplete?.Invoke();
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on node: " + node.name);
            onComplete?.Invoke(); // Ensure callback is called even on error
        }
    }

    private IEnumerator HighlightTrajectory(Node[] trajectoryPath)
    {
        Debug.Log("Highlighting trajectory...");

        foreach (var node in trajectoryPath)
        {
            if (node != null)
            {
                Debug.Log($"Highlighting node: {node.name}");
                SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Material originalMaterial = spriteRenderer.material;
                    Vector3 originalScale = node.transform.localScale;
                    Vector3 highlightedScale = originalScale * highlightScaleFactor;

                    spriteRenderer.material = glowMaterial;
                    spriteRenderer.material.SetColor("_EmissionColor", highlightColor);

                    node.transform.localScale = highlightedScale;

                    if (audioSource != null && highlightSound != null)
                    {
                        audioSource.PlayOneShot(highlightSound);
                    }

                    yield return new WaitForSeconds(highlightSpeed);

                    // Reset to original appearance
                    spriteRenderer.material = originalMaterial;
                    node.transform.localScale = originalScale;
                }
            }
        }

        Debug.Log("Finished highlighting trajectory.");
        isPathShowing = false; // Allow interaction after highlighting
        AlienGuideManager.Instance.NotifyHighlightEnd();
        NodeManager.Instance.ResetHighlight(); // Reset highlights
    }

    public bool IsPathShowing()
    {
        var currentMission = SpaceMissionManager.Instance.missions[SpaceMissionManager.Instance.CurrentMissionIndex];

        // Prevent interaction during highlighting for all mission types
        if (currentMission.missionType == SpaceMission.MissionType.NavigateToTarget)
        {
            return isPathShowing; // This should be set during target highlighting
        }

        return isPathShowing;
    }


    public void HideMissionDetails()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
    public void DelayNextMissionUI(System.Action onComplete)
    {
        StartCoroutine(DelayBeforeNextMissionCoroutine(onComplete));
    }

    private IEnumerator DelayBeforeNextMissionCoroutine(System.Action onComplete)
    {
        yield return new WaitForSeconds(delayBeforeNextMission);
        onComplete?.Invoke();
    }
    public void ShowStrategyPanel(System.Action onContinue)
    {
        if (strategyPanel != null)
        {
            // Show the panel
            strategyPanel.SetActive(true);

            // Get and display the next strategy from AlienGuideManager
            string strategy = AlienGuideManager.Instance.GetNextStrategy();
            Debug.Log($"Showing strategy in panel: {strategy}");
            strategyText.text = strategy;

            // Update the alien text to match the strategy panel
            AlienGuideManager.Instance.UpdateAlienText("Think carefully! Review the strategy and press 'Continue' to restart.");

            // Assign the "Continue" button action dynamically
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                strategyPanel.SetActive(false); // Hide the panel
                onContinue?.Invoke(); // Trigger the restart logic
            });
        }
        else
        {
            Debug.LogWarning("Strategy panel is not assigned in the UI Manager!");
        }
    }
    public void ResetPathState()
    {
        isPathShowing = false;
    }


}
