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
            // feedbackText.text = "������ ������ ������ ������.";
        }
        catch (System.Exception e)
        {
            feedbackText.text = $"����� �������� �����: {e.Message}";
        }
    }

    public async void OnRegisterClicked()
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "�� ���� �� ����� ������.";
            return;
        }

        // Username must contain only lowercase letters and digits
        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-z0-9]+$"))
        {
            feedbackText.text = "�� ����� ���� �� ������ ����� ������� �������";
            return;
        }

        if (password.Length < 4)
        {
            feedbackText.text = "������ ����� ����� ����� 4 �����.";
            return;
        }

        string finalPassword = MakePasswordCompliant(password); 


        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, finalPassword);
            feedbackText.text = "����� ������ ������.";

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
                feedbackText.text = "�� ������ ��� ����. ��� ������ �� �� ���.";
            }
            else
            {
                feedbackText.text = "����� �����";
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
            feedbackText.text = "�� ���� �� ����� ������.";
            return;
        }

        string finalPassword = MakePasswordCompliant(password);

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, finalPassword);
            feedbackText.text = "������� ������.";

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
                feedbackText.text = "�� ����� �� ����� ������. �� �� ���� ������� ���, ��� �� '�����'.";
            }
            else
            {
                feedbackText.text = $"������� �����";
            }
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Request failed: {e.Message}");
            feedbackText.text = $"����� ����� ��������";
        }
    }

}
