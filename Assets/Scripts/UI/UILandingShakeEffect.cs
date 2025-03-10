using System.Collections;
using UnityEngine;

public class UILandingShakeEffect : MonoBehaviour
{
    public float duration = 0.5f; // ��� ������
    public float magnitude = 3f; // ����� ������ �������� (����� ����)

    [Header("Sound Settings")]
    public AudioSource audioSource; // ���� ������
    public AudioClip landingSound; // ���� ������

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition; // ����� �-localPosition
    }

    public void StartShake()
    {
        if (audioSource != null && landingSound != null)
        {
            audioSource.PlayOneShot(landingSound); // ����� ����� �� �����
        }

        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-0.5f, 0.5f) * magnitude;
            float y = Random.Range(-0.5f, 0.5f) * magnitude;

            rectTransform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPosition; // ����� �� ����� ������ ������
    }
}
