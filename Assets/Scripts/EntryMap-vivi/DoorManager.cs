using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doorText; // Text displayed for the door
    [SerializeField] private string[] gameNames; // Game names for each door
    [SerializeField] private string[] sceneNames; // Scene names for each door
    [SerializeField] private GameObject[] doors; // Door objects
    [SerializeField] private AudioClip hoverSound; // Sound when hovering on a door
    [SerializeField] private AudioClip clickSound; // Sound when clicking a door
    private AudioSource audioSource; // Audio source component

    private void Start()
    {
        ResetDoorText();

        // Get or add an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void ResetDoorText()
    {
        doorText.text = ""; // Default to empty text
    }

    public void OnMouseEnterDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < gameNames.Length)
        {
            doorText.text = gameNames[doorIndex]; // Display the corresponding game name
            PlaySound(hoverSound); // Play hover sound
        }
    }

    public void OnMouseExitDoor()
    {
        ResetDoorText(); // Reset the text when the mouse exits the door
    }

    public void OnMouseClickDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < sceneNames.Length)
        {
            PlaySound(clickSound); // Play click sound
            SceneManager.LoadScene(sceneNames[doorIndex]); // Load the scene corresponding to the door
        }
        else
        {
            Debug.LogError("Invalid door index or scene not assigned.");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip); // Play the specified sound clip
        }
    }
}
