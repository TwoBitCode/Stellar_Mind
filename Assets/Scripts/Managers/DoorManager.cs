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
        if (GameProgressManager.Instance == null)
        {
            Debug.LogError("GameProgressManager is missing! Cannot initialize doors.");
            return;
        }

        if (string.IsNullOrEmpty(GameProgressManager.Instance.GetPlayerProgress().playerName))
        {
            Debug.LogError("Player name is missing! Returning to welcome screen.");
            SceneManager.LoadScene("WelcomeScene");
            return;
        }

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

            // Attach event triggers to each door
            AttachEventTriggers(doors[i], gameIndex);


            // If the game is completed, update the door's appearance
            if (playerProgress.gamesProgress[gameIndex].CheckIfCompleted())
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

    private void AttachEventTriggers(GameObject door, int gameIndex)
    {
        EventTrigger trigger = door.GetComponent<EventTrigger>() ?? door.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        // Pointer Enter (Show Game Name)
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => ShowGameName(gameIndex));
        trigger.triggers.Add(pointerEnter);

        // Pointer Exit (Reset Text)
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => ResetDoorText());
        trigger.triggers.Add(pointerExit);

        // Pointer Click (Load Scene)
        EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClick.callback.AddListener((data) => LoadScene(gameIndex));
        trigger.triggers.Add(pointerClick);
    }




    public void ShowGameName(int doorIndex)
    {
        if (doorText != null && doorIndex >= 0 && doorIndex < gameNames.Length)
        {
            doorText.text = gameNames[doorIndex];
            PlaySound(hoverSound);
        }
    }

    public void ResetDoorText()
    {
        if (doorText != null)
        {
            doorText.text = "";
        }
    }

    public void LoadScene(int doorIndex)
    {
        if (!string.IsNullOrEmpty(sceneNames[doorIndex]))
        {
            PlaySound(clickSound);

            GameProgressManager.Instance.GetPlayerProgress().lastPlayedGame = doorIndex;
            GameProgressManager.Instance.GetPlayerProgress().lastPlayedStage = GetLastCompletedStage(doorIndex);
            GameProgressManager.Instance.SaveProgress();

            Debug.Log($"Loading {sceneNames[doorIndex]}, resuming from Stage {GameProgressManager.Instance.GetPlayerProgress().lastPlayedStage} in Game {doorIndex}");

            SceneManager.LoadScene(sceneNames[doorIndex]);
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
