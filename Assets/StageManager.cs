using UnityEngine;

public class StageManager : MonoBehaviour
{
    public SymbolManager symbolManager;
    public SymbolLearningManager learningManager;
    public SymbolPracticeManager practiceManager;

    public SymbolStage[] stages; // Array of stages
    private int currentStageIndex = 0;

    void Start()
    {
        LoadCurrentStage();
    }

    public void LoadCurrentStage()
    {
        if (currentStageIndex < stages.Length)
        {
            SymbolStage stage = stages[currentStageIndex];
            symbolManager.LoadStage(stage);

            // Restart the learning phase for the new stage
            learningManager.InitializeLearningPhase();
        }
        else
        {
            Debug.Log("All stages completed!");
        }
    }

    public void AdvanceToNextStage()
    {
        currentStageIndex++;
        LoadCurrentStage();
    }
}
