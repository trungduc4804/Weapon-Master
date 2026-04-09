using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickItemBar : MonoBehaviour
{
    [Serializable]
    private class QuickItemSlot
    {
        public ShopItemData itemData;
        public int quantity;
        public float cooldownReadyTime;

        public float GetCooldownRemaining()
        {
            return Mathf.Max(0f, cooldownReadyTime - Time.time);
        }

        public void Clear()
        {
            itemData = null;
            quantity = 0;
            cooldownReadyTime = 0f;
        }
    }

    [Header("Dependencies")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("UI")]
    [SerializeField] private List<QuickItemSlotUI> slotUIs = new List<QuickItemSlotUI>();
    [SerializeField] private TMP_Text messageText;

    [Header("Input")]
    [SerializeField] private KeyCode useItemKey = KeyCode.Space;

    private QuickItemSlot[] slots = Array.Empty<QuickItemSlot>();
    private int selectedSlotIndex;

    private void Awake()
    {
        ResolveDependencies();
        InitializeSlots();
    }

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (autoFindPlayer && (player == null || playerAttack == null))
        {
            ResolveDependencies();
        }

        HandleSelectionInput();
        HandleUseInput();
        RefreshUI();
    }

    public bool TryAddItem(ShopItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0)
        {
            return false;
        }

        int stackIndex = FindSlotWithItem(itemData);
        if (stackIndex >= 0)
        {
            slots[stackIndex].quantity += amount;
            if (IsSlotEmpty(selectedSlotIndex))
            {
                selectedSlotIndex = stackIndex;
            }

            RefreshUI();
            return true;
        }

        int emptyIndex = FindEmptySlot();
        if (emptyIndex < 0)
        {
            return false;
        }

        slots[emptyIndex].itemData = itemData;
        slots[emptyIndex].quantity = amount;
        slots[emptyIndex].cooldownReadyTime = 0f;

        if (IsSlotEmpty(selectedSlotIndex))
        {
            selectedSlotIndex = emptyIndex;
        }

        RefreshUI();
        return true;
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length)
        {
            return;
        }

        selectedSlotIndex = index;
        RefreshUI();
    }

    public bool TryUseSelectedItem()
    {
        if (player == null)
        {
            ShowMessage("Chua tim thay Player.");
            return false;
        }

        if (selectedSlotIndex < 0 || selectedSlotIndex >= slots.Length)
        {
            ShowMessage("Chua chon item.");
            return false;
        }

        QuickItemSlot slot = slots[selectedSlotIndex];
        if (slot.itemData == null || slot.quantity <= 0)
        {
            ShowMessage("O item dang rong.");
            return false;
        }

        float cooldownRemaining = slot.GetCooldownRemaining();
        if (cooldownRemaining > 0f)
        {
            ShowMessage("Item dang hoi chieu.");
            return false;
        }

        string itemName = slot.itemData.itemName;
        ShopItemEffectApplier.Apply(slot.itemData, player, playerAttack);
        slot.quantity--;
        slot.cooldownReadyTime = Time.time + Mathf.Max(0f, slot.itemData.cooldown);

        if (slot.quantity <= 0)
        {
            slot.Clear();
        }

        ShowMessage("Da dung: " + itemName);
        RefreshUI();
        return true;
    }

    private void InitializeSlots()
    {
        int slotCount = slotUIs.Count;
        if (slotCount <= 0)
        {
            selectedSlotIndex = -1;
            slots = Array.Empty<QuickItemSlot>();
            return;
        }

        slots = new QuickItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = new QuickItemSlot();
            if (slotUIs[i] != null)
            {
                slotUIs[i].Bind(this, i);
            }
        }

        selectedSlotIndex = 0;
    }

    private void ResolveDependencies()
    {
        if (player == null && autoFindPlayer)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (playerAttack == null && player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        if (playerAttack == null && autoFindPlayer)
        {
            playerAttack = FindFirstObjectByType<PlayerAttack>();
        }
    }

    private void HandleSelectionInput()
    {
        int keyCount = Mathf.Min(slots.Length, 9);
        for (int i = 0; i < keyCount; i++)
        {
            KeyCode hotkey = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(hotkey))
            {
                SelectSlot(i);
                return;
            }
        }
    }

    private void HandleUseInput()
    {
        if (slots.Length == 0)
        {
            return;
        }

        if (Input.GetKeyDown(useItemKey))
        {
            TryUseSelectedItem();
        }
    }

    private void RefreshUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            QuickItemSlotUI slotUI = slotUIs[i];
            if (slotUI == null)
            {
                continue;
            }

            QuickItemSlot slot = i < slots.Length ? slots[i] : null;
            if (slot == null)
            {
                slotUI.Refresh(null, 0, false, 0f);
                continue;
            }

            slotUI.Refresh(slot.itemData, slot.quantity, i == selectedSlotIndex, slot.GetCooldownRemaining());
        }
    }

    private int FindSlotWithItem(ShopItemData itemData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData == itemData)
            {
                return i;
            }
        }

        return -1;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemData == null || slots[i].quantity <= 0)
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsSlotEmpty(int index)
    {
        return index < 0 || index >= slots.Length || slots[index].itemData == null || slots[index].quantity <= 0;
    }

    private void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
    }
}
