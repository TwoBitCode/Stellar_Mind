using UnityEngine;

public class TubeFlyAwayUI : MonoBehaviour
{
    public float flySpeed = 200f; // Base speed of flying
    public Vector2 flyRange = new Vector2(300f, 300f); // How far tubes can fly
    public float flipInterval = 0.2f; // Time between flips

    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private float flipTimer = 0f;
    private bool flipped = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Set the first random target position
        SetNewTargetPosition();
    }

    void Update()
    {
        // Move the tube toward the target position
        rectTransform.anchoredPosition = Vector2.MoveTowards(
            rectTransform.anchoredPosition,
            targetPosition,
            flySpeed * Time.deltaTime
        );

        // If the tube reaches the target position, set a new one
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < 1f)
        {
            SetNewTargetPosition();
        }

        // Handle flipping effect
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipInterval)
        {
            flipped = !flipped; // Toggle flip state
            rectTransform.localScale = new Vector3(flipped ? -1f : 1f, 1f, 1f); // Flip horizontally
            flipTimer = 0f;
        }
    }

    private void SetNewTargetPosition()
    {
        targetPosition = new Vector2(
            UnityEngine.Random.Range(-flyRange.x, flyRange.x),
            UnityEngine.Random.Range(-flyRange.y, flyRange.y)
        );
    }
}
