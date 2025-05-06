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

        // Destroy persistent managers that shouldn't follow to WelcomeScene
        var persistentObjects = new string[]
        {
        "CharacterSelectionManager",
        "OverAllScoreManager"
        };

        foreach (var name in persistentObjects)
        {
            var obj = GameObject.Find(name);
            if (obj != null)
            {
                Destroy(obj);
                Debug.Log($"Destroyed persistent object: {name}");
            }
        }

        SceneManager.LoadScene("WelcomeScene-vivi");
    }

}
