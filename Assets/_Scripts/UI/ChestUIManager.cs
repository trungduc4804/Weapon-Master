using UnityEngine;

public class ChestUIManager : MonoBehaviour, ISlotManager
{
    [Header("UI GameObject")]
    public GameObject chestPanel;
    public GameObject chestButton;
    [Header("Tham Chiếu System")]
    public ChestSystem chestSystem;
    public PlayerInventory playerInventory;

    [Header("Lưới Giao Diện")]
    // Kéo 18 ô trống của Rương vào đây
    public UIItemSlot[] chestSlots; 
    // Kéo 12 ô trống của Túi đồ vào đây
    public UIItemSlot[] bagSlots;

    private void Start()
    {
        if (chestPanel != null)
            chestPanel.SetActive(false);

        // Khởi tạo thẻ nhận dạng: Bật cờ "isEquipSlot = true" (ý nghĩa đổi thành isChestSlot) cho các ô rương trên để phân biệt
        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (chestSlots[i] != null) chestSlots[i].isEquipSlot = true;
        }
        for (int i = 0; i < bagSlots.Length; i++)
        {
            if (bagSlots[i] != null) bagSlots[i].isEquipSlot = false;
        }

        FindSystems();
    }

    private void FindSystems()
    {
        if (playerInventory == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerInventory = playerObj.GetComponent<PlayerInventory>();
        }

        if (chestSystem == null)
        {
            // Tìm anh em cùng cha (Chest) nằm chung thư mục với ChestUI (transform.parent)
            chestSystem = transform.parent?.GetComponentInChildren<ChestSystem>();
            if (chestSystem == null)
                chestSystem = FindFirstObjectByType<ChestSystem>(); // Dự phòng
        }
    }

    // Khi được gọi bởi ChestInteractable -> Mở rương lên
    public void OpenChest()
    {
        FindSystems();
        if (chestPanel == null) return;
        chestButton.SetActive(false);
        chestPanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game tránh quái đánh khi cất đồ
        RefreshUI();
    }

    public void CloseChest()
    {
        if (chestPanel == null) return;
        chestButton.SetActive(true);
        chestPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RefreshUI()
    {
        if (chestSystem == null || playerInventory == null) return;

        // --- 1. VẼ PHẦN RƯƠNG TRÊN CÙNG ---
        int chestWeaponCount = chestSystem.storedWeapons.Count;
        int chestItemCount = chestSystem.storedItems.Count;

        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (chestSlots[i] != null)
            {
                if (i < chestWeaponCount)
                    chestSlots[i].SetupWeapon(chestSystem.storedWeapons[i]);
                else if (i < chestWeaponCount + chestItemCount)
                    chestSlots[i].SetupItem(chestSystem.storedItems[i - chestWeaponCount]);
                else 
                    chestSlots[i].SetupWeapon(null); // Trống
            }
        }

        // --- 2. VẼ PHẦN TÚI ĐỒ (BAG) Ở DƯỚI ---
        int bagWeaponCount = playerInventory.weapons.Count;
        int bagItemCount = playerInventory.consumeItems.Count;

        for (int i = 0; i < bagSlots.Length; i++)
        {
            if (bagSlots[i] != null)
            {
                if (i < bagWeaponCount)
                    bagSlots[i].SetupWeapon(playerInventory.weapons[i]);
                else if (i < bagWeaponCount + bagItemCount)
                    bagSlots[i].SetupItem(playerInventory.consumeItems[i - bagWeaponCount]);
                else
                    bagSlots[i].SetupWeapon(null);
            }
        }
    }

    public void HandleSlotDoubleClick(UIItemSlot clickedSlot)
    {
        if (chestSystem == null || playerInventory == null) return;

        // Nếu Click ở RƯƠNG (Nửa Trên) -> Ném xuống Túi
        if (clickedSlot.isEquipSlot) 
        {
            // Kiểm tra túi trống ko
            if (playerInventory.GetEmptySlotsCount() <= 0)
            {
                Debug.Log("Túi đồ đầy! Không thể lấy từ rương xuống.");
                return;
            }

            if (clickedSlot.holdWeapon != null)
            {
                chestSystem.RemoveWeapon(clickedSlot.holdWeapon);
                playerInventory.AddWeapon(clickedSlot.holdWeapon);
            }
            else if (clickedSlot.holdItem != null)
            {
                chestSystem.RemoveItem(clickedSlot.holdItem);
                playerInventory.consumeItems.Add(clickedSlot.holdItem);
            }
        }
        else // Nếu Click ở TÚI (Nửa Dưới) -> Đẩy lên Rương
        {
            if (clickedSlot.holdWeapon != null)
            {
                playerInventory.RemoveWeapon(clickedSlot.holdWeapon);
                chestSystem.AddWeapon(clickedSlot.holdWeapon);
            }
            else if (clickedSlot.holdItem != null)
            {
                playerInventory.consumeItems.Remove(clickedSlot.holdItem);
                chestSystem.AddItem(clickedSlot.holdItem);
            }
        }

        RefreshUI();
    }
}
