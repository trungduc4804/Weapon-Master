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
        
        if (itemData.addToQuickItemBar)
        {
            QuickItemBar quickItemBar = FindFirstObjectByType<QuickItemBar>();
            if (quickItemBar != null)
            {
                quickItemBar.TryAddItem(itemData);
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
