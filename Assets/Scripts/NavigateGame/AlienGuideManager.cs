using UnityEngine;

public class AlienGuideManager : MonoBehaviour
{
    public static AlienGuideManager Instance;

    [Header("Feedback Settings")]
    public string correctFeedback = "Well done!";
    public string incorrectFeedback = "Oops! Try again.";
    public string restartMessage = "Restarting the mission... Get ready!";
    [TextArea] public string[] reconstructTrajectoryStrategies;
    [TextArea] public string[] navigateToTargetStrategies;

    private string[] currentStrategies;
    private int strategyIndex;

    public delegate void FeedbackUpdate(string message);
    //public static event FeedbackUpdate OnFeedbackUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMissionType(SpaceMission.MissionType missionType)
    {
        switch (missionType)
        {
            case SpaceMission.MissionType.ReconstructTrajectory:
                currentStrategies = reconstructTrajectoryStrategies;
                Debug.Log("Loaded reconstruct trajectory strategies.");
                break;
            case SpaceMission.MissionType.NavigateToTarget:
                currentStrategies = navigateToTargetStrategies;
                Debug.Log("Loaded navigate to target strategies.");
                break;
            default:
                currentStrategies = null;
                Debug.LogWarning("Mission type not recognized. No strategies loaded.");
                break;
        }

        strategyIndex = 0; // Reset strategy index
        Debug.Log($"Current strategies set: {string.Join(", ", currentStrategies ?? new string[0])}");
    }

    public void ProvidePositiveFeedback()
    {
        NotifyUI(correctFeedback); // Alien says "Well done!"
    }

    public void ProvideRestartMessage()
    {
        NotifyUI(restartMessage); // Alien says "Restarting the stage..."
    }


    public void ProvideNegativeFeedback()
    {
        NotifyUI(incorrectFeedback);
        ProvideStrategy();
    }

    public void ProvideStrategy()
    {
        if (currentStrategies == null || currentStrategies.Length == 0)
        {
            Debug.LogWarning("No strategies available. Using default strategy.");
            NotifyUI("Try to focus on the sequence!"); // Default fallback strategy
            return;
        }

        if (strategyIndex < currentStrategies.Length)
        {
            string strategy = currentStrategies[strategyIndex++];
            Debug.Log($"Providing strategy: {strategy} (Index: {strategyIndex - 1})");
            NotifyUI($"{incorrectFeedback}\n{strategy}\nWe will restart the stage. Pay close attention!");
        }
        else
        {
            Debug.LogWarning("Out of strategies. Using fallback strategy.");
            NotifyUI("Try to remember the steps carefully.");
        }
    }



    private void NotifyUI(string message)
    {
        AlienUIManager.Instance?.UpdateAlienText(message);
    }
    public void DisplayMissionIntro(string missionName)
    {
        AlienUIManager.Instance?.UpdateAlienText($"Welcome to the mission: {missionName}. Let's get started!");
    }
    public void NotifyHighlightStart()
    {
        NotifyUI("Keep an eye on the highlight!");
    }

    public void NotifyHighlightEnd()
    {
        NotifyUI("You can start now.");
    }
    public string GetNextStrategy()
    {
        if (currentStrategies == null || currentStrategies.Length == 0)
        {
            Debug.LogWarning("No strategies available. Using default strategy.");
            return "Pay attention and focus! Let’s try again.";
        }

        // Fetch the next strategy in the sequence
        if (strategyIndex < currentStrategies.Length)
        {
            string strategy = currentStrategies[strategyIndex];
            strategyIndex++; // Increment the index for the next call
            Debug.Log($"Returning strategy: {strategy} (Index: {strategyIndex - 1})");
            return strategy;
        }

        // Fallback if all strategies are used
        Debug.LogWarning("Out of strategies. Using fallback strategy.");
        return "Try to remember the steps carefully.";
    }



    public void UpdateAlienText(string message)
    {
        AlienUIManager.Instance?.UpdateAlienText(message);
    }



}
