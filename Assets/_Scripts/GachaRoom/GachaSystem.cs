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
        GachaEvents.OnPuzzleSolved += GiveGachaRoll;
    }

    private void OnDisable()
    {
        GachaEvents.OnPuzzleSolved -= GiveGachaRoll;
    }

    private void GiveGachaRoll()
    {
        if (player == null) return;
        player.gachaRolls++;
        GachaEvents.OnGachaRollsCountChanged?.Invoke(player.gachaRolls);
    }

    public void RollGacha()
    {
        if (player == null) return;

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

            if (item != null)
            {
                // Here we actually give the item to the player.
                // Assuming it works exactly like ShopItemData
                GiveItemToPlayer(item);
                
                // Fire Event for UI/Audio
                GachaEvents.OnGachaRolled?.Invoke(item);
                
                // Audio
                if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
                {
                    // Placeholder for Gacha Roll Sound
                    AudioManager.Instance.PlayUI(AudioManager.Instance.CueLibrary.ButtonClick); 
                }
            }
        }
        else
        {
            // No rolls left
            GachaEvents.OnRollFailed_NoRolls?.Invoke();
        }
    }

    private void GiveItemToPlayer(ShopItemData itemData)
    {
        // Re-use logic from ShopManager or QuickItemBar
        if (itemData.grantsBossKey)
        {
            player.AddBossKey(itemData.bossKeyAmount);
        }
        
        if (itemData.addToQuickItemBar)
        {
            QuickItemBar quickItemBar = FindFirstObjectByType<QuickItemBar>();
            if (quickItemBar != null)
            {
                quickItemBar.TryAddItem(itemData);
            }
        }
    }
}
