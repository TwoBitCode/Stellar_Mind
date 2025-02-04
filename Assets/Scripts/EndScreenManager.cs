using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class EndScreenManager : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restart button clicked! Resetting all data...");

        // ����� �� ������� ������ �-PlayerPrefs (������� �-WebGL �������� ������)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // ����� ��������� ������

#if !UNITY_WEBGL
        // ����� ���� �������� �� �� �-WebGL
        string saveFilePath = Path.Combine(Application.persistentDataPath, "playerProgress.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Player progress file deleted.");
        }
        else
        {
            Debug.Log("No player progress file found.");
        }
#endif

        // ����� �-GameProgressManager ��� ����� ������ ����� ����
        if (GameProgressManager.Instance != null)
        {
            Debug.Log("Destroying GameProgressManager instance...");
            Destroy(GameProgressManager.Instance.gameObject); // ���� �� �������� ��������
        }

        // ����� ���� ������ ����
        Debug.Log("Loading first scene: WelcomeScene-vivi");
        SceneManager.LoadScene("WelcomeScene-vivi");
    }
}
