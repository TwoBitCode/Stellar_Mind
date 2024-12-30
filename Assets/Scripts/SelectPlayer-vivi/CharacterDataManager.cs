using UnityEngine;
public class CharacterDataManager
{
    private const string SelectedCharacterKey = "SelectedCharacter";

    public void SaveCharacterSelection(CharacterSelectionManager.CharacterType character)
    {
        PlayerPrefs.SetString(SelectedCharacterKey, character.ToString());
        PlayerPrefs.Save();
        Debug.Log($"Character selected: {character}");
    }

    public CharacterSelectionManager.CharacterType LoadCharacterSelection()
    {
        string savedCharacter = PlayerPrefs.GetString(SelectedCharacterKey, "Girl");
        return savedCharacter == "Boy"
            ? CharacterSelectionManager.CharacterType.Boy
            : CharacterSelectionManager.CharacterType.Girl;
    }
}
