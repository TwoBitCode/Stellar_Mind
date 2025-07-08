using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayStrategyAudioButton : MonoBehaviour
{
    private Button button;
    private StrategyManager strategyManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        strategyManager = FindAnyObjectByType<StrategyManager>();

        if (strategyManager == null)
        {
            Debug.LogError("No StrategyManager found in the scene!");
            return;
        }

        button.onClick.AddListener(() =>
        {
            strategyManager.PlayCurrentStrategyAudio();
        });
    }
}
