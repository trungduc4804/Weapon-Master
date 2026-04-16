using UnityEngine;

public class ShopInteractable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Khoảng cách tối đa để người chơi có thể nhấn vào cửa hàng.")]
    public float interactDistance = 5f;

    private ShopManager shopManager;
    private Transform playerTransform;

    private void Start()
    {
        // Tìm ShopManager có sẵn trong ván chơi hiện tại
        shopManager = FindFirstObjectByType<ShopManager>();
        
        // Tìm Player để tính toán khoảng cách
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    /// <summary>
    /// Hàm này được dùng để mở Shop Panel.
    /// Có thể gán hàm này vào sự kiện OnClick của UI Button.
    /// </summary>
    public void OpenShopPanel()
    {
        if (shopManager == null)
        {
            Debug.LogWarning("ShopManager không tồn tại trong Scene.");
            return;
        }

        // Kiểm tra xem người chơi có đứng quá xa cửa hàng không
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > interactDistance)
            {
                Debug.Log("Bạn đang ở quá xa cửa hàng!");
                // (Tùy chọn) Bạn có thể cho hiển thị 1 dòng thông báo UI ở đây nếu muốn
                return;
            }
        }

        // Mở Panel Shop
        shopManager.OpenShop();
    }

    /// <summary>
    /// Hàm có sẵn của Unity. 
    /// Khi gắn script này vào object có Collider2D, người chơi click chuột vào object thì hàm này sẽ tự chạy.
    /// </summary>
    private void OnMouseDown()
    {
        OpenShopPanel();
    }
}
