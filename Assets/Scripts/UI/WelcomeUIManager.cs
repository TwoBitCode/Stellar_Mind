using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI welcomeMessage;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        // Wait a frame to ensure GameProgressManager is fully initialized
        yield return null;

        // Ensure GameProgressManager exists before accessing it
        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError("GameProgressManager is missing or uninitialized!");
            startButton.SetActive(false);
            yield break;
        }

        string savedName = GameProgressManager.Instance.playerProgress.playerName;

        // If a name is already saved, pre-fill the input field and activate the start button
        if (!string.IsNullOrEmpty(savedName))
        {
            playerNameInput.text = savedName;

            // Force UI refresh to properly display the text
            yield return null;
            playerNameInput.ForceLabelUpdate();

            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void OnNameInputChanged()
    {
        // Enable the start button only when input is not empty
        startButton.SetActive(!string.IsNullOrEmpty(playerNameInput.text.Trim()));
    }

    public void StartGame()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty! Cannot proceed.");
            return;
        }

        // Save the player's name
        GameProgressManager.Instance.playerProgress.playerName = playerName;
        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Player name saved: {playerName}");

        // Move to the character selection scene
        SceneManager.LoadScene("LioAndMayaScene");
    }
}
