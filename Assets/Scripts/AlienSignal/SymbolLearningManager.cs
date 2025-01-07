using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SymbolLearningManager : MonoBehaviour
{
    public SymbolManager symbolManager; // Reference to SymbolManager
    public TMP_Text meaningText; // Text to display meaning
    public Image symbolDisplay; // UI Image to display the symbol
    public GameObject nextButton; // Next button to proceed
    public AudioSource audioSource; // AudioSource for playing voice clips
    public bool isVoiceMode; // Toggle between symbols and voices

    [SerializeField]
    private int startingIndex = 0; // Starting index for the learning phase

    private int currentIndex;

    void Start()
    {
        InitializeLearningPhase();
    }

    public void InitializeLearningPhase()
    {
        currentIndex = startingIndex; // Reset to the starting index
        ShowNextItem();
    }

    // Shows the next item based on the current mode (symbol or voice)
    public void ShowNextItem()
    {
        if (isVoiceMode)
        {
            ShowNextVoice();
        }
        else
        {
            ShowNextSymbol();
        }
    }

    // Shows the next symbol in the learning phase
    private void ShowNextSymbol()
    {
        if (currentIndex < symbolManager.GetSymbolCount())
        {
            // Display the current symbol and its meaning
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

    // Shows the next voice in the learning phase
    private void ShowNextVoice()
    {
        if (currentIndex < symbolManager.GetVoiceCount())
        {
            // Play the current voice and display its meaning
            AudioClip currentVoice = symbolManager.GetVoice(currentIndex);
            audioSource.clip = currentVoice;
            audioSource.Play();

            meaningText.text = $"This voice means: {symbolManager.GetVoiceMeaning(currentIndex)}";

            nextButton.SetActive(true); // Enable "Next" button
        }
        else
        {
            EndLearningPhase();
        }
    }

    // Handles the "Next" button press to show the next item
    public void NextButtonPressed()
    {
        nextButton.SetActive(false); // Hide the "Next" button
        currentIndex++;
        ShowNextItem();
    }

    // Ends the learning phase and transitions to the practice phase
    private void EndLearningPhase()
    {
        // Display completion message
        meaningText.text = "You’ve learned all the items!";
        nextButton.SetActive(false);

        // Transition to Practice Phase
        SymbolGameUIManager.Instance.ShowPracticeUI();
        FindAnyObjectByType<SymbolPracticeManager>().StartPractice();
    }

    // Sets the alpha transparency for the symbol display image
    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha; // Adjust alpha value
        image.color = color;
    }

    // Restarts the learning phase from the beginning
    public void RestartLearning()
    {
        InitializeLearningPhase();
    }
}