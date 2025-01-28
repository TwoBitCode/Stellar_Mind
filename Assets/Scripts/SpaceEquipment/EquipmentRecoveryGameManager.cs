using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentRecoveryGameManager : MonoBehaviour
{
    public static EquipmentRecoveryGameManager Instance;

    [Header("Stage Settings")]
    public List<EquipmentRecoveryStage> stages;
    public List<GameObject> stagePanels;

    private int currentStageIndex = 0;
    private int correctPartsPlaced = 0;
    private HashSet<GameObject> placedParts = new HashSet<GameObject>();
    private HashSet<GameObject> penalizedParts = new HashSet<GameObject>(); // Tracks penalized objects

    [Header("UI Elements")]
    public GameObject gameOverPanel;

    public bool isInteractionAllowed = false; // Prevent dragging until allowed
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
        }
    }

    private EquipmentRecoveryStage CurrentStage => stages[currentStageIndex];

    public void StartStage()
    {
        Debug.Log("StartStage() called!");

        if (stages == null || stages.Count == 0)
        {
            Debug.LogError("stages list is empty!");
            return;
        }

        if (stagePanels == null || stagePanels.Count == 0)
        {
            Debug.LogError("stagePanels list is empty!");
            return;
        }

        if (currentStageIndex >= stages.Count)
        {
            Debug.LogError($"Invalid stage index: {currentStageIndex}. Stages count: {stages.Count}");
            return;
        }

        Debug.Log($"Activating Stage {currentStageIndex}: {stages[currentStageIndex].stageName}");

        // Reset logic for the new stage
        correctPartsPlaced = 0;
        placedParts.Clear();
        penalizedParts.Clear();
        isInteractionAllowed = false;

        // Activate the correct panel
        for (int i = 0; i < stagePanels.Count; i++)
        {
            stagePanels[i].SetActive(i == currentStageIndex);
        }

        Debug.Log("Resetting stage logic and looking for images to turn black...");

        // Start the delay before turning the robot black
        StartCoroutine(DelayedTurnBlack());
    }


    private IEnumerator DelayedTurnBlack()
    {
        Debug.Log("Waiting 3 seconds before turning black...");
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        Debug.Log("Turning images black...");
        foreach (string targetObjectName in CurrentStage.targetObjectNames)
        {
            Debug.Log($"Searching for {targetObjectName} in {stagePanels[currentStageIndex].name}");

            GameObject targetObject = stagePanels[currentStageIndex].transform.Find(targetObjectName)?.gameObject;

            if (targetObject == null)
            {
                Debug.LogWarning($"Target object {targetObjectName} not found in {stagePanels[currentStageIndex].name}.");
                continue;
            }

            Image image = targetObject.GetComponent<Image>();
            if (image == null)
            {
                Debug.LogWarning($"No Image component found on {targetObjectName} in {stagePanels[currentStageIndex].name}.");
                continue;
            }

            image.color = Color.black; // Set the image color to black
            Debug.Log($"Successfully turned {targetObjectName} black.");
        }

        // Enable interaction after the robot turns black
        isInteractionAllowed = true;
        Debug.Log("Interaction allowed.");
    }


    public void PartPlacedCorrectly(GameObject part)
    {
        if (placedParts.Contains(part)) return; // Skip if the part is already placed correctly

        placedParts.Add(part);
        penalizedParts.Remove(part); // Clear penalty tracking on correct placement
        correctPartsPlaced++;

        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{CurrentStage.totalParts}");

        // Check if the stage is complete
        if (correctPartsPlaced >= CurrentStage.totalParts)
        {
            StageComplete();
        }
    }

    private IEnumerator DelayedStageComplete()
    {
        Debug.Log("Stage completed! Waiting 3 seconds before moving to next stage...");
        yield return new WaitForSeconds(3f); // Wait 3 seconds before moving to the next stage

        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            StartStage();
        }
        else
        {
            MiniGameComplete();
        }
    }

    public void PartPlacedIncorrectly(GameObject part)
    {
        if (!penalizedParts.Contains(part))
        {
            penalizedParts.Add(part); // Mark the part as penalized
            Debug.Log("Player lost a point for incorrect placement.");

            // Deduct 1 point using the OverallScoreManager
            OverallScoreManager.Instance?.AddScore(-1);
        }
        else
        {
            Debug.Log("This part was already penalized for being incorrect.");
        }
    }


    private void StageComplete()
    {
        Debug.Log($"Stage {CurrentStage.stageName} complete!");

        // Add score for completing this stage
        OverallScoreManager.Instance?.AddScoreFromStage(CurrentStage.stageName, CurrentStage.pointsForCompletion);

        // Start the coroutine to wait before moving to the next stage
        StartCoroutine(DelayedStageTransition());
    }

    private IEnumerator DelayedStageTransition()
    {
        Debug.Log("Waiting 3 seconds before moving to next stage...");
        yield return new WaitForSeconds(3f); // Wait 3 seconds

        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            StartStage(); // Start the next stage after waiting
        }
        else
        {
            MiniGameComplete(); // If no more stages, finish the mini-game
        }
    }


    private void MiniGameComplete()
    {
        Debug.Log("Mini-game complete!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    public bool IsInteractionAllowed()
    {
        return isInteractionAllowed;
    }
}
