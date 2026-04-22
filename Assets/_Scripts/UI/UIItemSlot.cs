using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItemSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Hiển Thị")]
    public Image iconImage;

    [Header("Loại Ô Này")]
    public bool isEquipSlot = false; // Bật cờ này lên nếu ô này nằm ở dòng "Vũ Khí Trang Bị"
    public int equipIndex = 1; // 1 (Q) hoặc 2 (E)

    // Dữ liệu nội tại
    [HideInInspector] public WeaponBase holdWeapon;
    [HideInInspector] public ShopItemData holdItem; // Hỗ trợ chứa vật phẩm tiêu hao (Máu, Buff)
    
    private ISlotManager slotManager;

    private void Start()
    {
        // Ẩn ban đầu
        if (iconImage != null)
        {
            iconImage.enabled = false;
        }
        slotManager = GetComponentInParent<ISlotManager>();
    }

    public void SetupWeapon(WeaponBase weapon)
    {
        holdWeapon = weapon;
        holdItem = null; // Đảm bảo làm rỗng rác Item
        
        if (holdWeapon != null && holdWeapon.originData != null)
        {
            iconImage.sprite = holdWeapon.originData.itemIcon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void SetupItem(ShopItemData item)
    {
        holdItem = item;
        holdWeapon = null; // Đảm bảo làm rỗng rác vũ khí
        
        if (holdItem != null)
        {
            iconImage.sprite = holdItem.itemIcon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Xét click chuột Trái
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Kiểm tra mức độ click đúp
            if (eventData.clickCount == 2)
            {
                if (slotManager != null)
                {
                    slotManager.HandleSlotDoubleClick(this);
                }
            }
        }
    }
}
