using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doorText;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private string[] sceneNames;
    [SerializeField] private string[] gameNames;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private Sprite completedGameSprite; // New sprite for completed games

    private AudioSource audioSource;

    private void Awake()
    {
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
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError("GameProgressManager is missing! Make sure it exists in the scene.");
            return;
        }

        var playerProgress = GameProgressManager.Instance.GetPlayerProgress();

        if (playerProgress == null)
        {
            Debug.LogError("PlayerProgress is null! Ensure it is initialized.");
            return;
        }

        if (playerProgress.gamesProgress == null)
        {
            Debug.LogError("gamesProgress dictionary is null! Initializing now.");
            playerProgress.gamesProgress = new Dictionary<int, GameProgress>();
        }

        for (int i = 0; i < doors.Length; i++)
        {
            int gameIndex = i;

            if (!playerProgress.gamesProgress.ContainsKey(gameIndex))
            {
                Debug.LogWarning($"Game index {gameIndex} not found. Initializing...");
                playerProgress.gamesProgress[gameIndex] = new GameProgress();
            }

            // Now safe to use gamesProgress[gameIndex]
            GameProgress gameProgress = playerProgress.gamesProgress[gameIndex];

            if (gameProgress.CheckIfCompleted())
            {
                SetDoorAsCompleted(doors[i]);
            }
        }
    }




    // Helper method to find the last completed stage
    private int GetLastCompletedStage(int gameIndex)
    {
        if (!GameProgressManager.Instance.GetPlayerProgress().gamesProgress.ContainsKey(gameIndex))
            return 0; // No progress, start from stage 0

        var gameProgress = GameProgressManager.Instance.GetPlayerProgress().gamesProgress[gameIndex];
        for (int i = 0; i < gameProgress.stages.Count; i++)
        {
            if (!gameProgress.stages[i].isCompleted)
                return i; // Return the first unfinished stage
        }
        return gameProgress.stages.Count; // All stages completed
    }


    private void SetDoorAsCompleted(GameObject door)
    {
        SpriteRenderer spriteRenderer = door.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && completedGameSprite != null)
        {
            spriteRenderer.sprite = completedGameSprite;
        }
    }

    private void AttachEventTriggers(GameObject door, int doorIndex, int gameIndex)
    {
        EventTrigger trigger = door.GetComponent<EventTrigger>() ?? door.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => ShowGameName(doorIndex));
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => ResetDoorText());
        trigger.triggers.Add(pointerExit);

        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => LoadScene(sceneNames[doorIndex], gameIndex)); // Uses correct gameIndex
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

    private void LoadScene(string sceneName, int gameIndex)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            PlaySound(clickSound);

            GameProgressManager.Instance.GetPlayerProgress().lastPlayedGame = gameIndex;
            GameProgressManager.Instance.GetPlayerProgress().lastPlayedStage = GetLastCompletedStage(gameIndex);
            GameProgressManager.Instance.SaveProgress();

            Debug.Log($"Loading {sceneName}, resuming from Stage {GameProgressManager.Instance.GetPlayerProgress().lastPlayedStage} in Game {gameIndex}");

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
