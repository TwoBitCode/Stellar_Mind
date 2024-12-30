using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private IPlayerDataManager dataManager;

    private void Awake()
    {
        dataManager = new PlayerPrefsDataManager();
    }

    public void SavePlayerData(string playerName, int targetScore)
    {
        dataManager.SavePlayerData(playerName, targetScore);
    }

    public string LoadPlayerName()
    {
        return dataManager.LoadPlayerName();
    }

    public int LoadTargetScore()
    {
        return dataManager.LoadTargetScore();
    }
}
