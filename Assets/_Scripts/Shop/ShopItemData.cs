using UnityEngine;

public enum ShopItemEffectType
{
    Heal,
    DamageBoost,
    MoveSpeedBoost,
    Gold
}

[CreateAssetMenu(fileName = "ShopItem", menuName = "WeaponMaster/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Display")]
    public string itemName = "New Item";
    public Sprite itemIcon;
    public int price = 10;
    [Min(0f)] public float cooldown = 3f;

    [Header("Effect")]
    public ShopItemEffectType effectType = ShopItemEffectType.Heal;
    public float effectValue = 10f;

    [Header("Purchase Limit")]
    [Tooltip("-1 means unlimited purchases.")]
    public int maxPurchaseCount = -1;
}
