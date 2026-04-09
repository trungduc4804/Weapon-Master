using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemEntryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text buyButtonText;
    [SerializeField] private TMP_Text remainText;

    private ShopItemData itemData;
    private ShopManager shopManager;

    public ShopItemData ItemData => itemData;

    public void Setup(ShopItemData data, ShopManager manager)
    {
        itemData = data;
        shopManager = manager;

        if (iconImage != null)
        {
            iconImage.sprite = data.itemIcon;
            iconImage.enabled = data.itemIcon != null;
        }

        if (itemNameText != null)
        {
            itemNameText.text = data.itemName;
        }

        if (priceText != null)
        {
            int safePrice = Mathf.Max(0, data.price);
            priceText.text = safePrice.ToString();
        }

        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    public void RefreshState(bool canAfford, bool canPurchase, int remainPurchaseCount)
    {
        if (buyButton != null)
        {
            buyButton.interactable = canAfford && canPurchase;
        }

        if (buyButtonText != null)
        {
            if (!canPurchase)
            {
                buyButtonText.text = "Da het";
            }
            else if (!canAfford)
            {
                buyButtonText.text = "Thieu vang";
            }
            else
            {
                buyButtonText.text = priceText.text;
            }
        }

        if (remainText != null)
        {
            if (remainPurchaseCount < 0)
            {
                remainText.text = "Con lai: vo han";
            }
            else
            {
                remainText.text = "Con lai: " + remainPurchaseCount;
            }
        }
    }

    private void OnBuyClicked()
    {
        if (shopManager == null || itemData == null)
        {
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        shopManager.TryBuyItem(itemData);
    }

    private void OnDestroy()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
        }
    }
}
