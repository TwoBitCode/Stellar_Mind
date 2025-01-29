using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentRecoveryStage", menuName = "Equipment Recovery/Stage")]
public class EquipmentRecoveryStage : ScriptableObject
{
    [Header("Stage Settings")]
    public string stageName; // Name of the stage
    public int totalParts; // Total number of parts for the stage
    public int pointsForCompletion; // Points awarded for completing this stage
    public float stageTimeLimit;
    [Header("UI Settings")]
    public List<string> targetObjectNames; // Names of the UI objects to turn black
    [Header("Bonus Settings")]
    public float bonusTimeThreshold; // Minimum time left to earn a bonus
    public int bonusPoints; // Bonus points awarded if completed within the threshold

}
