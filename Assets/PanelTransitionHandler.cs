using UnityEngine;
using System;
using System.Collections;

public class PanelTransitionHandler : MonoBehaviour
{
    public float fadeDuration = 1f; // Time for fade effects

    public void ShowConnectedPanel(GameObject connectedPanel, GameObject disconnectedPanel, SparkEffectHandler sparkEffectHandler, Action onComplete)
    {
        // Enable the connected panel
        connectedPanel.SetActive(true);
        disconnectedPanel.SetActive(false);

        // Trigger the spark effect
        Vector3 sparkPosition = connectedPanel.transform.position;
        sparkEffectHandler.TriggerSparkEffect(sparkPosition);

        // Immediately switch to the unconnected panel while the spark effect is active
        StartCoroutine(SwitchToDisconnectedPanelImmediately(connectedPanel, disconnectedPanel, sparkEffectHandler.effectDuration, onComplete));
    }

    private IEnumerator SwitchToDisconnectedPanelImmediately(GameObject connectedPanel, GameObject disconnectedPanel, float delay, Action onComplete)
    {
        // Disable the connected panel
        connectedPanel.SetActive(false);

        // Enable the unconnected panel
        disconnectedPanel.SetActive(true);

        // Wait for the spark effect duration before invoking the completion callback
        yield return new WaitForSeconds(delay);

        onComplete?.Invoke();
    }
}
