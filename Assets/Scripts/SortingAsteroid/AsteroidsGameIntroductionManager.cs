using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class AsteroidsGameIntroductionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform panel; // Intro panel
    [SerializeField] private GameObject dialoguePanel; // Dialogue panel
    [SerializeField] private TextMeshProUGUI dialogueText; // Text for dialogue
    [SerializeField] private GameObject startButton; // Button for starting the game
    [SerializeField] private GameObject asteroidPrefab; // Asteroid prefab for falling effect

    [Header("Introduction Settings")]
    [SerializeField] private string[] dialogueLines; // Dialogue lines for introduction
    [SerializeField] private float typingSpeed = 0.05f; // Speed of typing effect
    [SerializeField] private int numberOfAsteroids = 10; // Number of asteroids to spawn
    [SerializeField] private Vector2 asteroidFallDurationRange = new Vector2(1f, 3f); // Duration range for falling asteroids
    [SerializeField] private AudioSource asteroidFallSound; // Sound for asteroid falling
    private Action onComplete; // Callback for completion

    private bool isSpawningAsteroids = true; // Flag to control asteroid spawning

    public void PlayIntroduction(Action onIntroductionComplete)
    {
        Debug.Log("PlayIntroduction called!"); // Debug check

        int currentGameIndex = 3; // Assuming this is the 4th game (0-based index)
        if (GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].hasStarted)
        {
            Debug.Log("Skipping introduction because the game has already started.");
            onIntroductionComplete?.Invoke(); // Go straight to the game
            return;
        }

        // Mark this game as started
        GameProgressManager.Instance.playerProgress.gamesProgress[currentGameIndex].hasStarted = true;
        GameProgressManager.Instance.SaveProgress();

        onComplete = onIntroductionComplete; // Store callback
        startButton.SetActive(false); // Hide Start button initially

        // Start falling asteroids and dialogue sequence
        TriggerFallingAsteroids();
        StartCoroutine(PlayDialogueSequence());
    }



    private IEnumerator PlayDialogueSequence()
    {
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            yield return StartCoroutine(TypeDialogue(dialogueLines[i]));
            yield return new WaitForSeconds(1f); // Pause between lines
        }

        // Wait for falling asteroids to finish, then show the Start button
        yield return new WaitForSeconds(1f);
        startButton.SetActive(true);
    }

    private IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = ""; // Clear text
        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait to simulate typing
        }
        yield return null; // Ensure coroutine completes
    }


    private void TriggerFallingAsteroids()
    {
        if (asteroidFallSound != null) asteroidFallSound.Play();
        StartCoroutine(SpawnAsteroidsWithDelay());
    }

    private IEnumerator SpawnAsteroidsWithDelay()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            if (!isSpawningAsteroids) yield break; // Stop spawning if the flag is false

            SpawnAsteroid(); // Spawn a single asteroid
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.7f)); // Random delay between spawns
        }
    }

    private void SpawnAsteroid()
    {
        GameObject asteroid = Instantiate(asteroidPrefab, panel);
        RectTransform rectTransform = asteroid.GetComponent<RectTransform>();

        // Randomize start position along the top of the panel
        Vector2 startPosition = new Vector2(
            UnityEngine.Random.Range(-panel.rect.width / 2, panel.rect.width / 2),
            panel.rect.height / 2 + 50f
        );

        // Randomize end position slightly below the bottom of the panel
        Vector2 endPosition = new Vector2(
            startPosition.x + UnityEngine.Random.Range(-50f, 50f), // Slight horizontal variation
            -panel.rect.height / 2 - 50f
        );

        rectTransform.anchoredPosition = startPosition;

        // Apply random rotation and scale for realism
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
        rectTransform.localScale = Vector3.one * UnityEngine.Random.Range(0.8f, 1.2f);

        // Start the falling animation
        float fallDuration = UnityEngine.Random.Range(asteroidFallDurationRange.x, asteroidFallDurationRange.y);
        StartCoroutine(AnimateAsteroidFall(rectTransform, startPosition, endPosition, fallDuration));
    }

    private IEnumerator AnimateAsteroidFall(RectTransform asteroid, Vector2 start, Vector2 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            asteroid.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(asteroid.gameObject);
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Transitioning to game...");
        isSpawningAsteroids = false; // Stop asteroid spawning
        startButton.SetActive(false); // Hide the Start button
        panel.gameObject.SetActive(false); // Hide the intro panel
        dialoguePanel.gameObject.SetActive(false); // Hide the intro panel

        // Stop the introduction sound
        if (asteroidFallSound != null && asteroidFallSound.isPlaying)
        {
            asteroidFallSound.Stop();
        }

        onComplete?.Invoke(); // Notify GameManager to show stage instructions
    }
    public void HideIntroduction()
    {
        Debug.Log("Hiding Introduction UI...");

        isSpawningAsteroids = false; // Stop spawning asteroids

        if (asteroidFallSound != null && asteroidFallSound.isPlaying)
        {
            asteroidFallSound.Stop(); // Stop asteroid sound
        }

        if (panel != null)
        {
            panel.gameObject.SetActive(false); // Hide the intro panel
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // Hide the dialogue panel
        }

        if (startButton != null)
        {
            startButton.SetActive(false); // Hide the Start button
        }
    }

}
