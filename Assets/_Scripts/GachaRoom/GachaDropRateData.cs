using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaItemRate
{
    public ShopItemData itemData;
    [Range(0f, 100f)]
    public float dropWeight;
}

[CreateAssetMenu(fileName = "NewGachaDropRateData", menuName = "WeaponMaster/Gacha Drop Rate Data")]
public class GachaDropRateData : ScriptableObject
{
    [Header("Drop Pool")]
    public List<GachaItemRate> items = new List<GachaItemRate>();

    public ShopItemData GetRandomItem()
    {
        if (items == null || items.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var drop in items)
        {
            totalWeight += drop.dropWeight;
        }

        float randomVal = UnityEngine.Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (var drop in items)
        {
            currentSum += drop.dropWeight;
            if (randomVal <= currentSum)
            {
                return drop.itemData;
            }
        }

        return items[0].itemData; // Fallback
    }
}
