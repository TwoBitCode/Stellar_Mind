using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SymbolPracticeManager : MonoBehaviour
{
    public static SymbolPracticeManager Instance; // Singleton for global access

    [Header("References")]
    public SymbolManager symbolManager; // Reference to SymbolManager
    public TMP_Text instructionText; // Text to display the question
    public TMP_Text feedbackText; // Text to display feedback
    public TMP_Text strategyText; // Text to display learning strategies
    public SymbolLearningManager symbolLearningManager; // Reference to the Learning Manager
    public AudioSource audioSource; // AudioSource for playing voice clips

    [Header("Settings")]
    [TextArea] public string[] learningStrategies; // Array for learning strategies
    public bool isVoiceMode; // Toggle between symbols and voices

    [SerializeField] private float feedbackDelay = 2f; // Delay before moving to the next round
    [SerializeField] private int maxIncorrectAttempts = 2; // Maximum incorrect attempts before showing a strategy
    [SerializeField] private float strategyDisplayDuration = 3f; // Time to display the learning strategy

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

        // Clear UI texts
        feedbackText.text = "";
        strategyText.text = "";
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

        // Clear feedback and reset incorrect attempts
        feedbackText.text = "";
        strategyText.text = "";
        incorrectAttempts = 0;

        // Pick a random item from the available list
        int randomIndex = Random.Range(0, availableSymbolIndices.Count);
        currentSymbolIndex = availableSymbolIndices[randomIndex];
        availableSymbolIndices.RemoveAt(randomIndex);

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
        int correctButtonIndex = Random.Range(0, SymbolGameUIManager.Instance.answerButtons.Length);

        for (int i = 0; i < SymbolGameUIManager.Instance.answerButtons.Length; i++)
        {
            Button button = SymbolGameUIManager.Instance.answerButtons[i];
            button.onClick.RemoveAllListeners(); // Clear previous listeners
            AudioSource buttonAudioSource = button.GetComponent<AudioSource>();

            if (i == correctButtonIndex)
            {
                if (isVoiceMode)
                {
                    // Set the correct sound for the button in voice mode
                    buttonAudioSource.clip = symbolManager.GetVoice(currentSymbolIndex);
                    button.onClick.AddListener(() => CheckAnswer(true));
                }
                else
                {
                    // Set the correct symbol for the button in symbol mode
                    button.GetComponent<Image>().sprite = symbolManager.GetSymbol(currentSymbolIndex);
                    button.onClick.AddListener(() => CheckAnswer(true));
                }
            }
            else
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, isVoiceMode ? symbolManager.GetVoiceCount() : symbolManager.GetSymbolCount());
                } while (randomIndex == currentSymbolIndex);

                if (isVoiceMode)
                {
                    // Set a random sound for the button in voice mode
                    buttonAudioSource.clip = symbolManager.GetVoice(randomIndex);
                    button.onClick.AddListener(() => CheckAnswer(false));
                }
                else
                {
                    // Set a random symbol for the button in symbol mode
                    button.GetComponent<Image>().sprite = symbolManager.GetSymbol(randomIndex);
                    button.onClick.AddListener(() => CheckAnswer(false));
                }
            }

            // Add hover sound functionality
            AddHoverSound(button, buttonAudioSource);
        }
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

        // Add hover event
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => PlayHoverSound(audioSource));
        trigger.triggers.Add(entry);
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
            Invoke(nameof(NextRound), feedbackDelay); // Use the serialized delay
        }
        else
        {
            incorrectAttempts++;
            if (incorrectAttempts >= maxIncorrectAttempts)
            {
                ShowLearningStrategy(); // Use the serialized max attempts
            }
            else
            {
                SymbolGameUIManager.Instance.DisplayFeedback(false);
            }
        }
    }

    private void ShowLearningStrategy()
    {
        // Display a random strategy or fallback text
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
        Invoke(nameof(ReturnToLearningPhase), strategyDisplayDuration);
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
    }
}
