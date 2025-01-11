using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSymbolStage", menuName = "SymbolGame/Stage")]
public class SymbolStage : ScriptableObject
{
    public string stageName; // Name of the stage
    public List<Sprite> symbols; // List of symbols for this stage
    public List<string> meanings; // Corresponding meanings for symbols
    [TextArea] public List<string> tips; // Learning tips for the stage
    public List<Sprite> distractorSymbols; // Additional symbols used as distractors
}
