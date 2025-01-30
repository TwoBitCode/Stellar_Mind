using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI welcomeMessage;
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        // ����� �� �� �� ���� ������ ����
        if (GameProgressManager.Instance.playerProgress != null && !string.IsNullOrEmpty(GameProgressManager.Instance.playerProgress.playerName))
        {
            playerNameInput.text = GameProgressManager.Instance.playerProgress.playerName;
            startButton.SetActive(true); // �� �� �� ����, ����� ������ ��� ����
        }
        else
        {
            startButton.SetActive(false); // �� ��� ��, ������ ����� �� ���� �����
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

        // ����� �� ����� �-PlayerProgress
        GameProgressManager.Instance.playerProgress.playerName = playerNameInput.text.Trim();
        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Player name saved: {GameProgressManager.Instance.playerProgress.playerName}");

        // ���� ������ ����
        SceneManager.LoadScene("LioAndMayaScene");
    }
}
