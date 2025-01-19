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
