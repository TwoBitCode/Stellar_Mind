using System.Collections.Generic;
using UnityEngine;

public class AsteroidChallengeManager : MonoBehaviour
{
    [Header("Asteroid Challenges")]
    [SerializeField] // Ensures the field is serialized and visible in the Inspector
    private List<AsteroidChallenge> asteroidChallenges = new List<AsteroidChallenge>();

    private int currentChallengeIndex = 0;

    public AsteroidChallenge CurrentChallenge
    {
        get
        {
            if (currentChallengeIndex < asteroidChallenges.Count)
            {
                return asteroidChallenges[currentChallengeIndex];
            }

            Debug.LogWarning("No more challenges available!");
            return null;
        }
    }

    public bool HasMoreChallenges => currentChallengeIndex < asteroidChallenges.Count;

    public void AdvanceToNextChallenge()
    {
        currentChallengeIndex++;

        if (!HasMoreChallenges)
        {
            Debug.Log("All asteroid challenges completed!");
        }
    }
}
