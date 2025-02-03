using System.Collections;
using UnityEngine;

public class ShakeUI : MonoBehaviour
{
    public RectTransform uiElement; // The UI element to shake (assign in Inspector)
    public float shakeDuration = 1f; // Duration of the shake
    public float shakeMagnitude = 5f; // How strong the shake is

    private Vector3 originalPosition;

    private void Start()
    {
        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>(); // Auto-assign if not set
        }
        originalPosition = uiElement.anchoredPosition;
    }

    public void StartShake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            uiElement.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiElement.anchoredPosition = originalPosition; // Reset position
    }
}
