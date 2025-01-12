using UnityEngine;

[CreateAssetMenu(menuName = "Asteroid Challenges/Size Sorting Rule")]
public class SizeSortingRule : ScriptableObject, ISortingRule
{
    [Tooltip("Size threshold to determine 'Small' or 'Large' categories")]
    public float largeSizeThreshold = 1.5f;

    public string GetCategory(GameObject item)
    {
        float size = Random.Range(0.5f, 2.5f);
        return size >= largeSizeThreshold ? "Large" : "Small";
    }

    public void ApplyVisuals(GameObject item)
    {
        float size = Random.Range(0.5f, 2.5f);
        item.transform.localScale = Vector3.one * size;
    }
}
