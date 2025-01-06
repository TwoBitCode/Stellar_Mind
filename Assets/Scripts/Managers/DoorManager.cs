using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doorText; // Text displayed for the door
    [SerializeField] private GameObject[] doors; // Array of door GameObjects
    [SerializeField] private string[] sceneNames; // Scene names corresponding to doors
    [SerializeField] private string[] gameNames; // Game names for display on hover
    [SerializeField] private AudioClip hoverSound; // Sound when hovering on a door
    [SerializeField] private AudioClip clickSound; // Sound when clicking a door

    private AudioSource audioSource; // Audio source component

    private void Awake()
    {
        // Ensure AudioSource is attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        InitializeDoors();
    }

    private void InitializeDoors()
    {
        // Check for setup issues
        if (doors == null || sceneNames == null || doors.Length != sceneNames.Length)
        {
            Debug.LogError("Mismatch between doors and sceneNames arrays. Ensure both are properly assigned.");
            return;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == null)
            {
                Debug.LogError($"Door at index {i} is missing!");
                continue;
            }

            // Attach event triggers to the door
            AttachEventTriggers(doors[i], i);
        }
    }

    private void AttachEventTriggers(GameObject door, int doorIndex)
    {
        EventTrigger trigger = door.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = door.AddComponent<EventTrigger>();
        }

        trigger.triggers.Clear();

        // Add PointerEnter event
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((data) => ShowGameName(doorIndex));
        trigger.triggers.Add(pointerEnter);

        // Add PointerExit event
        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((data) => ResetDoorText());
        trigger.triggers.Add(pointerExit);

        // Add PointerClick event
        EventTrigger.Entry pointerClick = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        pointerClick.callback.AddListener((data) => LoadScene(sceneNames[doorIndex]));
        trigger.triggers.Add(pointerClick);
    }

    private void ShowGameName(int doorIndex)
    {
        if (doorText != null && doorIndex >= 0 && doorIndex < gameNames.Length)
        {
            doorText.text = gameNames[doorIndex];
            PlaySound(hoverSound);
        }
    }

    private void ResetDoorText()
    {
        if (doorText != null)
        {
            doorText.text = "";
        }
    }

    private void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            PlaySound(clickSound);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not assigned!");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
