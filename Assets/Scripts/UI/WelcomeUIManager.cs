using UnityEngine;
using TMPro;

public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI welcomeMessage;
    [SerializeField] private GameObject startButton;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    private void Start()
    {
        string savedName = GameProgressManager.Instance.GetPlayerProgress().playerName;

        if (!string.IsNullOrEmpty(savedName))
        {
            Debug.Log($"Loaded existing player name: {savedName}");
            playerNameInput.text = savedName;
            startButton.SetActive(true);
        }
        else
        {
            Debug.Log("No valid name found. Waiting for player input.");
            startButton.SetActive(false); // מונע מהשחקן להתחיל בלי שם
        }
    }


    public void OnNameInputChanged()
    {
        bool isValid = !string.IsNullOrEmpty(playerNameInput.text);
        startButton.SetActive(isValid);
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogWarning("Player name is empty! Cannot proceed.");
            return;
        }

        // שמירת שם השחקן
        GameProgressManager.Instance.InitializePlayer(playerNameInput.text, "");
        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Player name saved: {GameProgressManager.Instance.GetPlayerProgress().playerName}");

        // מעבר לסצנה הבאה
        sceneTransitionManager.LoadNextScene();
    }






}
