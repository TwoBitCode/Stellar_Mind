using TMPro;
using UnityEngine;

public class InputValidator : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    //[SerializeField] private TMP_InputField targetScoreInput;
    [SerializeField] private WelcomeUIManager uiManager;

    public void ValidateInputs()
    {
        bool isPlayerNameValid = !string.IsNullOrEmpty(playerNameInput.text);
        ///bool isTargetScoreValid = int.TryParse(targetScoreInput.text, out int score) && score > 0;

        uiManager.UpdateStartButtonVisibility(isPlayerNameValid);
    }
}
