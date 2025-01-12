using System.Collections.Generic;
using UnityEngine;

public class AsteroidChallengeManager : MonoBehaviour
{
    [SerializeField] private List<AsteroidChallenge> asteroidChallenges;
    private int currentChallengeIndex = 0;

    public AsteroidChallenge CurrentChallenge => asteroidChallenges[currentChallengeIndex];

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
