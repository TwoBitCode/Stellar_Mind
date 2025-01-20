using System.Collections.Generic;
using UnityEngine;

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
        penalizedParts.Clear(); // Clear penalties for new stage

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
        penalizedParts.Remove(part); // Clear penalty tracking on correct placement
        correctPartsPlaced++;

        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{CurrentStage.totalParts}");

        if (correctPartsPlaced >= CurrentStage.totalParts)
        {
            StageComplete();
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

        OverallScoreManager.Instance?.AddScoreFromStage(CurrentStage.stageName, CurrentStage.pointsForCompletion);

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

    private void MiniGameComplete()
    {
        Debug.Log("Mini-game complete!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
