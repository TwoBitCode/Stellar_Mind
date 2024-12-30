public interface IPlayerDataManager
{
    void SavePlayerData(string playerName, int targetScore);
    string LoadPlayerName();
    int LoadTargetScore();
}
