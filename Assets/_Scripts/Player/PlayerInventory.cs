using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public static event Action OnInventoryChanged;

    [Header("Inventory Settings")]
    public int maxSlots = 12; // Giới hạn đúng 12 món đồ trên lưới UI

    [Header("Weapons")]
    public List<WeaponBase> weapons = new List<WeaponBase>();

    [Header("Items")]
    public List<ShopItemData> consumeItems = new List<ShopItemData>();

    public int GetEmptySlotsCount()
    {
        return maxSlots - (weapons.Count + consumeItems.Count);
    }

    public bool HasWeapon(ShopItemData weaponData)
    {
        if (weaponData == null || weaponData.effectType != ShopItemEffectType.WeaponUnlock) return false;

        // Check bag
        foreach (var w in weapons)
        {
            if (w != null && w.originData == weaponData) return true; // Đã có trong túi
        }

        // Check equipped
        PlayerAttack attack = GetComponent<PlayerAttack>();
        if (attack != null)
        {
            if (attack.weaponSlot1 != null && attack.weaponSlot1.originData == weaponData) return true;
            if (attack.weaponSlot2 != null && attack.weaponSlot2.originData == weaponData) return true;
        }

        return false;
    }

    // Nhận Loot từ Gacha hoặc Cửa hàng
    public void ReceiveLoot(ShopItemData itemData)
    {
        if (itemData == null) return;
        if (GetEmptySlotsCount() <= 0)
        {
            Debug.Log("Túi đồ đã đầy!");
            return;
        }

        if (itemData.effectType == ShopItemEffectType.WeaponUnlock)
        {
            if (itemData.weaponPrefab != null)
            {
                WeaponBase newWeapon = UnityEngine.Object.Instantiate(itemData.weaponPrefab);
                newWeapon.originData = itemData;
                AddWeapon(newWeapon);
            }
        }
        else
        {
            // Các đồ dùng tiêu hao (Buff, Heal...)
            consumeItems.Add(itemData);
            OnInventoryChanged?.Invoke();
        }
    }

    public bool AddWeapon(WeaponBase weapon)
    {
        if (weapon == null) return false;
        
        if (weapons.Count >= maxSlots)
        {
            Debug.Log("Không thể nhặt thêm, giới hạn túi đồ đã đạt mốc " + maxSlots);
            return false; // Trả về False để Gacha/Loot biết túi đầy
        }

        // Ensure weapon is childed to this inventory object and disabled
        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.gameObject.SetActive(false);
        
        if (!weapons.Contains(weapon))
        {
            weapons.Add(weapon);
            OnInventoryChanged?.Invoke();
        }
        return true;
    }

    public void RemoveWeapon(WeaponBase weapon)
    {
        if (weapons.Contains(weapon))
        {
            weapons.Remove(weapon);
            OnInventoryChanged?.Invoke();
        }
    }

    public void AddConsumeItem(ShopItemData item)
    {
        if (item == null) return;
        consumeItems.Add(item);
        OnInventoryChanged?.Invoke();
    }
}
