using UnityEngine;

public class PlayerPrefsDataManager : IPlayerDataManager
{
    private const string PlayerNameKey = "PlayerName";
    private const string TargetScoreKey = "TargetScore";

    public void SavePlayerData(string playerName, int targetScore)
    {
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.SetInt(TargetScoreKey, targetScore);
        PlayerPrefs.Save();
        Debug.Log($"Player Data Saved: Name = {playerName}, Target Score = {targetScore}");
    }

    public string LoadPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, "Guest");
    }

    public int LoadTargetScore()
    {
        return PlayerPrefs.GetInt(TargetScoreKey, 0);
    }
}
