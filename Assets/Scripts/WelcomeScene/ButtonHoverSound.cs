using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip hoverSound; // Assign different sounds per object
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound); // Play the specific hover sound
        }
        else
        {
            Debug.LogWarning("AudioSource or HoverSound is missing!");
        }
    }
}
