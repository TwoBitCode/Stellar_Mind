using UnityEngine;
using TMPro;

public class AlienUIManager : MonoBehaviour
{
    public static AlienUIManager Instance;

    [Header("Alien UI Elements")]
    public TextMeshProUGUI alienText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void UpdateAlienText(string message)
    {
        if (alienText != null)
        {
            alienText.text = message;
        }
    }

}
