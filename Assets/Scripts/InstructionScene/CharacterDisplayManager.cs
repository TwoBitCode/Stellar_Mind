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
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");

        if (selectedCharacter == "Girl")
        {
            characterImage.sprite = girlSprite;
        }
        else if (selectedCharacter == "Boy")
        {
            characterImage.sprite = boySprite;
        }
        else
        {
            Debug.LogError("Invalid character selection or missing PlayerPrefs key.");
        }
    }
}
