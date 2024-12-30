using UnityEngine;
using UnityEngine.UI;

public class CharacterDisplayManager : MonoBehaviour
{
    [SerializeField] private Image characterImage; // UI Image for the character
    [SerializeField] private Sprite girlSprite; // Girl astronaut sprite
    [SerializeField] private Sprite boySprite;  // Boy astronaut sprite

    private void Start()
    {
        LoadCharacterSprite();
    }

    private void LoadCharacterSprite()
    {
        // Fetch the selected character from PlayerPrefs
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");

        if (selectedCharacter == "Girl")
        {
            characterImage.sprite = girlSprite; // Assign the girl sprite
        }
        else if (selectedCharacter == "Boy")
        {
            characterImage.sprite = boySprite; // Assign the boy sprite
        }
        else
        {
            Debug.LogError("Invalid character selection or missing PlayerPrefs key.");
        }

        // Debug message for confirmation
        Debug.Log($"Character Loaded: {selectedCharacter}");
    }
}
