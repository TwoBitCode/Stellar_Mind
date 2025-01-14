using UnityEngine;
using System;
using System.Collections;

public class PanelTransitionHandler : MonoBehaviour
{
    public float fadeDuration = 1f; // Time for fade effects

    public void ShowConnectedPanel(
        GameObject connectedPanel,
        GameObject disconnectedPanel,
        SparkEffectHandler sparkEffectHandler,
        Action onComplete)
    {
        connectedPanel.SetActive(true);
        disconnectedPanel.SetActive(false);

        // Trigger the explosion/spark effect
        Vector3 sparkPosition = connectedPanel.transform.position;
        sparkEffectHandler.TriggerSparkEffect(sparkPosition);

        StartCoroutine(SwitchToDisconnectedPanelAfterSpark(
            connectedPanel,
            disconnectedPanel,
            sparkEffectHandler.effectDuration,
            onComplete
        ));
    }

    private IEnumerator SwitchToDisconnectedPanelAfterSpark(
        GameObject connectedPanel,
        GameObject disconnectedPanel,
        float delay,
        Action onComplete)
    {
        yield return new WaitForSeconds(delay);

        connectedPanel.SetActive(false);
        disconnectedPanel.SetActive(true);

        onComplete?.Invoke();
    }
}
