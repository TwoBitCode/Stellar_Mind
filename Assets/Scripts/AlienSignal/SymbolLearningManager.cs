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

    // Tracks the current item being displayed (symbol or voice)
    [SerializeField, Tooltip("The index of the current symbol or voice being displayed during learning.")]
    private int currentIndex = 0;

    [Header("Settings")]
    [SerializeField] private float fullOpacity = 1f; // Opacity for the symbol image
    [SerializeField] private string learningCompleteMessage = "You’ve learned all the items!"; // Completion message

    void Start()
    {
        InitializeLearningPhase();
    }

    public void InitializeLearningPhase()
    {
        currentIndex = 0; // Reset to the first item
        ShowNextItem();
    }

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

    private void ShowNextSymbol()
    {
        if (currentIndex < symbolManager.GetSymbolCount())
        {
            Sprite currentSymbol = symbolManager.GetSymbol(currentIndex);
            symbolDisplay.sprite = currentSymbol;

            // Ensure full opacity for the symbol image
            SetImageAlpha(symbolDisplay, fullOpacity);

            // Show the meaning text
            meaningText.text = $"This symbol means: {symbolManager.GetMeaning(currentIndex)}";

            nextButton.SetActive(true); // Enable "Next" button
        }
        else
        {
            EndLearningPhase();
        }
    }

    private void ShowNextVoice()
    {
        if (currentIndex < symbolManager.GetVoiceCount())
        {
            AudioClip currentVoice = symbolManager.GetVoice(currentIndex);
            audioSource.clip = currentVoice;
            audioSource.Play();

            // Show the meaning text
            meaningText.text = $"This voice means: {symbolManager.GetVoiceMeaning(currentIndex)}";

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
        currentIndex++; // Move to the next item
        ShowNextItem();
    }

    private void EndLearningPhase()
    {
        // Display completion message
        meaningText.text = learningCompleteMessage;

        nextButton.SetActive(false);

        // Transition to Practice Phase
        SymbolGameUIManager.Instance.ShowPracticeUI();
        FindAnyObjectByType<SymbolPracticeManager>().StartPractice();
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void RestartLearning()
    {
        InitializeLearningPhase(); // Resets the learning phase
    }
}
