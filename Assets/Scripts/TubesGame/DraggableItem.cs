using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image image;

    [Header("Audio")]
    [SerializeField] private AudioSource dragSoundSource;

    private bool canDrag = false;
    private bool isCurrentlyDragging = false; // Flag to track if dragging actually started
    private Transform parentAfterDrag;

    public int TubeID { get; set; }
    public Image Image => image;

    public Transform ParentAfterDrag
    {
        get => parentAfterDrag;
        set => parentAfterDrag = value;
    }

    private void Awake()
    {
        DisableDragging(); // Ensure it starts disabled

        if (dragSoundSource == null)
        {
            dragSoundSource = FindFirstObjectByType<AudioSource>();
            if (dragSoundSource == null)
            {
                Debug.LogError("DraggableItem: No AudioSource found in the scene!");
            }
        }
    }

    public void EnableDragging()
    {
        canDrag = true;
    }

    public void DisableDragging()
    {
        canDrag = false;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || (GameManager.Instance != null && !GameManager.Instance.IsInteractionAllowed))
        {
            Debug.Log($"{gameObject.name}: Dragging is not allowed.");
            isCurrentlyDragging = false;
            return;
        }

        Debug.Log($"{gameObject.name}: Begin drag");
        isCurrentlyDragging = true;

        ParentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        // **Play Global Drag Sound**
        if (GlobalSoundManager.Instance != null)
        {
            GlobalSoundManager.Instance.PlayDragSound();
            Debug.Log("Global drag sound started.");
        }
    }



    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isCurrentlyDragging) return; // Prevent movement if dragging never started
        transform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isCurrentlyDragging) return; // Do nothing if drag never started

        Debug.Log("End drag");
        isCurrentlyDragging = false; // Reset flag when drag ends
        transform.SetParent(ParentAfterDrag);
        image.raycastTarget = true;
    }
}
