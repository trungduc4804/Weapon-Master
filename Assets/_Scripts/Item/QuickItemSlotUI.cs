using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image selectionHighlight;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private TMP_Text hotkeyText;
    [SerializeField] private TMP_Text cooldownText;

    [Header("State Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] private Color selectedColor = new Color(1f, 0.8f, 0.35f, 1f);

    private QuickItemBar owner;
    private int slotIndex;

    public void Bind(QuickItemBar itemBar, int index)
    {
        owner = itemBar;
        slotIndex = index;

        if (hotkeyText != null)
        {
            hotkeyText.text = (index + 1).ToString();
        }
    }

    public void Refresh(ShopItemData itemData, int quantity, bool isSelected, float cooldownRemaining)
    {
        bool hasItem = itemData != null && quantity > 0;

        if (iconImage != null)
        {
            iconImage.sprite = hasItem ? itemData.itemIcon : null;
            iconImage.enabled = hasItem && itemData.itemIcon != null;
            iconImage.color = hasItem ? normalColor : emptyColor;
        }

        if (quantityText != null)
        {
            quantityText.text = hasItem ? quantity.ToString() : string.Empty;
        }

        if (selectionHighlight != null)
        {
            selectionHighlight.enabled = isSelected;
            selectionHighlight.color = isSelected ? selectedColor : normalColor;
        }

        if (cooldownOverlay != null)
        {
            bool isCoolingDown = hasItem && cooldownRemaining > 0f;
            cooldownOverlay.enabled = isCoolingDown;

            if (isCoolingDown && itemData.cooldown > 0f)
            {
                cooldownOverlay.fillAmount = Mathf.Clamp01(cooldownRemaining / itemData.cooldown);
            }
            else
            {
                cooldownOverlay.fillAmount = 0f;
            }
        }

        if (cooldownText != null)
        {
            cooldownText.text = cooldownRemaining > 0f ? Mathf.CeilToInt(cooldownRemaining).ToString() : string.Empty;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        owner?.SelectSlot(slotIndex);
    }
}
