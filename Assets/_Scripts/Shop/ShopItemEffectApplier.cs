using UnityEngine;

public static class ShopItemEffectApplier
{
    public static void Apply(ShopItemData itemData, Player player, PlayerAttack playerAttack)
    {
        if (itemData == null || player == null)
        {
            return;
        }

        switch (itemData.effectType)
        {
            case ShopItemEffectType.Heal:
                player.health += itemData.effectValue;
                break;
            case ShopItemEffectType.DamageBoost:
                if (playerAttack != null)
                {
                    playerAttack.AddDamage(itemData.effectValue);
                }
                break;
            case ShopItemEffectType.MoveSpeedBoost:
                player.speedPlayer += itemData.effectValue;
                break;
            case ShopItemEffectType.Gold:
                player.gold += Mathf.RoundToInt(itemData.effectValue);
                break;
        }
    }
}
