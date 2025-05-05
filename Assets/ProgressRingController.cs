using UnityEngine;
using UnityEngine.UI;

public class ProgressRingController : MonoBehaviour
{
    [SerializeField] private Image progressRing;
    [SerializeField] private float fillSpeed = 2f;

    private float targetFill = 0f;

    private const int totalStages = 12;

    private void Update()
    {
        if (progressRing == null) return;


        if (Mathf.Abs(progressRing.fillAmount - targetFill) > 0.001f)
        {
            progressRing.fillAmount = Mathf.Lerp(progressRing.fillAmount, targetFill, Time.deltaTime * fillSpeed);
        }
    }

    public void UpdateProgress(int completedStages)
    {
        targetFill = Mathf.Clamp01(completedStages / (float)totalStages);
    }
}
