// GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Stage
{
    public int numTubes;
    public bool isReverseOrder;
    public int timeLimit;
    public int scoreReward;
    public string instructionText;
    public int sortingTimeLimit;
    public int bonusTimeLimit;

}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float shuffleDelay = 2f; // Delay before shuffling
    [SerializeField] private float feedbackDuration = 2f; // Duration to show feedback
    [SerializeField] private List<Stage> stages;
    [SerializeField] private GameObject countdownBackground; // Timer background

    [Header("Managers")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private StackManager stackManager;
    [SerializeField] private TubesUIManager uiManager;
    //private DoorManager doorManager;

    private int currentStageIndex = 0;

    [SerializeField] private TubesGameIntroductionManager introductionManager; // Introduction manager
    [SerializeField] private GameObject errorPanel;
    private int gameIndex; // Store the game index
    public static GameManager Instance { get; private set; }
    private void Start()
    {
        ValidateSetup();
        var playerProgress = GameProgressManager.Instance.playerProgress;

        // Load last played stage and game
        gameIndex = playerProgress.lastPlayedGame;
        currentStageIndex = playerProgress.lastPlayedStage;

        Debug.Log($"Resuming Game {gameIndex}, Stage {currentStageIndex}");

        // Check if we are returning from map and NOT restarting
        if (SceneManager.GetActiveScene().name == "GameMapScene-V")
        {
            Debug.Log("Returning to Game Map, resetting GameManager.");
            Destroy(gameObject);  // This prevents GameManager from carrying over
            return;
        }

        // If resuming, skip intro and go straight to instructions
        if (currentStageIndex > 0)
        {
            introductionManager.SkipIntroduction();
            ShowStageIntroduction(currentStageIndex);
        }
        else
        {
            introductionManager.PlayIntroduction(() =>
            {
                ShowStageIntroduction(currentStageIndex);
            });
        }
    }





    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures GameManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
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
        uiManager.ChangeTimerBackgroundColor(Color.yellow);
        gridManager.ClearElements();
        stackManager.ClearElements();

        gridManager.GenerateGridElements(currentStage.numTubes);
        stackManager.GenerateStackElements(currentStage.numTubes);

        uiManager.ShowCheckButton();

       
        StartCoroutine(CountdownAndSetupStage(currentStage));
    }

    private IEnumerator CountdownAndSetupStage(Stage stage)
    {
        int remainingTime = stage.timeLimit;

        if (countdownBackground != null)
        {
            countdownBackground.SetActive(true);
       
        }

        while (remainingTime > 0)
        {
            uiManager.UpdateCountdownText($"{remainingTime}s");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        uiManager.UpdateCountdownText("");
        //if (countdownBackground != null)
        //{
        //    //countdownBackground.SetActive(false);
        //}

        gridManager.ShuffleGridElements();
        stackManager.MoveElementsToStack(gridManager.GridElements);


        uiManager.ChangeTimerBackgroundColor(Color.red);

        StartCoroutine(StartSortingTimer(stage));
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
        StopAllCoroutines(); 

        Stage currentStage = stages[currentStageIndex];
        bool isCorrect = currentStage.isReverseOrder ? CheckReverseOrder() : CheckOriginalOrder();

        if (isCorrect)
        {
            int remainingTime = Mathf.Max(0, currentStage.sortingTimeLimit - currentStage.bonusTimeLimit);
            int score = CalculateScore(remainingTime, currentStage);
            OverallScoreManager.Instance.AddScore(score);

            uiManager.UpdateResultText("יפה מאוד");
            uiManager.HideCheckButton();
            StartCoroutine(HandleFeedbackDelay(true));
        }
        else
        {
            ShowFailurePanel("טעית בסידור המבחנות. נסה שוב!");
        }
    }

    private IEnumerator HandleFeedbackDelay(bool isCorrect)
    {
        yield return new WaitForSeconds(feedbackDuration);

        if (isCorrect)
        {
            ProgressToNextStage(); // Move to the next stage
        }
 
    }
    private void ProgressToNextStage()
    {
        uiManager.ResetUI();
        var playerProgress = GameProgressManager.Instance.playerProgress;

        // Mark current stage as completed
        if (playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            GameProgress gameProgress = playerProgress.gamesProgress[gameIndex];

            if (gameProgress.stages.ContainsKey(currentStageIndex))
            {
                gameProgress.stages[currentStageIndex].isCompleted = true;
                Debug.Log($"Stage {currentStageIndex} in Game {gameIndex} marked as completed.");
            }
            else
            {
                Debug.LogError($"Stage {currentStageIndex} not found in game {gameIndex}!");
            }
        }
        else
        {
            Debug.LogError($"Game {gameIndex} not found in progress manager!");
        }

        currentStageIndex++;

        // If there are more stages, continue
        if (currentStageIndex < stages.Count)
        {
            Debug.Log($"Moving to next stage: {currentStageIndex}");
            uiManager.ChangeTimerBackgroundColor(Color.yellow);
            ShowStageIntroduction(currentStageIndex);
        }
        else
        {
            Debug.Log($"All stages completed for Game {gameIndex}!");
            uiManager.HideCheckButton();
            uiManager.HideInstructionPanel();
            uiManager.ShowCompletionPanel();
        }

        GameProgressManager.Instance.SaveProgress(); // Ensure the progress is saved
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

    public void RestartGame()
    {
        Debug.Log("Restarting the game...");
        currentStageIndex = 0;
        ShowStageIntroduction(currentStageIndex);
    }
    public void OnStartStageButtonClicked()
    {
        uiManager.HideInstructionPanel(); 
        StartStage(); 
    }

    private IEnumerator StartSortingTimer(Stage stage)
    {
        int remainingTime = stage.sortingTimeLimit;

        while (remainingTime > 0)
        {
            uiManager.UpdateSortingTimer($"{remainingTime}s"); // עדכון התצוגה של הזמן
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        uiManager.UpdateSortingTimer(""); 
        ShowFailurePanel("הזמן לסידור המבחנות נגמר. נסה שוב!");
    }
    private int CalculateScore(int remainingTime, Stage stage)
    {
        if (remainingTime >= stage.bonusTimeLimit)
        {
            return stage.scoreReward + 25; 
        }
        return stage.scoreReward; 
    }
    private void ShowFailurePanel(string message)
    {
        uiManager.ShowFailurePanel(message, () => RestartStage());
    }
    public void ReturnToMainMenu()
    {
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (playerProgress != null)
        {
            playerProgress.lastPlayedGame = gameIndex;
            playerProgress.lastPlayedStage = currentStageIndex;
            GameProgressManager.Instance.SaveProgress();
        }

        Debug.Log("Returning to Game Map...");

        // Reset the game state properly before switching scenes
        ResetGameState();

        // Destroy this GameManager so it does not persist across scenes
        Destroy(gameObject);

        SceneManager.LoadScene("GameMapScene-V");
    }


    private void ResetGameState()
    {
        currentStageIndex = 0;  // Ensure stage starts from zero on next return
       // gameIndex = -1;  // Reset game index to prevent reloading issues

        Debug.Log("Game state reset before exiting.");
    }



    public void RestartStage()
    {
        Debug.Log("Restarting the current stage...");
        uiManager.HideFailurePanel();
        ShowStageIntroduction(currentStageIndex); // Restart from the same stage
    }






}
