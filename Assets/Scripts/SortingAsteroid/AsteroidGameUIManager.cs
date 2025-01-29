using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidGameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Panels")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failurePanel;
    [SerializeField] private TextMeshProUGUI failureMessageText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    public void ShowInstructions(string instructions, System.Action onStartGame)
    {
        if (instructionsPanel == null || instructionsText == null)
        {
            Debug.LogError("Instructions panel or text is not assigned in the UI Manager!");
            return;
        }

        instructionsPanel.SetActive(true);
        instructionsText.text = instructions;

        Button startButton = instructionsPanel.GetComponentInChildren<Button>();
        if (startButton == null)
        {
            Debug.LogError("Start button not found in the instructions panel!");
            return;
        }

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(() =>
        {
            instructionsPanel.SetActive(false); // Hide the panel
            onStartGame?.Invoke(); // Trigger the StartGame method
        });
    }


    public void UpdateTimer(string timeText)
    {
        if (timerText != null)
        {
            timerText.text = timeText;
        }
        else
        {
            Debug.LogWarning("Timer text is not assigned in the UI Manager!");
        }
    }

    public void ShowSuccessPanel(string message)
    {
        if (successPanel != null)
        {
            successPanel.SetActive(true);

            TextMeshProUGUI successText = successPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (successText != null)
            {
                successText.text = message;
            }
        }
        else
        {
            Debug.LogWarning("Success panel is not assigned in the UI Manager!");
        }
    }




    public void HideSuccessPanel()
    {
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
    }

    public void ShowFailurePanel(string message, System.Action retryAction, System.Action returnToMenu)
    {
        HideAllUI(); // Hide other panels

        if (failurePanel != null)
        {
            failurePanel.SetActive(true);

            TextMeshProUGUI failureText = failurePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (failureText != null)
            {
                failureText.text = message;
            }

            Button[] buttons = failurePanel.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                // Retry button
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() =>
                {
                    failurePanel.SetActive(false); // Hide the failure panel
                    retryAction?.Invoke(); // Call the retry action
                });

                // Return to menu button
                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(() =>
                {
                    failurePanel.SetActive(false); // Hide the failure panel
                    returnToMenu?.Invoke(); // Call the return-to-menu action
                });
            }
        }
        else
        {
            Debug.LogWarning("Failure panel is not assigned in the UI Manager!");
        }
    }


    public void HideFailurePanel()
    {
        if (failurePanel != null)
        {
            failurePanel.SetActive(false);
        }
    }

    public void HideAllUI()
    {
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (successPanel != null) successPanel.SetActive(false);
        if (failurePanel != null) failurePanel.SetActive(false);
    }
    public void ShowTimerText()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
    }
    public void HideTimerText()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

}
