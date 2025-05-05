using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class MapLogoutHandler : MonoBehaviour
{
    public void LogoutFromMap()
    {
        Debug.Log("Logout button clicked — saving progress and logging out.");

        GameProgressManager.Instance?.SaveProgress();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
            Debug.Log("User signed out.");
        }

        SceneManager.LoadScene("WelcomeScene-vivi");
    }
}
