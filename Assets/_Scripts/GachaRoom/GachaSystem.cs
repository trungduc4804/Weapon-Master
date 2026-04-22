using UnityEngine;

public class GachaSystem : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Player player;
    [SerializeField] private GachaDropRateData dropRateData;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public ShopItemData PreCalculateRoll()
    {
        if (player == null) return null;

        if (player.gachaRolls > 0)
        {
            // Deduct roll
            player.gachaRolls--;
            GachaEvents.OnGachaRollsCountChanged?.Invoke(player.gachaRolls);

            // Get Random Item
            ShopItemData item = null;
            if (dropRateData != null)
            {
                item = dropRateData.GetRandomItem();
            }
            return item;
        }
        else
        {
            // No rolls left
            GachaEvents.OnRollFailed_NoRolls?.Invoke();
            return null;
        }
    }

    public void GiveItemToPlayer(ShopItemData itemData)
    {
        if (itemData == null) return;
        
        // Re-use logic from ShopManager or QuickItemBar
        if (itemData.grantsBossKey)
        {
            player.AddBossKey(itemData.bossKeyAmount);
        }

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            if (itemData.effectType == ShopItemEffectType.WeaponUnlock && inventory.HasWeapon(itemData))
            {
                // Trùng vũ khí -> Đổi thành 1 Vàng
                player.gold += 1;
                Debug.Log("Quay trúng vũ khí ĐÃ CÓ trong túi! Đã quy đổi thành 1 Vàng.");
            }
            else
            {
                inventory.ReceiveLoot(itemData);
            }
        }
        
        // Fire Event for UI/Audio when actually received
        GachaEvents.OnGachaRolled?.Invoke(itemData);
        
        // Audio
        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlayUI(AudioManager.Instance.CueLibrary.GachaReward); 
        }
    }
}
