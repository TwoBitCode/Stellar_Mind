using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipBackground;  // Assign in Inspector
    [SerializeField] private TMP_Text tooltipText;             // Assign in Inspector

    private string currentMessage;  // changes based on the button that uses it, so doesn't need to be shown
    private RectTransform currentButton;  // changes based on the button that uses it, so doesn't need to be shown

    [SerializeField] private bool isHoveringButton = false;
    [SerializeField] private bool isHoveringTooltip = false;

    [SerializeField] private CanvasGroup tooltipCanvasGroup;


    void Start()
    {
        tooltipCanvasGroup = tooltipBackground.GetComponent<CanvasGroup>(); //

        if (!tooltipCanvasGroup)
        {
            tooltipCanvasGroup = tooltipBackground.gameObject.AddComponent<CanvasGroup>();
        }

        tooltipBackground.gameObject.SetActive(false); // Hide by default
    }


    public void SetTooltipMessage(string message)
    {
        currentMessage = message; // Store the message
    }

    public void ShowTooltip(GameObject button)
    {
        if (string.IsNullOrEmpty(currentMessage)) return;

        currentButton = button.GetComponent<RectTransform>();

        tooltipCanvasGroup.blocksRaycasts = false; // Ensure it doesn't block raycasts
        tooltipText.text = currentMessage;
        tooltipBackground.gameObject.SetActive(true);
    }

    public void OnTooltipEnter()
    {
        isHoveringTooltip = true;
    }

    public void OnTooltipExit()
    {
        isHoveringTooltip = false;
        CheckHideTooltip();
    }

    public void OnButtonExit()
    {
        isHoveringButton = false;
        tooltipCanvasGroup.blocksRaycasts = true; // Allow raycasts again
        CheckHideTooltip();
    }

    private void CheckHideTooltip()
    {
        if (!isHoveringButton && !isHoveringTooltip)
        {
            tooltipBackground.gameObject.SetActive(false);
        }
    }
}
