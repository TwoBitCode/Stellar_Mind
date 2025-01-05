using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SymbolLearningManager : MonoBehaviour
{
    public SymbolManager symbolManager; // Reference to SymbolManager
    public TMP_Text meaningText; // Text to display meaning
    public Image symbolDisplay; // UI Image to display the symbol
    public GameObject nextButton; // Next button to proceed

    private int currentIndex = 0;

    void Start()
    {
        InitializeLearningPhase();
    }

    public void InitializeLearningPhase()
    {
        currentIndex = 0; // Reset to the first symbol
        ShowNextSymbol();
    }

    public void ShowNextSymbol()
    {
        if (currentIndex < symbolManager.GetSymbolCount())
        {
            // Display the current symbol and meaning
            Sprite currentSymbol = symbolManager.GetSymbol(currentIndex);
            symbolDisplay.sprite = currentSymbol;

            // Ensure full opacity for the symbol image
            SetImageAlpha(symbolDisplay, 1f);

            // Show the meaning text
            meaningText.text = $"This symbol means: {symbolManager.GetMeaning(currentIndex)}";

            nextButton.SetActive(true); // Enable "Next" button
        }
        else
        {
            EndLearningPhase();
        }
    }

    public void NextButtonPressed()
    {
        nextButton.SetActive(false); // Hide the "Next" button
        currentIndex++;
        ShowNextSymbol();
    }

    public void RestartLearning()
    {
        InitializeLearningPhase();
    }

    private void EndLearningPhase()
    {
        // Display completion message
        meaningText.text = "You’ve learned all the symbols!";
        nextButton.SetActive(false);

        // Transition to Practice Phase
        SymbolGameUIManager.Instance.ShowPracticeUI();
        FindAnyObjectByType<SymbolPracticeManager>().StartPractice();
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        // Adjust the alpha value of the image color
        Color color = image.color;
        color.a = alpha; // Set alpha to full opacity (1f)
        image.color = color;
    }
}
