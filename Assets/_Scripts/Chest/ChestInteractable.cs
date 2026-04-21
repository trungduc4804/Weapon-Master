using UnityEngine;

public class ChestInteractable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Khoảng cách tối đa để người chơi tương tác với rương.")]
    public float interactDistance = 2f;

    private ChestUIManager inventoryUI;
    private Transform playerTransform;

    private void Start()
    {
        inventoryUI = transform.parent?.GetComponentInChildren<ChestUIManager>(true);
        if (inventoryUI == null) inventoryUI = FindFirstObjectByType<ChestUIManager>(FindObjectsInactive.Include);
        
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
            inventoryUI = transform.parent?.GetComponentInChildren<ChestUIManager>(true);
            if (inventoryUI == null) inventoryUI = FindFirstObjectByType<ChestUIManager>(FindObjectsInactive.Include);
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

        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OpenChest();
    }

    private void OnMouseDown()
    {
        OpenChestInterface();
    }
}
