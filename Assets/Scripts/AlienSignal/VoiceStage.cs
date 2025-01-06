using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewVoiceStage", menuName = "SymbolGame/VoiceStage")]
public class VoiceStage : ScriptableObject
{
    public string stageName; // Name of the stage
    public List<AudioClip> voices; // List of voice clips for this stage
    public List<string> meanings; // Corresponding meanings for voice clips
    [TextArea] public List<string> tips; // Learning tips for the stage
}
