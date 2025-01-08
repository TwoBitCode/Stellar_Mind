using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Player Settings")]
    public GameObject player; // Reference to the player GameObject
    public float moveDuration = 0.5f;
    public AnimationCurve movementCurve; // Optional movement smoothing

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of PlayerController detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public void SetPosition(Vector3 position)
    {
        player.transform.position = position;
    }

    public void MoveTo(Vector3 targetPosition, System.Action onComplete)
    {
        StartCoroutine(MovePlayerToPosition(targetPosition, onComplete));
    }

    private IEnumerator MovePlayerToPosition(Vector3 targetPosition, System.Action onComplete)
    {
        Vector3 startPosition = player.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            float smoothT = movementCurve != null ? movementCurve.Evaluate(t) : t;

            player.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPosition;

        onComplete?.Invoke();
    }
}
