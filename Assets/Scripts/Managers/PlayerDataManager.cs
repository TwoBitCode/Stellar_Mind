//using UnityEngine;

//public class PlayerDataManager : MonoBehaviour
//{
//    public static PlayerDataManager Instance { get; private set; }

//    private IPlayerDataManager dataManager;

//    [Header("Default Values (For Testing)")]
//    [SerializeField] private string defaultPlayerName = "DefaultPlayer";
//    [SerializeField] private int defaultTargetScore = 100;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//            dataManager = new PlayerPrefsDataManager();
//            EnsureDefaults();
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void SavePlayerData(string playerName, int targetScore)
//    {
//        dataManager.SavePlayerData(playerName, targetScore);
//    }

//    public string LoadPlayerName()
//    {
//        string playerName = dataManager.LoadPlayerName();
//        return string.IsNullOrEmpty(playerName) ? defaultPlayerName : playerName;
//    }

//    public int LoadTargetScore()
//    {
//        int targetScore = dataManager.LoadTargetScore();
//        return targetScore <= 0 ? defaultTargetScore : targetScore;
//    }

//    private void EnsureDefaults()
//    {
//        if (string.IsNullOrEmpty(dataManager.LoadPlayerName()))
//        {
//            SavePlayerData(defaultPlayerName, defaultTargetScore);
//            Debug.Log($"Default data set: PlayerName={defaultPlayerName}, TargetScore={defaultTargetScore}");
//        }
//    }
//}
