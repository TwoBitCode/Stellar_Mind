using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SymbolPracticeManager : MonoBehaviour
{
    public SymbolManager symbolManager; // Reference to SymbolManager
    public TMP_Text instructionText; // Text to display the question
    public TMP_Text feedbackText; // Text to display feedback
    public TMP_Text strategyText; // Text to display learning strategies
    public SymbolLearningManager symbolLearningManager; // Reference to the Learning Manager

    [TextArea] public string[] learningStrategies; // Array for learning strategies

    private List<int> availableSymbolIndices; // Tracks which symbols haven't been practiced yet
    private int currentSymbolIndex;
    private int incorrectAttempts;

    void Start()
    {
        StartPractice();
    }

    public void StartPractice()
    {
        // Initialize the list of available symbols for this practice session
        availableSymbolIndices = new List<int>();
        for (int i = 0; i < symbolManager.GetSymbolCount(); i++)
        {
            availableSymbolIndices.Add(i);
        }

        feedbackText.text = ""; // Clear feedback
        strategyText.text = ""; // Clear strategies
        instructionText.text = "Match the meaning to the correct symbol!";
        NextRound();
    }

    private void NextRound()
    {
        if (availableSymbolIndices.Count == 0)
        {
            // All meanings have been practiced
            LevelComplete();
            return;
        }

        feedbackText.text = ""; // Clear feedback
        strategyText.text = ""; // Clear any strategy text
        incorrectAttempts = 0; // Reset attempts for the new question

        // Pick a random symbol from the available list
        int randomIndex = Random.Range(0, availableSymbolIndices.Count);
        currentSymbolIndex = availableSymbolIndices[randomIndex];
        availableSymbolIndices.RemoveAt(randomIndex); // Remove the symbol from the list

        string currentMeaning = symbolManager.GetMeaning(currentSymbolIndex);
        SymbolGameUIManager.Instance.DisplayQuestion(currentMeaning);
        SetupAnswerButtons();
    }

    private void SetupAnswerButtons()
    {
        int correctButtonIndex = Random.Range(0, SymbolGameUIManager.Instance.answerButtons.Length);

        for (int i = 0; i < SymbolGameUIManager.Instance.answerButtons.Length; i++)
        {
            Button button = SymbolGameUIManager.Instance.answerButtons[i];
            button.onClick.RemoveAllListeners();

            if (i == correctButtonIndex)
            {
                button.GetComponent<Image>().sprite = symbolManager.GetSymbol(currentSymbolIndex);
                button.onClick.AddListener(() => CheckAnswer(true));
            }
            else
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, symbolManager.GetSymbolCount());
                } while (randomIndex == currentSymbolIndex);

                button.GetComponent<Image>().sprite = symbolManager.GetSymbol(randomIndex);
                button.onClick.AddListener(() => CheckAnswer(false));
            }
        }
    }

    private void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            SymbolGameUIManager.Instance.DisplayFeedback(true);
            Invoke(nameof(NextRound), 2f);
        }
        else
        {
            incorrectAttempts++;
            if (incorrectAttempts >= 2)
            {
                ShowLearningStrategy();
            }
            else
            {
                SymbolGameUIManager.Instance.DisplayFeedback(false);
            }
        }
    }

    private void ShowLearningStrategy()
    {
        // Display a random strategy from the array
        if (learningStrategies.Length > 0)
        {
            string strategy = learningStrategies[Random.Range(0, learningStrategies.Length)];
            SymbolGameUIManager.Instance.DisplayStrategy(strategy);
        }
        else
        {
            SymbolGameUIManager.Instance.DisplayStrategy("No strategies available. Please add some!");
        }

        // Wait before transitioning to learning
        Invoke(nameof(ReturnToLearningPhase), 3f);
    }

    private void ReturnToLearningPhase()
    {
        symbolLearningManager.RestartLearning();
        SymbolGameUIManager.Instance.ShowLearningUI();
    }

    private void LevelComplete()
    {
        // Display level completion message
        SymbolGameUIManager.Instance.DisplayCompletion();
        SymbolGameUIManager.Instance.DisableAnswerButtons();
    }
}
