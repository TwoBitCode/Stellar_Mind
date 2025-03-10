using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIImageBlink : MonoBehaviour
{
    public Image targetImage;
    public float blinkSpeed = 1f; // ������ ������

    private void Start()
    {
        if (targetImage != null)
        {
            StartCoroutine(BlinkEffect());
        }
    }

    private IEnumerator BlinkEffect()
    {
        while (true)
        {
            // ����� �����
            for (float alpha = 1f; alpha >= 0f; alpha -= Time.deltaTime * blinkSpeed)
            {
                SetAlpha(alpha);
                yield return null;
            }

            // ����� �����
            for (float alpha = 0f; alpha <= 1f; alpha += Time.deltaTime * blinkSpeed)
            {
                SetAlpha(alpha);
                yield return null;
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        if (targetImage != null)
        {
            Color color = targetImage.color;
            color.a = alpha;
            targetImage.color = color;
        }
    }
}
