using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restart button clicked! Resetting all data...");

        // ����� ������ ���� ������
        PlayerPrefs.SetInt("reset", 1);
        PlayerPrefs.Save();

#if UNITY_WEBGL
        // ����� localStorage ������ ��� JavaScript
        ResetLocalStorage();
#else
        // ����� �� ������� ������ �-PlayerPrefs (���������� ����� WebGL)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // ����� ���� ��������
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


        GameObject progressManager = GameObject.Find("GameProgressManager");
        if (progressManager != null)
        {
            Debug.Log("Found and destroying GameProgressManager manually.");
            Destroy(progressManager);
        }
        else if (GameProgressManager.Instance != null)
        {
            Debug.Log("Destroying GameProgressManager instance...");
            Destroy(GameProgressManager.Instance.gameObject);
        }

        // ����� ���� ������ ����
        Debug.Log("Loading first scene: WelcomeScene-vivi");
        SceneManager.LoadScene("WelcomeScene-vivi");
    }

#if UNITY_WEBGL
    private void ResetLocalStorage()
    {
        Debug.Log("Resetting localStorage via JavaScript...");
        Application.ExternalEval("localStorage.clear(); location.reload();");
    }
#endif
}
