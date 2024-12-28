using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy } // Enum for character types

    public static CharacterType SelectedCharacter; // Store the selected character

    [SerializeField] private SceneTransitionManager sceneTransitionManager; // Reference to SceneTransitionManager

    // Sets the selected character to Girl and saves it
    public void SelectGirlAstronaut()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Girl");
        Debug.Log("Girl Astronaut selected.");
        TransitionToGame();
    }

    // Sets the selected character to Boy and saves it
    public void SelectBoyAstronaut()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Boy");
        Debug.Log("Boy Astronaut selected.");
        TransitionToGame();
    }

    // Uses SceneTransitionManager to handle the transition
    private void TransitionToGame()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadNextScene(); // Delegate transition logic
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned to CharacterSelectionManager!");
        }
    }

    // Retrieves the selected character from PlayerPrefs
    public static CharacterType GetSelectedCharacter()
    {
        string savedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Girl");
        return savedCharacter == "Boy" ? CharacterType.Boy : CharacterType.Girl;
    }
}
