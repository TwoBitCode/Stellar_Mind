using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SymbolPracticeManager : MonoBehaviour
{
    public static SymbolPracticeManager Instance; // Singleton for global access

    public SymbolManager symbolManager; // Reference to SymbolManager
    public TMP_Text instructionText; // Text to display the question
    public TMP_Text feedbackText; // Text to display feedback
    public TMP_Text strategyText; // Text to display learning strategies
    public SymbolLearningManager symbolLearningManager; // Reference to the Learning Manager

    public bool isVoiceMode; // Toggle between symbols and voices
    public AudioSource audioSource; // To play voice clips

    private List<int> availableSymbolIndices; // Tracks which items haven't been practiced yet
    private int currentSymbolIndex;
    private int incorrectAttempts;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevent duplicate managers
    }

    void Start()
    {
        StartPractice();
    }

    public void StartPractice()
    {
        // Initialize the list of available items (symbols or voices) for this practice session
        availableSymbolIndices = new List<int>();
        int count = isVoiceMode ? symbolManager.GetVoiceCount() : symbolManager.GetSymbolCount();
        for (int i = 0; i < count; i++)
        {
            availableSymbolIndices.Add(i);
        }

        feedbackText.text = ""; // Clear feedback
        strategyText.text = ""; // Clear strategies
        instructionText.text = isVoiceMode
            ? "Listen to the voice and choose the correct meaning!"
            : "Match the meaning to the correct symbol!";
        NextRound();
    }

    private void NextRound()
    {
        if (availableSymbolIndices.Count == 0)
        {
            LevelComplete();
            return;
        }

        // Re-enable all buttons at the start of the round
        foreach (Button button in SymbolGameUIManager.Instance.answerButtons)
        {
            button.interactable = true;
        }

        feedbackText.text = ""; // Clear feedback
        strategyText.text = ""; // Clear any strategy text
        incorrectAttempts = 0; // Reset attempts for the new question

        // Pick a random item from the available list
        int randomIndex = Random.Range(0, availableSymbolIndices.Count);
        currentSymbolIndex = availableSymbolIndices[randomIndex];
        availableSymbolIndices.RemoveAt(randomIndex); // Remove the item from the list

        if (isVoiceMode)
        {
            string currentMeaning = symbolManager.GetVoiceMeaning(currentSymbolIndex);
            SymbolGameUIManager.Instance.DisplayQuestion($"What is the sound of this meaning: {currentMeaning}?");
        }
        else
        {
            string currentMeaning = symbolManager.GetMeaning(currentSymbolIndex);
            SymbolGameUIManager.Instance.DisplayQuestion($"What symbol represents: {currentMeaning}?");
        }

        SetupAnswerButtons();
    }


    private void SetupAnswerButtons()
    {
        List<int> availableIndices = new List<int>();

        if (isVoiceMode)
        {
            // Use distractor voices if in voice mode
            if (symbolManager.currentVoiceStage != null && symbolManager.currentVoiceStage.distractorVoices.Count > 0)
            {
                for (int i = 0; i < symbolManager.currentVoiceStage.distractorVoices.Count; i++)
                {
                    availableIndices.Add(i);
                }
            }
            else
            {
                Debug.LogWarning("No distractor voices available in currentVoiceStage!");
            }
        }
        else
        {
            // Use distractor symbols for symbol mode
            int itemCount = symbolManager.GetSymbolCount();
            for (int i = 0; i < itemCount; i++)
            {
                if (i != currentSymbolIndex)
                {
                    availableIndices.Add(i); // Exclude the correct symbol
                }
            }
        }

        int correctButtonIndex = Random.Range(0, SymbolGameUIManager.Instance.answerButtons.Length);

        for (int i = 0; i < SymbolGameUIManager.Instance.answerButtons.Length; i++)
        {
            Button button = SymbolGameUIManager.Instance.answerButtons[i];
            button.onClick.RemoveAllListeners(); // Clear previous listeners
            AudioSource buttonAudioSource = button.GetComponent<AudioSource>();

            if (i == correctButtonIndex)
            {
                // Set the correct answer
                if (isVoiceMode)
                {
                    buttonAudioSource.clip = symbolManager.GetVoice(currentSymbolIndex);
                    button.GetComponent<Image>().sprite = SymbolGameUIManager.Instance.soundIcon; // Use sound icon
                    button.onClick.AddListener(() => CheckAnswer(true));
                }
                else
                {
                    button.GetComponent<Image>().sprite = symbolManager.GetSymbol(currentSymbolIndex);
                    button.onClick.AddListener(() => CheckAnswer(true));
                }
            }
            else
            {
                // Assign distractors
                if (availableIndices.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableIndices.Count);
                    int distractorIndex = availableIndices[randomIndex];
                    availableIndices.RemoveAt(randomIndex);

                    if (isVoiceMode)
                    {
                        buttonAudioSource.clip = symbolManager.currentVoiceStage.distractorVoices[distractorIndex];
                        button.GetComponent<Image>().sprite = SymbolGameUIManager.Instance.soundIcon; // Use sound icon
                        button.onClick.AddListener(() => CheckAnswer(false));
                    }
                    else
                    {
                        button.GetComponent<Image>().sprite = symbolManager.GetSymbol(distractorIndex);
                        button.onClick.AddListener(() => CheckAnswer(false));
                    }
                }
                else
                {
                    Debug.LogWarning("No more distractors available!");
                }
            }

            AddHoverSound(button, buttonAudioSource); // Add hover functionality
        }
    }




    private Sprite GetRandomDistractorSymbol()
    {
        if (symbolManager.currentStage.distractorSymbols.Count > 0)
        {
            return symbolManager.currentStage.distractorSymbols[Random.Range(0, symbolManager.currentStage.distractorSymbols.Count)];
        }
        return symbolManager.GetRandomSymbol(); // Fallback to any random symbol
    }

    private AudioClip GetRandomDistractorVoice()
    {
        if (symbolManager.currentVoiceStage != null && symbolManager.currentVoiceStage.distractorVoices.Count > 0)
        {
            Debug.Log($"Distractor voices available: {symbolManager.currentVoiceStage.distractorVoices.Count}");
            int randomIndex = Random.Range(0, symbolManager.currentVoiceStage.distractorVoices.Count);
            Debug.Log($"Selected distractor voice index: {randomIndex}");
            return symbolManager.currentVoiceStage.distractorVoices[randomIndex];
        }

        Debug.LogWarning("No distractor voices defined in currentVoiceStage!");
        return symbolManager.GetRandomVoice(); // Fallback
    }






    private void AddHoverSound(Button button, AudioSource audioSource)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Clear previous triggers
        trigger.triggers.Clear();

        // Add hover event (PointerEnter)
        EventTrigger.Entry enterEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enterEntry.callback.AddListener((eventData) =>
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
        });
        trigger.triggers.Add(enterEntry);

        // Add leave event (PointerExit)
        EventTrigger.Entry exitEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exitEntry.callback.AddListener((eventData) =>
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop(); // Stop the sound when the pointer leaves
            }
        });
        trigger.triggers.Add(exitEntry);
    }

    private void PlayHoverSound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }



    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            SymbolGameUIManager.Instance.DisplayFeedback(true);
            Invoke(nameof(NextRound), 2f); // Move to the next question after a delay
        }
        else
        {
            incorrectAttempts++;
            SymbolGameUIManager.Instance.DisplayFeedback(false);

            // Show a tip and return to learning if too many incorrect attempts
            if (incorrectAttempts >= 2)
            {
                ShowLearningStrategy();
            }
        }
    }


    private void ShowLearningStrategy()
    {
        string strategy = symbolManager.GetRandomTip();
        SymbolGameUIManager.Instance.DisplayStrategy(strategy);

        Invoke(nameof(ReturnToLearningPhase), 3f);
    }

    private void ReturnToLearningPhase()
    {
        symbolLearningManager.RestartLearning();
        SymbolGameUIManager.Instance.ShowLearningUI();
    }

    private void LevelComplete()
    {
        SymbolGameUIManager.Instance.DisplayCompletion();
        SymbolGameUIManager.Instance.DisableAnswerButtons();

        // Notify StageManager that the stage is complete
        StageManager stageManager = FindAnyObjectByType<StageManager>();
        if (stageManager != null)
        {
            stageManager.CompleteStage(); // Mark the stage as complete
            stageManager.AdvanceToNextStageWithDelay(3f); // Delay advancing to the next stage
        }
        else
        {
            Debug.LogError("StageManager not found!");
        }
    }

    private void LoadNextStage()
    {
        FindAnyObjectByType<StageManager>().AdvanceToNextStage();
    }
}