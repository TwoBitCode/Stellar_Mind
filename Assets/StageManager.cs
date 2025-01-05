using UnityEngine;

public class StageManager : MonoBehaviour
{
    public SymbolManager symbolManager;
    public SymbolLearningManager learningManager;
    public SymbolPracticeManager practiceManager;

    public StageData[] stages; // Array of stages
    private int currentStageIndex = 0;

    void Start()
    {
        LoadCurrentStage();
    }

    public void LoadCurrentStage()
    {
        if (currentStageIndex < stages.Length)
        {
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

                    // Show the learning UI for voice mode
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

                    // Show the learning UI for symbol mode
                    SymbolGameUIManager.Instance.ShowLearningUI();
                }
                else
                {
                    Debug.LogError($"[StageManager] Stage {currentStageIndex} is marked as a symbol stage, but no SymbolStage is assigned!");
                }
            }

            // Start the learning phase for the new stage
            learningManager.InitializeLearningPhase();
        }
        else
        {
            Debug.Log("[StageManager] All stages completed!");
        }
    }


    public void AdvanceToNextStage()
    {
        currentStageIndex++;
        if (currentStageIndex < stages.Length)
        {
            LoadCurrentStage();
        }
        else
        {
            Debug.Log("[StageManager] No more stages to load! Game has reached the end.");
        }
    }

    public void AdvanceToNextStageWithDelay(float delay)
    {
        Invoke(nameof(AdvanceToNextStage), delay);
    }
}
