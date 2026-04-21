using UnityEngine;

public class ChestInteractable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Khoảng cách tối đa để người chơi tương tác với rương.")]
    public float interactDistance = 2f;

    private InventoryChestUI inventoryUI;
    private Transform playerTransform;

    private void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryChestUI>();
        
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void OpenChestInterface()
    {
        if (inventoryUI == null)
        {
            inventoryUI = FindFirstObjectByType<InventoryChestUI>();
        }

        if (inventoryUI == null) return;

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > interactDistance)
            {
                Debug.Log("Bạn đang đứng quá xa rương!");
                return;
            }
        }

        inventoryUI.OpenChestUI();
    }

    private void OnMouseDown()
    {
        OpenChestInterface();
    }
}
