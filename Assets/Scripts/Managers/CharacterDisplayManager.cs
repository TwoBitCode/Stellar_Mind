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

        if (GameProgressManager.Instance == null || GameProgressManager.Instance.playerProgress == null)
        {
            Debug.LogError(" GameProgressManager or playerProgress is missing! Cannot load character.");
            return;
        }

        string selectedCharacter = GameProgressManager.Instance.playerProgress.selectedCharacter;

        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.LogError("No character selection found! Defaulting to Boy.");
            selectedCharacter = "Boy";
        }


        if (selectedCharacter == "Girl")
        {
            characterImage.sprite = girlSprite;
        }
        else
        {
            characterImage.sprite = boySprite;
        }

        Debug.Log($"Character Loaded: {selectedCharacter}");
    }
}
