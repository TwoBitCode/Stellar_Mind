using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class EndScreenManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button reportButton;         // Opens game report
    public Button newCycleButton;       // Starts a new cycle and goes to game map
    public Button logoutButton;         // Logs out and goes back to welcome screen

    private void Start()
    {
        PlayerPrefs.SetString("LastSceneBeforeReport", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        if (newCycleButton != null)
            newCycleButton.onClick.AddListener(StartNewCycle);
        else
            Debug.LogWarning("NewCycleButton is not assigned.");

        if (logoutButton != null)
            logoutButton.onClick.AddListener(LogoutAndReturnToLogin);
        else
            Debug.LogWarning("LogoutButton is not assigned.");
    }

    public void OpenGameReport()
    {
        Debug.Log("Opening Game Report");
        SceneManager.LoadScene("Player report");
    }

    public void StartNewCycle()
    {
        Debug.Log("Start New Cycle button clicked.");
        GameProgressManager.Instance.AdvanceToNextCycle();
        SceneManager.LoadScene("GameMapScene-V");
    }

    public void LogoutAndReturnToLogin()
    {
        var pp = GameProgressManager.Instance.playerProgress;

        if (pp != null && AllGamesCompleted(pp))
        {
            Debug.Log("Player completed all games — saving current cycle before logout.");
            GameProgressManager.Instance.AdvanceToNextCycle();
        }

        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("User signed out from Unity Authentication.");
        }

        SceneManager.LoadScene("WelcomeScene-vivi");
    }

    private bool AllGamesCompleted(PlayerProgress pp)
    {
        foreach (var game in pp.gamesProgress.Values)
        {
            if (!game.isCompleted)
                return false;
        }
        return true;
    }

}
