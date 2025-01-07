using System.Collections.Generic;
using UnityEngine;

public class SymbolManager : MonoBehaviour
{
    public List<Sprite> symbols; // List of symbol sprites
    public List<string> meanings; // List of meanings corresponding to symbols

    void Awake()
    {
        if (symbols.Count != meanings.Count)
        {
            Debug.LogError("Symbols and meanings count mismatch! Check the SymbolManager setup.");
        }
    }

    public Sprite GetSymbol(int index)
    {
        if (index >= 0 && index < symbols.Count)
        {
            return symbols[index];
        }
        return null;
    }

    public string GetMeaning(int index)
    {
        if (index >= 0 && index < meanings.Count)
        {
            return meanings[index];
        }
        return null;
    }

    public int GetSymbolCount()
    {
        return symbols.Count;
    }
}
