using UnityEngine;

public class LineResizer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public RectTransform startUIElement;
    public RectTransform endUIElement;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateLinePositions();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateLinePositions();
        }
    }

    private int lastScreenWidth, lastScreenHeight;

    void UpdateLinePositions()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        Vector3 startPos = mainCamera.ScreenToWorldPoint(startUIElement.position);
        Vector3 endPos = mainCamera.ScreenToWorldPoint(endUIElement.position);

        startPos.z = 0;
        endPos.z = 0;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
