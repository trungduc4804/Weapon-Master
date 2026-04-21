using UnityEngine;

public class InventoryChestUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory;
    public ChestSystem chestSystem;
    public PlayerAttack playerAttack;

    // Hàm hỗ trợ tự động tìm Rương nếu nó được sinh ra ở runtime (Dungeon Generator)
    public ChestSystem GetChestSystem()
    {
        if (chestSystem == null)
        {
            chestSystem = Object.FindFirstObjectByType<ChestSystem>();
        }
        return chestSystem;
    }

    [Header("UI Panels")]
    public GameObject chestUIPanel;
    public GameObject inventoryUIPanel;

    public void OpenChestUI()
    {
        if (chestUIPanel != null) chestUIPanel.SetActive(true);
        if (inventoryUIPanel != null) inventoryUIPanel.SetActive(true);
        Time.timeScale = 0f; // Tạm dừng game
    }

    public void CloseUI()
    {
        if (chestUIPanel != null) chestUIPanel.SetActive(false);
        if (inventoryUIPanel != null) inventoryUIPanel.SetActive(false);
        Time.timeScale = 1f; // Tiếp tục game
    }

    // --- CÁC HÀM GIAO TIẾP VỚI UI (GÁN VÀO BUTTON ONCLICK) ---

    // Gọi hàm này khi click button "Trang bị Slot 1" trên giao diện của 1 vũ khí
    public void EquipWeaponToSlot1(WeaponBase weapon)
    {
        if (playerAttack != null && playerInventory.weapons.Contains(weapon))
        {
            playerAttack.EquipWeaponToSlot(weapon, 1);
        }
    }

    // Gọi hàm này khi click button "Trang bị Slot 2" trên giao diện của 1 vũ khí
    public void EquipWeaponToSlot2(WeaponBase weapon)
    {
        if (playerAttack != null && playerInventory.weapons.Contains(weapon))
        {
            playerAttack.EquipWeaponToSlot(weapon, 2);
        }
    }

    // Gọi hàm này khi click button "Chuyển vào Rương"
    public void MoveWeaponToChest(WeaponBase weapon)
    {
        ChestSystem currentChest = GetChestSystem();
        
        if (currentChest != null && playerInventory.weapons.Contains(weapon))
        {
            playerInventory.RemoveWeapon(weapon);
            currentChest.AddWeapon(weapon);
            
            // Xóa khỏi slot nếu đang trang bị
            if (playerAttack.weaponSlot1 == weapon) playerAttack.weaponSlot1 = null;
            if (playerAttack.weaponSlot2 == weapon) playerAttack.weaponSlot2 = null;
        }
    }

    // Gọi hàm này khi click button "Lấy về Túi"
    public void MoveWeaponToInventory(WeaponBase weapon)
    {
        ChestSystem currentChest = GetChestSystem();
        
        if (currentChest != null && currentChest.storedWeapons.Contains(weapon))
        {
            currentChest.RemoveWeapon(weapon);
            playerInventory.AddWeapon(weapon);
        }
    }
}
