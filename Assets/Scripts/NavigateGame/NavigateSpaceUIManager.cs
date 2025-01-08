using System.Collections;
using UnityEngine;
using TMPro;

public class NavigateSpaceUIManager : MonoBehaviour
{
    public static NavigateSpaceUIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI missionInstructionText;

    [Header("Highlight Settings")]
    public float highlightSpeed = 1.0f;
    public Color highlightColor = Color.yellow;

    private Coroutine highlightCoroutine;

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
    }

    public void SetMissionDetails(string missionName, string instruction)
    {
        if (missionNameText != null)
        {
            missionNameText.text = missionName;
        }

        if (missionInstructionText != null)
        {
            missionInstructionText.text = instruction;
        }
    }

    // Start highlighting a trajectory path
    public void StartHighlightingPath(Node[] trajectoryPath)
    {
        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(HighlightTrajectory(trajectoryPath));
    }

    // Sequentially highlight the trajectory path
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

        // Reset highlights after the trajectory guide ends
        NodeManager.Instance.ResetHighlight();
    }
}
