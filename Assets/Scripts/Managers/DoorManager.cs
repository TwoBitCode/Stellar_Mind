using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doorText;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private string[] sceneNames;
    [SerializeField] private string[] gameNames;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private Sprite completedGameSprite;
    [SerializeField] private string defaultDoorText;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    private IEnumerator Start()
    {
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError("GameProgressManager is missing! Cannot initialize doors.");
            yield break;
        }

        yield return null;

        if (doorText != null)
        {
            doorText.text = defaultDoorText; 
        }

        Debug.Log("Finished loading progress, now initializing doors...");
        InitializeDoors();
    }

    private void InitializeDoors()
    {
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (playerProgress == null || playerProgress.gamesProgress == null)
        {
            Debug.LogError("PlayerProgress or gamesProgress is null! Cannot initialize doors.");
            return;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i] == null)
            {
                Debug.LogError($"Door {i} is missing in the scene!");
                continue;
            }

            if (!playerProgress.gamesProgress.ContainsKey(i))
            {
                playerProgress.gamesProgress[i] = new GameProgress();
            }

            bool isCompleted = playerProgress.gamesProgress[i].CheckIfCompleted();

            if (isCompleted)
            {
                SetDoorAsCompleted(doors[i]);
            }

            AttachEventTriggers(doors[i], i, isCompleted);
        }

        GameProgressManager.Instance.SaveProgress();
    }

    private void SetDoorAsCompleted(GameObject door)
    {
        if (door.TryGetComponent(out UnityEngine.UI.Image image) && completedGameSprite != null)
        {
            image.sprite = completedGameSprite; 
        }
        else
        {
            Debug.LogError($"Failed to update door sprite! Missing Image component on {door.name}");
        }
    }

    private void AttachEventTriggers(GameObject door, int gameIndex, bool isCompleted)
    {
        if (!door.TryGetComponent(out EventTrigger trigger))
        {
            trigger = door.AddComponent<EventTrigger>();
        }

        trigger.triggers.Clear();

        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => ShowGameName(gameIndex, isCompleted));
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) => ResetDoorText());

        if (!isCompleted)
        {
            AddEventTrigger(trigger, EventTriggerType.PointerClick, (data) => LoadScene(gameIndex));
        }
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    public void ShowGameName(int doorIndex, bool isCompleted)
    {
        if (doorText != null)
        {
            if (isCompleted)
            {
                doorText.text = "כל הכבוד! השלמת את המשחק!";
            }
            else if (doorIndex >= 0 && doorIndex < gameNames.Length)
            {
                doorText.text = gameNames[doorIndex];
            }

            PlaySound(hoverSound);
        }
    }

    public void ResetDoorText()
    {
        if (doorText != null)
        {
            doorText.text = defaultDoorText;
        }
    }

    public void LoadScene(int doorIndex)
    {
        if (doorIndex < 0 || doorIndex >= sceneNames.Length || string.IsNullOrEmpty(sceneNames[doorIndex]))
        {
            Debug.LogError("Invalid scene index or missing scene name!");
            return;
        }

        PlaySound(clickSound);

        var playerProgress = GameProgressManager.Instance.playerProgress;
        playerProgress.lastPlayedGame = doorIndex;
        playerProgress.lastPlayedStage = GetLastCompletedStage(doorIndex);

        GameProgressManager.Instance.SaveProgress();

        Debug.Log($"Loading {sceneNames[doorIndex]}, resuming from Stage {playerProgress.lastPlayedStage} in Game {doorIndex}");

        DestroyMiniGameManagers();

        SceneManager.LoadScene(sceneNames[doorIndex]);
    }

    private int GetLastCompletedStage(int gameIndex)
    {
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (!playerProgress.gamesProgress.ContainsKey(gameIndex))
            return 0;

        var gameProgress = playerProgress.gamesProgress[gameIndex];
        for (int i = 0; i < gameProgress.stages.Count; i++)
        {
            if (!gameProgress.stages[i].isCompleted)
                return i;
        }
        return gameProgress.stages.Count;
    }

    private void DestroyMiniGameManagers()
    {
        string[] persistentManagers = { "MemoryGameManager", "AudioFeedbackManager", "EquipmentRecoveryGameManager", "EquipmentRecoveryUIManager" };

        foreach (string managerName in persistentManagers)
        {
            GameObject[] existingManagers = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject manager in existingManagers)
            {
                if (manager.name == managerName)
                {
                    Debug.Log($"Destroying {managerName} before switching scenes.");
                    Destroy(manager);
                }
            }
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
