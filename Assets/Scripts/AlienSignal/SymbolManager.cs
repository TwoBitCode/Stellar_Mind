using System.Collections.Generic;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    // Symbol stage for handling symbols and their meanings
    public SymbolStage currentStage; // Reference to the active symbol stage

    // Voice stage for handling voices and their meanings
    public VoiceStage currentVoiceStage; // Reference to the active voice stage

    void Awake()
    {
        // Validate that symbols and meanings have matching counts
        if (currentStage != null && currentStage.symbols.Count != currentStage.meanings.Count)
        {
            Debug.LogError("Symbols and meanings count mismatch! Check the stage setup.");
        }

        if (currentVoiceStage != null && currentVoiceStage.voices.Count != currentVoiceStage.meanings.Count)
        {
            Debug.LogError("Voices and meanings count mismatch! Check the stage setup.");
        }
    }

    // Loads a specific symbol stage
    public void LoadStage(SymbolStage stage)
    {
        currentStage = stage;
        if (currentStage.symbols.Count != currentStage.meanings.Count)
        {
            Debug.LogError("Symbols and meanings count mismatch! Check the stage setup.");
        }
    }

    // Loads a specific voice stage
    public void LoadVoiceStage(VoiceStage stage)
    {
        currentVoiceStage = stage;
        if (currentVoiceStage.voices.Count != currentVoiceStage.meanings.Count)
        {
            Debug.LogError("Voices and meanings count mismatch! Check the stage setup.");
        }
    }

    // Retrieves a symbol by index
    public Sprite GetSymbol(int index)
    {
        if (currentStage != null && index >= 0 && index < currentStage.symbols.Count)
        {
            return currentStage.symbols[index];
        }
        return null;
    }

    // Retrieves the meaning of a symbol by index
    public string GetMeaning(int index)
    {
        if (currentStage != null && index >= 0 && index < currentStage.meanings.Count)
        {
            return currentStage.meanings[index];
        }
        return null;
    }

    // Retrieves the total count of symbols
    public int GetSymbolCount()
    {
        return currentStage != null ? currentStage.symbols.Count : 0;
    }

    // Retrieves a random tip for the current symbol stage
    public string GetRandomTip()
    {
        if (currentStage != null && currentStage.tips.Count > 0)
        {
            int randomIndex = Random.Range(0, currentStage.tips.Count);
            return currentStage.tips[randomIndex];
        }
        return "No tips available.";
    }

    // Retrieves a voice clip by index
    public AudioClip GetVoice(int index)
    {
        if (currentVoiceStage != null && index >= 0 && index < currentVoiceStage.voices.Count)
        {
            return currentVoiceStage.voices[index];
        }
        return null;
    }

    // Retrieves the meaning of a voice clip by index
    public string GetVoiceMeaning(int index)
    {
        if (currentVoiceStage != null && index >= 0 && index < currentVoiceStage.meanings.Count)
        {
            return currentVoiceStage.meanings[index];
        }
        return null;
    }

    // Retrieves the total count of voice clips
    public int GetVoiceCount()
    {
        return currentVoiceStage != null ? currentVoiceStage.voices.Count : 0;
    }
}