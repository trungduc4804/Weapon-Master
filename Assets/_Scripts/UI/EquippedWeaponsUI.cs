using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedWeaponsUI : MonoBehaviour
{
    [Header("UI Slot 1 (Phím Q)")]
    public Image slot1Icon;
    public Image slot1Highlight; // Khung phát sáng khi đang cầm

    [Header("UI Slot 2 (Phím E)")]
    public Image slot2Icon;
    public Image slot2Highlight; // Khung phát sáng khi đang cầm

    private void OnEnable()
    {
        PlayerAttack.OnWeaponSwitched += HandleWeaponSwitched;
        PlayerAttack.OnWeaponEquippedToSlot += HandleWeaponEquipped;
    }

    private void OnDisable()
    {
        PlayerAttack.OnWeaponSwitched -= HandleWeaponSwitched;
        PlayerAttack.OnWeaponEquippedToSlot -= HandleWeaponEquipped;
    }

    private void HandleWeaponEquipped(int slotIndex, WeaponBase weapon)
    {
        // Hiển thị Icon vũ khí khi vừa được gắn vào
        Sprite weaponSprite = null;
        if (weapon != null && weapon.originData != null)
        {
            weaponSprite = weapon.originData.itemIcon;
        }

        if (slotIndex == 1 && slot1Icon != null)
        {
            slot1Icon.sprite = weaponSprite;
            slot1Icon.enabled = (weaponSprite != null);
        }
        else if (slotIndex == 2 && slot2Icon != null)
        {
            slot2Icon.sprite = weaponSprite;
            slot2Icon.enabled = (weaponSprite != null);
        }
    }

    private void HandleWeaponSwitched(int activeSlotIndex)
    {
        // Thay vì tắt component (làm mất luôn khung nền), ta đổi màu/độ mờ để tạo hiệu ứng highlight
        if (slot1Highlight != null)
        {
            Color c = slot1Highlight.color;
            c.a = (activeSlotIndex == 1) ? 1f : 0.4f;
            slot1Highlight.color = c;
        }
        
        if (slot2Highlight != null)
        {
            Color c = slot2Highlight.color;
            c.a = (activeSlotIndex == 2) ? 1f : 0.4f;
            slot2Highlight.color = c;
        }
    }
}
