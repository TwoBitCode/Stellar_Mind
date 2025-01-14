using UnityEngine;
using System;
using System.Collections;

public class PanelTransitionHandler : MonoBehaviour
{
    public float fadeDuration = 1f; // Time for fade effects

    public void ShowConnectedPanel(GameObject connectedPanel, GameObject disconnectedPanel, SparkEffectHandler sparkEffectHandler, Action onComplete)
    {
        connectedPanel.SetActive(true);
        disconnectedPanel.SetActive(false);

        // Trigger spark effect
        Vector3 sparkPosition = connectedPanel.transform.position; // Place spark effect at panel center
        sparkEffectHandler.TriggerSparkEffect(sparkPosition);

        // Wait for the spark effect duration, then switch panels
        StartCoroutine(SwitchToDisconnectedPanelAfterSpark(connectedPanel, disconnectedPanel, sparkEffectHandler.effectDuration, onComplete));
    }

    private IEnumerator SwitchToDisconnectedPanelAfterSpark(GameObject connectedPanel, GameObject disconnectedPanel, float delay, Action onComplete)
    {
        // Wait for the spark effect duration
        yield return new WaitForSeconds(delay);

        // Disable connected panel and enable disconnected panel
        connectedPanel.SetActive(false);
        disconnectedPanel.SetActive(true);

        onComplete?.Invoke();
    }


    private IEnumerator FadeOutPanelWithSpark(GameObject panel, SparkEffectHandler sparkEffectHandler, Action onComplete)
    {
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is missing!");
            yield break;
        }

        // Trigger the spark effect at the center of the connected panel
        Vector3 sparkPosition = panel.transform.position;
        sparkEffectHandler.TriggerSparkEffect(sparkPosition);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha; // Gradually fade out the panel
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        onComplete?.Invoke();
    }
}
