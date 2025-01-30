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
        // לבדוק אם יש שם שמור ולהציג אותו
        if (GameProgressManager.Instance.playerProgress != null && !string.IsNullOrEmpty(GameProgressManager.Instance.playerProgress.playerName))
        {
            playerNameInput.text = GameProgressManager.Instance.playerProgress.playerName;
            startButton.SetActive(true); // אם יש שם שמור, כפתור ההתחלה כבר פעיל
        }
        else
        {
            startButton.SetActive(false); // אם אין שם, הכפתור יופיע רק אחרי הקלדה
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

        // שמירת שם השחקן ב-PlayerProgress
        GameProgressManager.Instance.playerProgress.playerName = playerNameInput.text.Trim();
        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Player name saved: {GameProgressManager.Instance.playerProgress.playerName}");

        // מעבר לבחירת דמות
        SceneManager.LoadScene("LioAndMayaScene");
    }
}
