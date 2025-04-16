using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public AudioSource audioSource; // Sound source
    public AudioClip buttonClickSound; // Sound to play on click
    public AudioClip hoverSound; // Sound to play on hover

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(buttonClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
