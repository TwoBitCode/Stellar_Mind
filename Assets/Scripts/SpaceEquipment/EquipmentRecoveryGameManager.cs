using UnityEngine;
using System.Collections.Generic; // Required for HashSet

public class EquipmentRecoveryGameManager : MonoBehaviour
{
    public static EquipmentRecoveryGameManager Instance; // Singleton for global access

    [Header("Game Settings")]
    public int totalParts; // Total number of parts that need to be placed correctly
    public int pointsForCompletion = 10; // Points awarded for completing the level, modifiable in the Inspector
    private int correctPartsPlaced = 0; // Counter for correctly placed parts
    private HashSet<GameObject> placedParts = new HashSet<GameObject>(); // Tracks correctly placed parts

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

    public void PartPlacedCorrectly(GameObject part)
    {
        // Check if the part is already placed correctly
        if (placedParts.Contains(part))
        {
            Debug.Log($"Part {part.name} is already placed correctly. Ignoring.");
            return; // Exit early if the part was already counted
        }

        // Add part to the HashSet and update the count
        placedParts.Add(part);
        correctPartsPlaced++;
        Debug.Log($"Correct parts placed: {correctPartsPlaced}/{totalParts}");

        // Check if all parts are placed correctly
        if (correctPartsPlaced >= totalParts)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Level complete!");

        // Add points to the overall score
        if (OverallScoreManager.Instance != null)
        {
            OverallScoreManager.Instance.AddScoreFromStage("Equipment Recovery", pointsForCompletion);
        }
        else
        {
            Debug.LogError("OverallScoreManager instance not found!");
        }

        // Show the level complete panel
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // Additional logic (e.g., stopping interactions) can go here
    }
}
