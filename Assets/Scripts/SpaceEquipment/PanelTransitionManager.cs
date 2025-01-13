using System.Collections;
using UnityEngine;

public class PanelTransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    public GameObject transitionCanvas; // The transition canvas GameObject
    public CanvasGroup transitionCanvasGroup; // Canvas group for fade effects
    public float fadeDuration = 1f; // Duration of the fade effect

    private void Start()
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(false); // Ensure the transition canvas is initially disabled
        }
    }

    /// <summary>
    /// Smoothly transitions from the current panel to the next panel.
    /// </summary>
    /// <param name="currentPanel">The current active panel.</param>
    /// <param name="nextPanel">The next panel to activate.</param>
    public void TransitionPanels(GameObject currentPanel, GameObject nextPanel)
    {
        StartCoroutine(PerformPanelTransition(currentPanel, nextPanel));
    }

    private IEnumerator PerformPanelTransition(GameObject currentPanel, GameObject nextPanel)
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(true); // Ensure the transition canvas is enabled
        }

        // Fade out
        yield return Fade(1f);

        // Switch panels
        currentPanel.SetActive(false);
        nextPanel.SetActive(true);

        // Fade in
        yield return Fade(0f);

        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(false); // Disable the transition canvas after fading in
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }
}
