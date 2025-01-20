using UnityEngine;
using System.Collections.Generic;

public class EquipmentRecoveryGameManager : MonoBehaviour
{
    public static EquipmentRecoveryGameManager Instance;

    [Header("Stage Settings")]
    public List<EquipmentRecoveryStage> stages; // List of stage data (ScriptableObjects)
    public List<GameObject> stagePanels; // List of stage panels (Canvas objects in the scene)

    private int currentStageIndex = 0;
    private int correctPartsPlaced = 0;
    private HashSet<GameObject> placedParts = new HashSet<GameObject>();

    [Header("UI Elements")]
    public GameObject gameOverPanel;

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
        correctPartsPlaced = 0;
        placedParts.Clear();

        // Activate the current stage panel
        for (int i = 0; i < stagePanels.Count; i++)
        {
            stagePanels[i].SetActive(i == currentStageIndex);
        }

        Debug.Log($"Starting Stage: {CurrentStage.stageName} with {CurrentStage.totalParts} parts.");
    }

    public void PartPlacedCorrectly(GameObject part)
    {
        if (placedParts.Contains(part)) return;

        placedParts.Add(part);
        correctPartsPlaced++;
        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{CurrentStage.totalParts}");

        if (correctPartsPlaced >= CurrentStage.totalParts)
        {
            StageComplete();
        }
    }

    private void StageComplete()
    {
        Debug.Log($"Stage {CurrentStage.stageName} complete!");

        // Add points to the overall score
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScoreFromStage(CurrentStage.stageName, CurrentStage.pointsForCompletion);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }

        // Move to the next stage
        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            Debug.Log($"Starting next stage: {stages[currentStageIndex].stageName}");
            StartStage(); // Start the next stage
        }
        else
        {
            Debug.Log("All stages complete. Triggering MiniGameComplete...");
            MiniGameComplete();
        }
    }

    private void MiniGameComplete()
    {
        Debug.Log("Mini-game complete!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Game over panel is not assigned!");
        }

        // Additional game-over logic (e.g., stopping interactions) can go here
    }

}
