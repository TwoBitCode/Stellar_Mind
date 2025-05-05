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
    [SerializeField] private AudioClip[] girlHoverAudioClips; // One clip per door
    [SerializeField] private AudioClip[] boyHoverAudioClips;  // One clip per door
    [SerializeField] private ProgressRingController progressRingController;

    [Header("End Game Panel Settings")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private string endSceneName = "EndScene";

    [Header("Falling Objects Settings")]
    [SerializeField] private GameObject[] fallingPrefabs; // Prefabs of stars, coins, etc.
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private int totalObjects = 50; // Number of falling objects
    [SerializeField] private float spawnRate = 0.1f; // Interval between spawns
    //[SerializeField] private float fallSpeed = 300f; // Speed of falling objects
    [SerializeField] private float fallDuration = 3f; // Duration of fall


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

        if (CheckIfAllGamesCompleted())
        {
            ShowEndGamePanel();
        }
    }
    private void ShowEndGamePanel()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
            StartCoroutine(SpawnFallingObjects());
            StartCoroutine(TransitionToEndScene());
        }
    }
    private IEnumerator SpawnFallingObjects()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnObject()
    {
        if (fallingPrefabs.Length == 0 || spawnArea == null) return;

        GameObject prefab = fallingPrefabs[Random.Range(0, fallingPrefabs.Length)];
        GameObject newObject = Instantiate(prefab, spawnArea);

        RectTransform objTransform = newObject.GetComponent<RectTransform>();

        float randomX = Random.Range(-spawnArea.sizeDelta.x / 2, spawnArea.sizeDelta.x / 2);
        float startY = spawnArea.sizeDelta.y / 2;

        objTransform.anchoredPosition = new Vector2(randomX, startY);

        StartCoroutine(FallAnimation(objTransform));
    }

    private IEnumerator FallAnimation(RectTransform objTransform)
    {
        float elapsedTime = 0f;
        Vector2 startPos = objTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, -spawnArea.sizeDelta.y / 2);

        while (elapsedTime < fallDuration)
        {
            objTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(objTransform.gameObject);
    }

    private IEnumerator TransitionToEndScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(endSceneName);
    }
    private bool CheckIfAllGamesCompleted()
    {
        var playerProgress = GameProgressManager.Instance.playerProgress;
        if (playerProgress == null || playerProgress.gamesProgress == null) return false;

        foreach (var game in playerProgress.gamesProgress.Values)
        {
            if (!game.isCompleted) return false;
        }
        return true;
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

            // Ensure the correct `GameProgress` type is assigned
            if (!playerProgress.gamesProgress.ContainsKey(i))
            {
                // If this is the Asteroid Game (index 3), use `GameProgress(3)`
                playerProgress.gamesProgress[i] = new GameProgress(i);
            }

            bool isCompleted = playerProgress.gamesProgress[i].CheckIfCompleted();

            if (isCompleted)
            {
                SetDoorAsCompleted(doors[i]);
            }

            AttachEventTriggers(doors[i], i, isCompleted);
        }

        GameProgressManager.Instance.SaveProgress();
        if (progressRingController != null)
        {
            int completedStages = CountCompletedStages();
            progressRingController.UpdateProgress(completedStages);
        }

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

            PlayHoverSound(doorIndex, isCompleted);
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

        // Check if game progress exists
        if (playerProgress == null || !playerProgress.gamesProgress.ContainsKey(gameIndex))
        {
            Debug.LogWarning($"No progress found for game {gameIndex}. Returning stage 0.");
            return 0; // Default to stage 0 if no progress exists
        }

        var gameProgress = playerProgress.gamesProgress[gameIndex];

        // Check if there are any stages in the game
        if (gameProgress.stages == null || gameProgress.stages.Count == 0)
        {
            Debug.LogWarning($"Game {gameIndex} has no stages recorded yet. Returning stage 0.");
            return 0; // Default to 0 if no stages exist
        }

        // Find the last incomplete stage
        for (int i = 0; i < gameProgress.stages.Count; i++)
        {
            if (!gameProgress.stages[i].isCompleted)
            {
                return i;
            }
        }

        // If all stages are completed, return the total stage count
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
    public void OpenGameReport()
    {
        PlayerPrefs.SetString("LastSceneBeforeReport", SceneManager.GetActiveScene().name); // Save current scene
        PlayerPrefs.Save();
        SceneManager.LoadScene("Player report"); // Change to your actual report scene name
    }
    private void PlayHoverSound(int doorIndex, bool isCompleted)
    {
        if (audioSource == null || isCompleted)
        {
            PlaySound(hoverSound); // Play regular hover sound if completed
            return;
        }

        string selectedCharacter = GameProgressManager.Instance.playerProgress.selectedCharacter;

        if (selectedCharacter == "Girl")
        {
            if (girlHoverAudioClips != null && doorIndex >= 0 && doorIndex < girlHoverAudioClips.Length)
            {
                if (girlHoverAudioClips[doorIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.clip = girlHoverAudioClips[doorIndex];
                    audioSource.Play();
                }
            }
        }
        else if (selectedCharacter == "Boy")
        {
            if (boyHoverAudioClips != null && doorIndex >= 0 && doorIndex < boyHoverAudioClips.Length)
            {
                if (boyHoverAudioClips[doorIndex] != null)
                {
                    audioSource.Stop();
                    audioSource.clip = boyHoverAudioClips[doorIndex];
                    audioSource.Play();
                }
            }
        }
        else
        {
            PlaySound(hoverSound); // fallback sound
        }
    }
    private int CountCompletedStages()
    {
        int count = 0;
        var games = GameProgressManager.Instance?.playerProgress?.gamesProgress;

        if (games == null) return 0;

        foreach (var game in games.Values)
        {
            if (game.stages == null) continue;

            foreach (var stagePair in game.stages)
            {
                if (stagePair.Value.isCompleted)
                    count++;
            }
        }

        return count;
    }



}
