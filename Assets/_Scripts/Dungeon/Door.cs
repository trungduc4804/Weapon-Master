using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject doorBlock;
    [SerializeField] private bool useSpatialAudio = true;

    private Collider2D doorCollider;
    private bool desiredClosedState;
    private bool isLocked;
    private bool? lastClosedState;

    private void Awake()
    {
        if (doorBlock != null)
        {
            doorBlock.SetActive(true);
            doorCollider = doorBlock.GetComponent<Collider2D>();
        }
    }

    public void SetClosed(bool closed, bool playSound = true)
    {
        desiredClosedState = closed;
        ApplyState(playSound);
    }

    public void SetLocked(bool locked, bool playSound = true)
    {
        isLocked = locked;
        ApplyState(playSound);
    }

    private void ApplyState(bool playSound = true)
    {
        bool closed = desiredClosedState || isLocked;

        if (lastClosedState.HasValue && lastClosedState.Value == closed)
        {
            return;
        }

        if (doorBlock != null)
        {
            doorBlock.SetActive(closed);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = closed;
        }

        lastClosedState = closed;

        if (!playSound || AudioManager.Instance == null || AudioManager.Instance.CueLibrary == null)
        {
            return;
        }

        AudioCue cue = closed
            ? AudioManager.Instance.CueLibrary.DoorClose
            : AudioManager.Instance.CueLibrary.DoorOpen;

        if (useSpatialAudio)
        {
            AudioManager.Instance.PlaySFXAtPoint(cue, transform.position);
            return;
        }

        AudioManager.Instance.PlaySFX(cue);
    }
}
