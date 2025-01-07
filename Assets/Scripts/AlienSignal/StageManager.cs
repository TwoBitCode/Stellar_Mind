using UnityEngine;

public class StageManager : MonoBehaviour
{
    public SymbolManager symbolManager;
    public SymbolLearningManager learningManager;
    public SymbolPracticeManager practiceManager;

    public StageData[] stages; // Array of stages

    [Header("Stage Index")]
    [SerializeField]
    private int startingStageIndex = 0; // The starting index for stages
    public int currentStageIndex { get; private set; } // Tracks the current stage index

    [Header("Stage Settings")]
    [SerializeField]
    private float defaultAdvanceDelay = 1f; // Default delay for advancing to the next stage
    private bool isStageComplete = false; // Tracks if the current stage is completed

    void Start()
    {
        // Initialize the starting stage index
        currentStageIndex = startingStageIndex;
        LoadCurrentStage();
    }

    public void LoadCurrentStage()
    {
        if (currentStageIndex < stages.Length)
        {
            isStageComplete = false; // Reset stage completion flag
            StageData currentStage = stages[currentStageIndex];
            Debug.Log($"[StageManager] Loading stage {currentStageIndex}: IsVoiceStage = {currentStage.isVoiceStage}");

            if (currentStage.isVoiceStage)
            {
                if (currentStage.voiceStage != null)
                {
                    Debug.Log($"[StageManager] Voice stage loaded: {currentStage.voiceStage.stageName}");
                    symbolManager.LoadVoiceStage(currentStage.voiceStage);
                    practiceManager.isVoiceMode = true;
                    learningManager.isVoiceMode = true;

                    SymbolGameUIManager.Instance.ShowLearningUI();
                }
                else
                {
                    Debug.LogError($"[StageManager] Stage {currentStageIndex} is marked as a voice stage, but no VoiceStage is assigned!");
                }
            }
            else
            {
                if (currentStage.symbolStage != null)
                {
                    Debug.Log($"[StageManager] Symbol stage loaded: {currentStage.symbolStage.stageName}");
                    symbolManager.LoadStage(currentStage.symbolStage);
                    practiceManager.isVoiceMode = false;
                    learningManager.isVoiceMode = false;

                    SymbolGameUIManager.Instance.ShowLearningUI();
                }
                else
                {
                    Debug.LogError($"[StageManager] Stage {currentStageIndex} is marked as a symbol stage, but no SymbolStage is assigned!");
                }
            }

            learningManager.InitializeLearningPhase();
        }
        else
        {
            Debug.Log("[StageManager] All stages completed!");
        }
    }

    public void CompleteStage()
    {
        if (!isStageComplete)
        {
            isStageComplete = true; // Mark the stage as complete

            // Award points for the current stage
            StageData currentStage = stages[currentStageIndex];
            if (OverallScoreManager.Instance != null && currentStage != null)
            {
                OverallScoreManager.Instance.AddScoreFromStage($"Stage {currentStageIndex}", currentStage.scoreReward);
                Debug.Log($"Stage {currentStageIndex} completed. Awarded {currentStage.scoreReward} points.");
            }
            else
            {
                Debug.LogError("OverallScoreManager instance or currentStage is null!");
            }
        }
    }

    public void AdvanceToNextStage()
    {
        if (isStageComplete && currentStageIndex < stages.Length)
        {
            currentStageIndex++;
            if (currentStageIndex < stages.Length)
            {
                LoadCurrentStage();
            }
            else
            {
                Debug.Log("[StageManager] All stages completed!");
            }
        }
        else
        {
            Debug.LogError("Cannot advance to the next stage. Stage is not complete or no more stages available!");
        }
    }

    public void AdvanceToNextStageWithDelay(float delay)
    {
        // Use defaultAdvanceDelay if no delay is specified
        float delayToUse = delay > 0 ? delay : defaultAdvanceDelay;
        Invoke(nameof(AdvanceToNextStage), delayToUse);
    }
}
