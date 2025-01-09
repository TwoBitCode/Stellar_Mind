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
                break;
            case SpaceMission.MissionType.NavigateToTarget:
                currentStrategies = navigateToTargetStrategies;
                break;
            default:
                currentStrategies = null;
                break;
        }

        strategyIndex = 0; // Reset strategy index
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
        string strategy = strategyIndex < currentStrategies?.Length
            ? currentStrategies[strategyIndex++]
            : "Try to focus on the sequence!";

        NotifyUI($"{incorrectFeedback}\n{strategy}\nWe will restart the stage. Pay close attention!");
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
            return "Pay attention and focus! Let’s try again.";
        }

        // Fetch the next strategy in the sequence
        string strategy = strategyIndex < currentStrategies.Length
            ? currentStrategies[strategyIndex++]
            : "Try to remember the steps carefully.";

        return strategy;
    }
    public void UpdateAlienText(string message)
    {
        AlienUIManager.Instance?.UpdateAlienText(message);
    }



}
