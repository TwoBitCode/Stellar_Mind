using System.Collections;
using UnityEngine;
using TMPro;

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
            missionInstructionText.text = instruction;
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
            StartCoroutine(HighlightSingleNode(targetNode));
        }
        else
        {
            Debug.LogWarning("Target node is null, cannot highlight.");
        }
    }


    private IEnumerator HighlightSingleNode(Node node)
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
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on node: " + node.name);
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
}
