// GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Stage
{
    public int numTubes;
    public bool isReverseOrder;
    public int timeLimit;
    public int scoreReward;
    public string instructionText;

}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float shuffleDelay = 2f;
    [SerializeField] private List<Stage> stages;

    [Header("Managers")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private StackManager stackManager;
    [SerializeField] private TubesUIManager uiManager;
    private DoorManager doorManager;


    private int currentStageIndex = 0;

    [SerializeField] private TubesGameIntroductionManager introductionManager; // Reference to the new introduction manager
    private void Start()
    {
        ValidateSetup();

        // Access the DoorManager singleton
        doorManager = Object.FindFirstObjectByType<DoorManager>();
        if (doorManager == null)
        {
            Debug.LogError("DoorManager not found in the scene!");
        }

        // Play the introduction
        introductionManager.PlayIntroduction(() =>
        {
            Debug.Log("Introduction complete. Showing first stage instructions...");
            ShowStageIntroduction(currentStageIndex); // Show instructions for the first stage
        });
    }


    private void ValidateSetup()
    {
        if (gridManager == null || stackManager == null || uiManager == null || stages == null || stages.Count == 0)
        {
            Debug.LogError("GameManager setup is incomplete! Make sure all references and stages are assigned.");
        }
    }


    private void ShowStageIntroduction(int stageIndex)
    {
        if (stageIndex >= stages.Count)
        {
            Debug.Log("All stages completed!");
            //uiManager.ShowRestartButton();
            return;
        }

        Stage currentStage = stages[stageIndex];
        Debug.Log($"Stage {stageIndex + 1}: {currentStage.instructionText}");
        uiManager.ShowInstructionPanel(currentStage.instructionText);
    }


    private void StartStage()
    {
        Stage currentStage = stages[currentStageIndex];
        Debug.Log($"Starting Stage {currentStageIndex + 1}");

        // Clear previous elements
        gridManager.ClearElements();
        stackManager.ClearElements();

        // Generate grid elements based on the current stage
        gridManager.GenerateGridElements(currentStage.numTubes);
        stackManager.GenerateStackElements(currentStage.numTubes);

        // Show "Check Answer" button
        uiManager.ShowCheckButton();

        // Start the timer and then shuffle the elements
        StartCoroutine(CountdownAndSetupStage(currentStage));
    }

    private IEnumerator CountdownAndSetupStage(Stage stage)
    {
        int remainingTime = stage.timeLimit;

        // Timer starts here
        while (remainingTime > 0)
        {
            uiManager.UpdateCountdownText($"{remainingTime}s");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        // Clear the timer display after countdown finishes
        uiManager.UpdateCountdownText("");

        // Shuffle the grid and finalize setup
        gridManager.ShuffleGridElements();
        stackManager.MoveElementsToStack(gridManager.GridElements);
        Debug.Log("Stage setup completed. Player can now start.");

        // Show "Check Result" button only after the timer
        uiManager.ShowCheckButton();
    }

    private IEnumerator ShuffleAndDisplayStage(Stage stage)
    {
        yield return StartCountdown(shuffleDelay);

        gridManager.ShuffleGridElements();
        stackManager.MoveElementsToStack(gridManager.GridElements);
        Debug.Log("Stage setup completed.");

        uiManager.ShowCheckButton();
    }

    private IEnumerator StartCountdown(float delay)
    {
        float countdown = delay;

        while (countdown > 0)
        {
            uiManager.UpdateCountdownText(Mathf.Ceil(countdown).ToString());
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        uiManager.UpdateCountdownText("");
    }

    // Called when the player clicks "Check Answer"
    public void OnCheckAnswerButtonClicked()
    {
        CheckAnswer();
    }

    // Check the player's answer
    private void CheckAnswer()
    {
        Debug.Log("Checking if the grid is in the correct order...");

        Stage currentStage = stages[currentStageIndex];
        bool isCorrect = currentStage.isReverseOrder ? CheckReverseOrder() : CheckOriginalOrder();

        if (isCorrect)
        {
            Debug.Log($"Stage {currentStageIndex + 1} completed successfully.");
            OverallScoreManager.Instance.AddScore(currentStage.scoreReward);
            uiManager.UpdateResultText("Correct Order!"); // Only updates feedback
            uiManager.HideCheckButton(); // Hides the button on success
            ProgressToNextStage();
        }
        else
        {
            Debug.Log("Incorrect order. Player can try again.");
            uiManager.UpdateResultText("Incorrect! Try Again."); // Keeps feedback clear
                                                                 // Button remains visible for retry
        }
    }






    private void RetryStage()
    {
        Debug.Log("Retrying the current stage with new colors...");
        RegenerateColors(); // Regenerate new colors
        uiManager.ResetUI(); // Reset the UI for the stage
        uiManager.ShowCheckButton(); // Ensure the "Check Answer" button is visible
    }


    // Progress to the next stage
    private void ProgressToNextStage()
    {
        // Clear feedback before moving to the next stage
        uiManager.ResetUI();

        currentStageIndex++;
        if (currentStageIndex < stages.Count)
        {
            ShowStageIntroduction(currentStageIndex);
        }
        else
        {
            Debug.Log("All stages completed!");

            //// Mark this mini-game as completed in the DoorManager
            //if (doorManager != null)
            //{
            //    doorManager.MarkGameAsCompleted(0); // Replace 2 with the correct index for this mini-game
            //}

            // Show the completion panel when all stages are done
            uiManager.HideCheckButton();
            uiManager.HideInstructionPanel();
            uiManager.ShowCompletionPanel();
        }
    }


    private bool CheckOriginalOrder()
    {
        for (int i = 0; i < gridManager.GridElements.Length; i++)
        {
            DraggableItem draggableItem = gridManager.GridElements[i].GetComponentInChildren<DraggableItem>();

            if (draggableItem == null || draggableItem.TubeID != gridManager.OriginalTubeIDs[i])
            {
                Debug.Log($"Mismatch at index {i}. Player needs to retry.");
                return false;
            }
        }
        return true;
    }

    private bool CheckReverseOrder()
    {
        for (int i = 0; i < gridManager.GridElements.Length; i++)
        {
            DraggableItem draggableItem = gridManager.GridElements[i].GetComponentInChildren<DraggableItem>();

            if (draggableItem == null || draggableItem.TubeID != gridManager.OriginalTubeIDs[gridManager.OriginalTubeIDs.Length - 1 - i])
            {
                Debug.Log($"Mismatch at index {i}. Player needs to retry.");
                return false;
            }
        }
        return true;
    }

    private void DisplayResult(bool isCorrect)
    {
        if (isCorrect)
        {
            Stage currentStage = stages[currentStageIndex];
            uiManager.UpdateResultText("Correct Order!");


            Debug.Log($"Stage {currentStageIndex + 1} completed. Awarding {currentStage.scoreReward} points.");
            currentStageIndex++;

            ShowStageIntroduction(currentStageIndex);
        }
        else
        {
            uiManager.UpdateResultText("Incorrect Order! Try Again.");

            StartStage();
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting the game...");
        currentStageIndex = 0;
        ShowStageIntroduction(currentStageIndex);
    }
    public void OnStartStageButtonClicked()
    {
        uiManager.HideInstructionPanel(); // Hide the instruction panel
        StartStage(); // Begin the first stage
    }
    private void RegenerateColors()
    {
        Debug.Log("Regenerating colors for the current stage...");

        // Shuffle and regenerate grid elements
        gridManager.ClearElements();
        gridManager.GenerateGridElements(stages[currentStageIndex].numTubes);

        // Update the stack with the new grid setup
        stackManager.ClearElements();
        stackManager.GenerateStackElements(stages[currentStageIndex].numTubes);

        Debug.Log("Colors regenerated successfully.");
    }
    private IEnumerator RestartStageWithNewColors()
    {
        Stage currentStage = stages[currentStageIndex];

        // Clear the feedback after a short delay
        yield return new WaitForSeconds(1f);
        uiManager.UpdateResultText("");

        // Clear and regenerate elements
        gridManager.ClearElements();
        stackManager.ClearElements();
        gridManager.GenerateGridElements(currentStage.numTubes);
        stackManager.GenerateStackElements(currentStage.numTubes);

        Debug.Log("New colors generated. Starting countdown...");

        // Countdown before moving elements to stack
        int remainingTime = currentStage.timeLimit;
        while (remainingTime > 0)
        {
            uiManager.UpdateCountdownText($"Time Left: {remainingTime}s");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        uiManager.UpdateCountdownText(""); // Clear the countdown text

        // Shuffle and move elements to stack
        gridManager.ShuffleGridElements();
        stackManager.MoveElementsToStack(gridManager.GridElements);

        Debug.Log("Stage restarted with reshuffled elements. Player can try again.");
    }



}
