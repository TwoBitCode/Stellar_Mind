using UnityEngine;
using TMPro;

public class AlienGuideManager : MonoBehaviour
{
    public static AlienGuideManager Instance;

    [Header("Alien UI Elements")]
    public TextMeshProUGUI alienText; // Alien's dialogue text

    [Header("Feedback Settings")]
    public string correctFeedback = "Well done!";
    public string incorrectFeedback = "Oops! Try again.";
    [TextArea] public string[] reconstructTrajectoryStrategies;
    [TextArea] public string[] navigateToTargetStrategies;

    private string[] currentStrategies;
    private int strategyIndex;

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
        if (alienText != null)
        {
            alienText.text = correctFeedback;
        }
    }

    public void ProvideNegativeFeedback()
    {
        if (alienText != null)
        {
            alienText.text = incorrectFeedback;
            ProvideStrategy();
        }
    }

    public void ProvideStrategy()
    {
        if (currentStrategies != null && strategyIndex < currentStrategies.Length)
        {
            alienText.text += "\n" + currentStrategies[strategyIndex];
            strategyIndex++;
        }
        else
        {
            alienText.text += "\nTry to focus and plan your steps!";
        }

        // Restart the current stage
        RestartStage();
    }

    private void RestartStage()
    {
        SpaceMissionManager.Instance.RestartMission();
    }
}
