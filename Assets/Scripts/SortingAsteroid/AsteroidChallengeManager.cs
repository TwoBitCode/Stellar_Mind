using System.Collections.Generic;
using UnityEngine;

public class AsteroidChallengeManager : MonoBehaviour
{
    [SerializeField] private List<AsteroidChallenge> asteroidChallenges;
    private int currentChallengeIndex = 0;

    public AsteroidChallenge CurrentChallenge
    {
        get
        {
            // Check if the index is valid before accessing the list
            if (currentChallengeIndex < asteroidChallenges.Count)
            {
                return asteroidChallenges[currentChallengeIndex];
            }

            Debug.LogWarning("No more challenges available!");
            return null; // Return null if out of bounds
        }
    }

    public void AdvanceToNextChallenge()
    {
        currentChallengeIndex++;
        if (currentChallengeIndex >= asteroidChallenges.Count)
        {
            Debug.Log("All asteroid challenges completed!");
            return;
        }

        Debug.Log($"Starting Asteroid Challenge: {CurrentChallenge.challengeName}");
    }
}
