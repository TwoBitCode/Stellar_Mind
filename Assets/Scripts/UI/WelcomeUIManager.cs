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

        // Password must have at least 6 characters, including lowercase, uppercase, digit, special char
        if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$"))
        {
            feedbackText.text = "������ ����� ����� ����� 6 �����, ���� ��� ����, ��� �����, ���� ��� �����.";
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            feedbackText.text = "����� ������ ������.";

            // New user - create and save fresh progress
            GameProgressManager.Instance.playerProgress = new PlayerProgress(username, "");
            var pp = GameProgressManager.Instance.playerProgress;

            // Convert and snapshot Cycle 1
            pp.ConvertDictionaryToList();
            var initialSnapshot = new List<SerializableGameProgress>();
            foreach (var game in pp.gamesProgressList)
            {
                var copiedGame = new SerializableGameProgress
                {
                    gameIndex = game.gameIndex,
                    progress = JsonConvert.DeserializeObject<GameProgress>(
                        JsonConvert.SerializeObject(game.progress,
                        new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                    )
                };
                initialSnapshot.Add(copiedGame);
            }

            pp.cycleHistory.Add(new CycleSummary
            {
                cycleNumber = pp.currentCycle,
                totalScore = pp.totalScore,
                startDate = pp.currentCycleStartDate,
                endDate = pp.currentCycleStartDate, 
                gamesSnapshot = initialSnapshot
            });

            GameProgressManager.Instance.SaveProgress();

            SceneManager.LoadScene("LioAndMayaScene");
        }
        catch (AuthenticationException e)
        {
            feedbackText.text = $"����� �����: {e.Message}";
        }
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

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            feedbackText.text = "������� ������.";

            await GameProgressManager.Instance.LoadProgress();

            // If the cloud save had no name, inject the login username
            if (string.IsNullOrWhiteSpace(GameProgressManager.Instance.playerProgress?.playerName))
            {
                GameProgressManager.Instance.playerProgress.playerName = username;
                GameProgressManager.Instance.SaveProgress();
            }

            SceneManager.LoadScene("LioAndMayaScene");
        }
        catch (AuthenticationException e)
        {
            feedbackText.text = $"������� �����: {e.Message}";
        }
    }
}
