using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;


public class WelcomeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject startButton;

    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            // feedbackText.text = "שירותי יוניטי הופעלו בהצלחה.";
        }
        catch (System.Exception e)
        {
            feedbackText.text = $"הפעלת השירותים נכשלה: {e.Message}";
        }
    }

    public async void OnRegisterClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "יש למלא שם משתמש וסיסמה.";
            return;
        }

        // Username must contain only lowercase letters and digits
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-z0-9]+$"))
        {
            feedbackText.text = "שם משתמש יכיל רק אותיות קטנות באנגלית ומספרים";
            return;
        }

        if (password.Length < 4)
        {
            feedbackText.text = "הסיסמה חייבת להכיל לפחות 4 תווים.";
            return;
        }

        string finalPassword = MakePasswordCompliant(password); 


        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, finalPassword);
            feedbackText.text = "הרשמה הושלמה בהצלחה.";

            // New user - create and save fresh progress
            GameProgressManager.Instance.playerProgress = new PlayerProgress(username, "");
            var pp = GameProgressManager.Instance.playerProgress;


            GameProgressManager.Instance.SaveProgress();

            SceneManager.LoadScene("LioAndMayaScene");
        }
        catch (AuthenticationException e)
        {
            //Debug.LogError($"Request failed: {e.Message}");

            if (e.Message.ToLower().Contains("username already exists") || e.Message.Contains("409"))
            {
                feedbackText.text = "שם המשתמש כבר קיים. אנא הירשמו עם שם אחר.";
            }
            else
            {
                feedbackText.text = "הרשמה נכשלה";
            }
        }

    }

    private string MakePasswordCompliant(string rawPassword)
    {
        return rawPassword + "Aa1!";
    }



    public async void OnLoginClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "יש למלא שם משתמש וסיסמה.";
            return;
        }

        string finalPassword = MakePasswordCompliant(password);

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, finalPassword);
            feedbackText.text = "התחברות הצליחה.";

            await GameProgressManager.Instance.LoadProgress();

            if (string.IsNullOrWhiteSpace(GameProgressManager.Instance.playerProgress?.playerName))
            {
                GameProgressManager.Instance.playerProgress.playerName = username;
                GameProgressManager.Instance.SaveProgress();
            }

            SceneManager.LoadScene("LioAndMayaScene");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"Login failed: {e.Message}");

            if (e.Message.ToLower().Contains("wrong_username_password") || e.Message.ToLower().Contains("invalid_parameters"))
            {
                feedbackText.text = "שם משתמש או סיסמה שגויים. אם זו הפעם הראשונה שלך, לחץ על 'הרשמה'.";
            }
            else
            {
                feedbackText.text = $"התחברות נכשלה";
            }
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request failed: {e.Message}");
            feedbackText.text = $"שגיאה כללית בהתחברות";
        }
    }

}
