using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    public SymbolStage currentStage; // Reference to the active stage

    public void LoadStage(SymbolStage stage)
    {
        currentStage = stage;

        if (currentStage.symbols.Count != currentStage.meanings.Count)
        {
            Debug.LogError("Symbols and meanings count mismatch! Check the stage setup.");
        }
    }

    public Sprite GetSymbol(int index)
    {
        if (index >= 0 && index < currentStage.symbols.Count)
        {
            return currentStage.symbols[index];
        }
        return null;
    }

    public string GetMeaning(int index)
    {
        if (index >= 0 && index < currentStage.meanings.Count)
        {
            return currentStage.meanings[index];
        }
        return null;
    }

    public int GetSymbolCount()
    {
        return currentStage.symbols.Count;
    }

    public string GetRandomTip()
    {
        if (currentStage.tips.Count > 0)
        {
            int randomIndex = Random.Range(0, currentStage.tips.Count);
            return currentStage.tips[randomIndex];
        }
        return "No tips available.";
    }
}
