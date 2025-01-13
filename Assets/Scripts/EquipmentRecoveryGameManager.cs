using UnityEngine;

public class EquipmentRecoveryGameManager : MonoBehaviour
{
    public static EquipmentRecoveryGameManager Instance; // Singleton for global access

    [Header("Game Settings")]
    public int totalParts; // Total number of parts that need to be placed correctly
    private int correctPartsPlaced = 0; // Counter for correctly placed parts

    [Header("UI Elements")]
    public GameObject levelCompletePanel; // Panel to show when the level is complete

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if needed
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void PartPlacedCorrectly()
    {
        correctPartsPlaced++;
        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{totalParts}");

        if (correctPartsPlaced >= totalParts)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level complete!");

        // Show the level complete panel
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // You can add additional logic here, like stopping player interactions or triggering animations
    }
}
