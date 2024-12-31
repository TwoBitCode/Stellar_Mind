using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float shuffleDelay = 2f;
    [SerializeField] private int numObjects = 4;
    [SerializeField] private int pointsToAdd = 50;

    [Header("Managers")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private StackManager stackManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    [Header("Scene Configuration")]
    [Tooltip("Name of the map scene to load")]
    [SerializeField] private string mapSceneName = "MapScene";
    [Tooltip("Name of the game over scene to load")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    private void Start()
    {
        ValidateSetup();
        InitializeGame();
    }

    private void ValidateSetup()
    {
        if (gridManager == null || stackManager == null || scoreManager == null || uiManager == null || sceneTransitionManager == null)
        {
            Debug.LogError("One or more managers are not assigned in GameManager!");
        }
    }

    private void InitializeGame()
    {
        gridManager.GenerateGridElements(numObjects);
        stackManager.GenerateStackElements(numObjects);
        StartCoroutine(ShuffleAfterDelay(shuffleDelay));
        DisplayOriginalOrder();
        uiManager.ResetUI();
    }

    private void DisplayOriginalOrder()
    {
        string originalOrder = "Original Order: ";
        foreach (GameObject element in gridManager.GridElements)
        {
            DraggableItem draggableItem = element.GetComponentInChildren<DraggableItem>();
            if (draggableItem != null)
            {
                originalOrder += $"{draggableItem.TubeID} ";
            }
        }
        Debug.Log(originalOrder);
    }

    private IEnumerator ShuffleAfterDelay(float delay)
    {
        yield return StartCountdown(delay);
        gridManager.ShuffleGridElements();
        stackManager.MoveElementsToStack(gridManager.GridElements);
        Debug.Log("Shuffle and move to stack completed.");
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

    public void CheckAnswer()
    {
        Debug.Log("Checking if the grid is in the correct order...");

        bool isCorrect = true;

        for (int i = 0; i < gridManager.GridElements.Length; i++)
        {
            DraggableItem draggableItem = gridManager.GridElements[i].GetComponentInChildren<DraggableItem>();

            if (draggableItem == null)
            {
                Debug.Log($"Grid index {i}: No DraggableItem found!");
                isCorrect = false;
                continue;
            }

            int expectedTubeID = gridManager.OriginalTubeIDs[i];
            int actualTubeID = draggableItem.TubeID;

            Debug.Log($"Grid index {i}: Expected TubeID {expectedTubeID}, Found TubeID {actualTubeID}");

            if (expectedTubeID != actualTubeID)
            {
                Debug.Log($"Grid index {i}: Mismatch! Expected TubeID {expectedTubeID}, Found TubeID {actualTubeID}");
                isCorrect = false;
            }
        }

        DisplayResult(isCorrect);
    }

    private void DisplayResult(bool isCorrect)
    {
        if (isCorrect)
        {
            uiManager.UpdateResultText("Correct Order!", true);
            scoreManager.AddScore(pointsToAdd);
            UpdateOverallScore(pointsToAdd);
        }
        else
        {
            uiManager.UpdateResultText("Incorrect Order! Try Again.", false);
        }

        uiManager.ShowRestartButton();
    }

    private void UpdateOverallScore(int points)
    {
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScore(points);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting the game...");
        gridManager.ClearElements();
        stackManager.ClearElements();
        gridManager.GenerateGridElements(numObjects);
        stackManager.GenerateStackElements(numObjects);
        uiManager.ResetUI();
        StartCoroutine(ShuffleAfterDelay(shuffleDelay));
        Debug.Log("Game restarted successfully with new colors!");
    }

    public void HandleSceneTransition()
    {
        if (OverallScoreManager.Instance != null)
        {
            string targetScene = OverallScoreManager.Instance.OverallScore >= OverallScoreManager.Instance.TargetScore
                ? gameOverSceneName
                : mapSceneName;

            sceneTransitionManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }
    }
}
