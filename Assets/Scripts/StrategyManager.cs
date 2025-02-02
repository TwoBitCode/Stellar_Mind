using UnityEngine;
using TMPro;

public class StrategyManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI strategyText; // Text field to display the strategy
    public GameObject strategyPanel; // Panel containing the strategy UI

    [Header("Strategies")]
    [TextArea(2, 5)]
    public string[] strategies; // Bank of strategies for the mini-game

    private int currentStrategyIndex = 0; // Tracks the current strategy

    private void Start()
    {
        // Hide the strategy panel initially
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(false);
        }

        // Ensure there's at least one strategy
        if (strategies.Length == 0)
        {
            Debug.LogWarning("No strategies found! Please add strategies in the Inspector.");
        }
    }

    /// <summary>
    /// Displays the next strategy when the button is clicked.
    /// </summary>
    public void ShowNextStrategy()
    {
        if (strategies.Length == 0)
        {
            Debug.LogWarning("No strategies available to show.");
            return;
        }

        // Show the panel if it’s not already active
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(true);
        }

        // Cycle through strategies
        strategyText.text = strategies[currentStrategyIndex];
        currentStrategyIndex = (currentStrategyIndex + 1) % strategies.Length; // Loop back to the first strategy
    }

    /// <summary>
    /// Hides the strategy panel.
    /// </summary>
    public void HideStrategyPanel()
    {
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(false);
        }
    }
    public void ShowStrategyPanel()
    {
        if (strategyPanel != null)
        {
            strategyPanel.SetActive(true);
        }
    }

}
