using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipmentRecoveryStage", menuName = "Equipment Recovery/Stage")]
public class EquipmentRecoveryStage : ScriptableObject
{
    [Header("Stage Settings")]
    public string stageName; // Name of the stage
    public int totalParts; // Total number of parts for the stage
    public int pointsForCompletion; // Points awarded for completing this stage

    [Header("UI Settings")]
    public List<string> targetObjectNames; // Names of the UI objects to turn black
}
