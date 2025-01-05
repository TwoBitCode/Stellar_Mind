using UnityEngine;

[System.Serializable]
public class StageData
{
    public bool isVoiceStage; // Whether this stage uses voices
    public SymbolStage symbolStage; // Reference to a symbol stage
    public VoiceStage voiceStage; // Reference to a voice stage
}
