using System.Collections.Generic;
using UnityEngine;
using System;

public class ChestSystem : MonoBehaviour
{
    public static event Action OnChestChanged;

    [Header("Chest Storage")]
    public List<WeaponBase> storedWeapons = new List<WeaponBase>();
    public List<ShopItemData> storedItems = new List<ShopItemData>();

    public void AddWeapon(WeaponBase weapon)
    {
        if (weapon == null) return;

        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.gameObject.SetActive(false);

        storedWeapons.Add(weapon);
        OnChestChanged?.Invoke();
    }

    public void RemoveWeapon(WeaponBase weapon)
    {
        if (storedWeapons.Contains(weapon))
        {
            storedWeapons.Remove(weapon);
            OnChestChanged?.Invoke();
        }
    }

    public void AddItem(ShopItemData item)
    {
        if (item == null) return;
        storedItems.Add(item);
        OnChestChanged?.Invoke();
    }

    public void RemoveItem(ShopItemData item)
    {
        if (storedItems.Contains(item))
        {
            storedItems.Remove(item);
            OnChestChanged?.Invoke();
        }
    }
}
