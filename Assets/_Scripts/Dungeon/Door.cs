using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorBlock;
    private Collider2D doorCollider;

    void Awake()
    {
        if (doorBlock != null)
        {
            // Ensure doorBlock is active so we can control it
            doorBlock.SetActive(true);
            doorCollider = doorBlock.GetComponent<Collider2D>();
        }
    }

    public void SetClosed(bool closed)
    {
        if (doorBlock != null)
        {
            doorBlock.SetActive(closed);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = closed;
        }
    }
}