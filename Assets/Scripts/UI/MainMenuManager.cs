using UnityEngine;
using UnityEngine.SceneManagement;  // For loading scenes

public class MainMenuController : MonoBehaviour
{
    // Function to load Game 1
    public void PlayGameMemoryGame()
    {
        SceneManager.LoadScene("MemoryGame");  // Replace with your actual game scene name
    }

    // Function to load Game 2
    public void PlayGamesortingAsteroids()
    {
        SceneManager.LoadScene("sortingAsteroids");  // Replace with your actual game scene name
    }
}
