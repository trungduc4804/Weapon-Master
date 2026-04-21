using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public static event Action OnInventoryChanged;

    [Header("Weapons")]
    public List<WeaponBase> weapons = new List<WeaponBase>();

    [Header("Items")]
    public List<ShopItemData> consumeItems = new List<ShopItemData>();

    public void AddWeapon(WeaponBase weapon)
    {
        if (weapon == null) return;
        
        // Ensure weapon is childed to this inventory object and disabled
        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.gameObject.SetActive(false);
        
        weapons.Add(weapon);
        OnInventoryChanged?.Invoke();
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
