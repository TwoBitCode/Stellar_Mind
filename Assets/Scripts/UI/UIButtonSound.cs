using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    public AudioSource audioSource; // Sound source
    public AudioClip buttonSound; // Sound to play on click

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound();
    }

    private void PlaySound()
    {
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
    }
}
