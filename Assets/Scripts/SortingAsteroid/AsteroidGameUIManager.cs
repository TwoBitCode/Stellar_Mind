using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidGameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI timerText;
    public void ShowInstructions(string instructions, System.Action onStartGame)
    {
        if (instructionsPanel == null || instructionsText == null)
        {
            Debug.LogError("Instructions panel or text is not assigned in the UI Manager!");
            return;
        }

        // Display the instructions
        instructionsPanel.SetActive(true);
        instructionsText.text = instructions;

        // Configure the start button
        Button startButton = instructionsPanel.GetComponentInChildren<Button>();
        if (startButton == null)
        {
            Debug.LogError("Start button not found in the instructions panel!");
            return;
        }

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            instructionsPanel.SetActive(false); // Hide the instructions panel
            Debug.Log("Start button clicked. Game starting...");
            onStartGame?.Invoke(); // Call the start game action
        });
    }




    public void UpdateTimerDisplay(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
