using TMPro;
using UnityEngine;

public class RTLTextFixer : MonoBehaviour
{
    public void FixText(TextMeshProUGUI textComponent)
    {
        if (textComponent != null)
        {
            textComponent.isRightToLeftText = true; // ����� ����� ����� �����
            textComponent.alignment = TextAlignmentOptions.Right; // ����� �����
        }
    }
}
