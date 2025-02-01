using System.Collections.Generic;
using UnityEngine;

public class AsteroidChallengeManager : MonoBehaviour
{
    [Header("Asteroid Challenges")]
    [SerializeField]
    private List<AsteroidChallenge> asteroidChallenges = new List<AsteroidChallenge>();

    private int currentChallengeIndex = 0;

    public AsteroidChallenge CurrentChallenge
    {
        get
        {
            if (currentChallengeIndex >= 0 && currentChallengeIndex < asteroidChallenges.Count)
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
        if (HasMoreChallenges)
        {
            currentChallengeIndex++;
            Debug.Log($"Advancing to challenge {currentChallengeIndex}");
        }
        else
        {
            Debug.Log("All asteroid challenges completed!");
        }
    }

    /// <summary>
    /// Sets the current challenge index to resume from a specific stage.
    /// </summary>
    public void SetCurrentChallengeIndex(int index)
    {
        if (index >= 0 && index < asteroidChallenges.Count)
        {
            currentChallengeIndex = index;
            Debug.Log($"Resuming challenge at index {currentChallengeIndex}");
        }
        else
        {
            Debug.LogError($"Invalid challenge index: {index}. Cannot resume.");
        }
    }

    /// <summary>
    /// Resets the challenge index to the beginning.
    /// </summary>
    public void ResetChallenges()
    {
        currentChallengeIndex = 0;
        Debug.Log("Challenges have been reset.");
    }

    /// <summary>
    /// Exposes the list of challenges.
    /// </summary>
    public List<AsteroidChallenge> Challenges => asteroidChallenges;
    public int GetChallengeCount()
    {
        return asteroidChallenges.Count;
    }

}
