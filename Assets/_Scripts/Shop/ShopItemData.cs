using UnityEngine;

public enum ShopItemEffectType
{
    Heal,
    DamageBoost,
    MoveSpeedBoost,
    Gold,
    WeaponUnlock
}

[CreateAssetMenu(fileName = "ShopItem", menuName = "WeaponMaster/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Display")]
    public string itemName = "New Item";
    public Sprite itemIcon;
    public WeaponBase weaponPrefab;
    public int price = 10;
    [Min(0f)] public float cooldown = 3f;
    public bool isConsumable = true;

    [Header("Effect")]
    public ShopItemEffectType effectType = ShopItemEffectType.Heal;
    public float effectValue = 10f;
    public bool grantsBossKey;
    [Min(1)] public int bossKeyAmount = 1;

    [Header("Purchase Limit")]
    [Tooltip("-1 means unlimited purchases.")]
    public int maxPurchaseCount = -1;
}
