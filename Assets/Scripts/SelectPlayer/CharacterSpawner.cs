using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public GameObject girlAstronautPrefab;
    public GameObject boyAstronautPrefab;

    private void Start()
    {
        SpawnSelectedCharacter();
    }

    private void SpawnSelectedCharacter()
    {
        if (CharacterSelectionManager.GetSelectedCharacter() == CharacterSelectionManager.CharacterType.Girl)
        {
            Instantiate(girlAstronautPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(boyAstronautPrefab, transform.position, Quaternion.identity);
        }
    }
}
