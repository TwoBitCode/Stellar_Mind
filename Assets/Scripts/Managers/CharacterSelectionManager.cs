using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public enum CharacterType { Girl, Boy } // Enum for character types

    public static CharacterSelectionManager Instance { get; private set; }

    private CharacterDataManager characterDataManager;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize CharacterDataManager
        characterDataManager = new CharacterDataManager(); // Replace with FindObjectOfType if needed
    }

    public void SelectGirlAstronaut()
    {
        characterDataManager.SaveCharacterSelection(CharacterType.Girl);
        Debug.Log("Girl Astronaut selected.");
        TransitionToGame();
    }

    public void SelectBoyAstronaut()
    {
        characterDataManager.SaveCharacterSelection(CharacterType.Boy);
        Debug.Log("Boy Astronaut selected.");
        TransitionToGame();
    }

    private void TransitionToGame()
    {
        if (sceneTransitionManager != null)
        {
            sceneTransitionManager.LoadNextScene();
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not assigned to CharacterSelectionManager!");
        }
    }

    public CharacterType GetSelectedCharacter()
    {
        return characterDataManager.LoadCharacterSelection();
    }
}
