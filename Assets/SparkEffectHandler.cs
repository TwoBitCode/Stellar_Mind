using UnityEngine;

public class SparkEffectHandler : MonoBehaviour
{
    public GameObject sparkEffectPrefab; // Assign the SparkEffect prefab
    public float effectDuration = 1f;

    public void TriggerSparkEffect(Vector3 position)
    {
        if (sparkEffectPrefab == null)
        {
            Debug.LogError("SparkEffect prefab is not assigned!");
            return;
        }

        // Instantiate the spark effect at the specified position
        GameObject spark = Instantiate(sparkEffectPrefab, position, Quaternion.identity);

        // Ensure the spark effect destroys itself after its duration
        ParticleSystem particleSystem = spark.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            Destroy(spark, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        }
        else
        {
            // Fallback: Destroy the spark effect after a default duration
            Destroy(spark, 2f);
        }
    }

}
