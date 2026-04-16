using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Player player;
    [SerializeField] private QuickItemBar quickItemBar;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private Transform listRoot;
    [SerializeField] private ShopItemEntryUI itemEntryPrefab;
    [SerializeField] private TMP_Text totalGoldText;
    [SerializeField] private TMP_Text messageText;

    [Header("Shop Data")]
    [SerializeField] private List<ShopItemData> items = new List<ShopItemData>();

    private readonly Dictionary<ShopItemData, int> purchasedCounts = new Dictionary<ShopItemData, int>();
    private readonly List<ShopItemEntryUI> activeEntries = new List<ShopItemEntryUI>();

    private int lastGold = int.MinValue;
    private bool hasAppliedInitialShopState;

    private void Awake()
    {
        ResolveDependencies();
    }

    private void Start()
    {
        BuildItemList();
        //SetShopOpen(false);
        RefreshUI(force: true);
    }

    private void Update()
    {
        if (player == null && autoFindPlayer)
        {
            ResolveDependencies();
        }

        if (shopPanel != null && shopPanel.activeSelf)
        {
            RefreshUI(force: false);
        }
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        gameplayUI.SetActive(false);
        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        gameplayUI.SetActive(true);
        Time.timeScale = 1f;
    }
    // public void OpenShop()
    // {
    //     
    //     SetShopOpen(true);
    //     
    //     Time.timeScale = 0f;
    //     
    // }

    // public void CloseShop()
    // {
    //     
    //     SetShopOpen(false);
    //     Time.timeScale = 1f;
    //     
    // }

    // public void ToggleShop()
    // {
    //    
    //     if (shopPanel == null)
    //     {
    //         Debug.LogError("ToggleShop: shopPanel is NULL!");
    //         return;
    //     }

    //     Debug.Log($"shopPanel.activeSelf = {shopPanel.activeSelf}");

    //     if (shopPanel.activeSelf)
    //     {
    //         Debug.Log("Shop is active, calling CloseShop()");
    //         CloseShop();
    //     }
    //     else
    //     {
    //         Debug.Log("Shop is inactive, calling OpenShop()");
    //         OpenShop();
    //     }
    // }

    public bool TryBuyItem(ShopItemData itemData)
    {
        if (itemData == null || player == null)
        {
            return false;
        }

        if (itemData.addToQuickItemBar && quickItemBar == null)
        {
            ShowMessage("Chua gan QuickItemBar.");
            return false;
        }

        int price = Mathf.Max(0, itemData.price);
        int purchasedCount = GetPurchasedCount(itemData);

        if (!CanPurchase(itemData, purchasedCount))
        {
            ShowMessage("Vat pham nay da dat gioi han mua.");
            RefreshUI(force: true);
            return false;
        }

        if (player.gold < price)
        {
            ShowMessage("Khong du vang.");
            RefreshUI(force: true);
            return false;
        }

        if (itemData.addToQuickItemBar && !quickItemBar.TryAddItem(itemData))
        {
            ShowMessage("Thanh item da day.");
            RefreshUI(force: true);
            return false;
        }

        player.gold -= price;
        purchasedCounts[itemData] = purchasedCount + 1;

        if (itemData.grantsBossKey)
        {
            player.AddBossKey(itemData.bossKeyAmount);
        }

        ShowMessage("Mua thanh cong: " + itemData.itemName);
        RefreshUI(force: true);
        return true;
    }

    // private void SetShopOpen(bool isOpen)
    // {
    //     if (shopPanel == null)
    //     {
    //         Debug.LogError("SetShopOpen: shopPanel is NULL!");
    //         return;
    //     }

    //     Debug.Log($"SetShopOpen({isOpen}) - shopPanel name: {shopPanel.name}");
    //     shopPanel.SetActive(isOpen);
    //     Debug.Log($"After SetActive({isOpen}): shopPanel.activeSelf = {shopPanel.activeSelf}");

    //     if (hasAppliedInitialShopState && AudioManager.Instance != null)
    //     {
    //         if (isOpen)
    //         {
    //             Debug.Log("Playing MenuOpen sound");
    //             AudioManager.Instance.PlayMenuOpen();
    //         }
    //         else
    //         {
    //             Debug.Log("Playing MenuClose sound");
    //             AudioManager.Instance.PlayMenuClose();
    //         }
    //     }

    //     if (isOpen)
    //     {
    //         ShowMessage(string.Empty);
    //         RefreshUI(force: true);
    //     }

    //     hasAppliedInitialShopState = true;
    // }

    private void ResolveDependencies()
    {
        if (player == null && autoFindPlayer)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (quickItemBar == null && autoFindPlayer)
        {
            quickItemBar = FindFirstObjectByType<QuickItemBar>();
        }
    }

    private void BuildItemList()
    {
        ClearEntries();

        if (itemEntryPrefab == null || listRoot == null)
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            ShopItemData item = items[i];
            if (item == null)
            {
                continue;
            }

            ShopItemEntryUI entry = Instantiate(itemEntryPrefab, listRoot);
            entry.Setup(item, this);
            activeEntries.Add(entry);
        }
    }

    private void ClearEntries()
    {
        for (int i = 0; i < activeEntries.Count; i++)
        {
            if (activeEntries[i] != null)
            {
                Destroy(activeEntries[i].gameObject);
            }
        }

        activeEntries.Clear();
    }

    private void RefreshUI(bool force)
    {
        if (player == null)
        {
            if (totalGoldText != null)
            {
                totalGoldText.text = "0";
            }
            return;
        }

        if (force || player.gold != lastGold)
        {
            if (totalGoldText != null)
            {
                totalGoldText.text = player.gold.ToString();
            }

            lastGold = player.gold;
        }

        for (int i = 0; i < activeEntries.Count; i++)
        {
            ShopItemEntryUI entry = activeEntries[i];
            if (entry == null)
            {
                continue;
            }

            ShopItemData item = entry.ItemData;
            if (item == null)
            {
                continue;
            }

            int purchasedCount = GetPurchasedCount(item);
            int remainCount = GetRemainCount(item, purchasedCount);
            bool canPurchase = CanPurchase(item, purchasedCount);
            bool canAfford = player.gold >= Mathf.Max(0, item.price);

            entry.RefreshState(canAfford, canPurchase, remainCount);
        }
    }

    private int GetPurchasedCount(ShopItemData itemData)
    {
        if (purchasedCounts.TryGetValue(itemData, out int count))
        {
            return count;
        }

        return 0;
    }

    private static bool CanPurchase(ShopItemData itemData, int purchasedCount)
    {
        if (itemData.maxPurchaseCount < 0)
        {
            return true;
        }

        return purchasedCount < itemData.maxPurchaseCount;
    }

    private static int GetRemainCount(ShopItemData itemData, int purchasedCount)
    {
        if (itemData.maxPurchaseCount < 0)
        {
            return -1;
        }

        return Mathf.Max(0, itemData.maxPurchaseCount - purchasedCount);
    }

    private void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
    }
}
