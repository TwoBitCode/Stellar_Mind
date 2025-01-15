using UnityEngine;

public class TubeEffectManager : MonoBehaviour
{
    public GameObject tubePrefab; // Reference to the tube prefab
    public RectTransform panel; // Reference to the parent panel
    public int numberOfTubes = 10; // Number of tubes to create

    public void SpawnAndFlyTubes()
    {
        // Clear existing tubes
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        // Spawn tubes and trigger fly-away effect
        for (int i = 0; i < numberOfTubes; i++)
        {
            GameObject tube = Instantiate(tubePrefab, panel);
            RectTransform rectTransform = tube.GetComponent<RectTransform>();

            // Random initial position within the panel
            rectTransform.anchoredPosition = new Vector2(
                Random.Range(-panel.rect.width / 2, panel.rect.width / 2),
                Random.Range(-panel.rect.height / 2, panel.rect.height / 2)
            );

            // Add fly-away effect
            tube.AddComponent<TubeFlyAwayUI>();
        }
    }
}
