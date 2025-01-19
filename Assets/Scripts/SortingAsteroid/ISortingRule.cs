using UnityEngine;

public interface ISortingRule
{
    string GetCategory(GameObject asteroid); // Determine the category for an asteroid
    void ApplyVisuals(GameObject asteroid); // Apply visual changes (e.g., color or size)
    string GetRandomType();
}
