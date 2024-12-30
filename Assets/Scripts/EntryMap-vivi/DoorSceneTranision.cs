using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneTransition : MonoBehaviour
{
    [SerializeField] private string[] sceneNames; // שמות הסצנות לכל דלת

    public void LoadSceneByDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < sceneNames.Length)
        {
            SceneManager.LoadScene(sceneNames[doorIndex]);
        }
        else
        {
            Debug.LogError("Invalid door index or scene not assigned.");
        }
    }
}
