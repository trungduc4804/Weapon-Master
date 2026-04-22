using UnityEngine;

public class MainInventoryUI : MonoBehaviour, ISlotManager
{
    [Header("UI GameObject")]
    public GameObject inventoryPanel;

    [Header("Tham Chiếu System")]
    public PlayerAttack playerAttack;
    public PlayerInventory playerInventory;

    [Header("Lưới Giao Diện")]
    // Kéo 2 ô Image trên (Vũ khí trang bị) vào đây. index 0 là Q, 1 là E
    public UIItemSlot[] equipSlots; 
    
    // Kéo 12 ô Image dưới (Túi đồ) vào đây.
    public UIItemSlot[] bagSlots;

    private void Start()
    {
        // Ẩn UI lúc ban đầu
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // Khởi tạo loại hình cho các slot
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] != null)
            {
                equipSlots[i].isEquipSlot = true;
                equipSlots[i].equipIndex = (i == 0) ? 1 : 2; // Q là 1, E là 2
            }
        }
        for (int i = 0; i < bagSlots.Length; i++)
        {
            if (bagSlots[i] != null) bagSlots[i].isEquipSlot = false;
        }

        // Tự động tìm player nếu chưa gắn
        if (playerAttack == null || playerInventory == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                if (playerAttack == null) playerAttack = playerObj.GetComponent<PlayerAttack>();
                if (playerInventory == null) playerInventory = playerObj.GetComponent<PlayerInventory>();
            }
        }
    }

    private void Update()
    {
        // Nhấn nút TAB mở/tắt
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryPanel == null) return;

        bool isOpening = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpening);

        if (isOpening)
        {
            Time.timeScale = 0f; // Tạm dừng Game
            RefreshUI();
        }
        else
        {
            Time.timeScale = 1f; // Tiếp tục Game
        }
    }

    public void CloseUI()
    {
        if (inventoryPanel == null) return;
        inventoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Làm mới diện mạo tất cả các ô
    public void RefreshUI()
    {
        if (playerAttack == null || playerInventory == null) return;

        // Vẽ 2 ô Trang bị
        if (equipSlots.Length >= 2)
        {
            if (equipSlots[0] != null) equipSlots[0].SetupWeapon(playerAttack.weaponSlot1);
            if (equipSlots[1] != null) equipSlots[1].SetupWeapon(playerAttack.weaponSlot2);
        }

        // Vẽ lưới túi đồ (Bag) kết hợp cả Weapon và Item Buff
        int weaponCount = playerInventory.weapons.Count;
        int itemCount = playerInventory.consumeItems.Count;

        for (int i = 0; i < bagSlots.Length; i++)
        {
            if (bagSlots[i] != null)
            {
                if (i < weaponCount)
                {
                    // Lấp ô bằng Vũ Khí
                    bagSlots[i].SetupWeapon(playerInventory.weapons[i]);
                }
                else if (i < weaponCount + itemCount)
                {
                    // Lấp ô bằng Vật Phẩm Tiêu Hao
                    int itemIndex = i - weaponCount;
                    bagSlots[i].SetupItem(playerInventory.consumeItems[itemIndex]);
                }
                else
                {
                    // Ô trống
                    bagSlots[i].SetupWeapon(null);
                }
            }
        }
    }

    // Nhận tín hiệu Click đúp từ các Slot
    public void HandleSlotDoubleClick(UIItemSlot clickedSlot)
    {
        if (playerAttack == null || playerInventory == null) return;

        if (clickedSlot.isEquipSlot)
        {
            // Trục Xuống: Chuyển từ Trang bị -> Túi đồ
            if (clickedSlot.holdWeapon != null)
            {
                if (playerInventory.GetEmptySlotsCount() > 0)
                {
                    // Cất vô túi
                    if (clickedSlot.equipIndex == 1 || clickedSlot.equipIndex == 2)
                    {
                        // Tháo trên người
                        playerAttack.UnequipWeapon(clickedSlot.equipIndex);
                        
                        // Ném vào túi
                        playerInventory.AddWeapon(clickedSlot.holdWeapon);
                    }
                }
                else
                {
                    Debug.Log("Túi đồ đã đầy, không thể tháo vũ khí rớt vào túi!");
                    return; // Mất slot
                }
            }
        }
        else // Là click ở dưới túi bag
        {
            if (clickedSlot.holdItem != null)
            {
                // Sử dụng vật phẩm tiêu hao
                ShopItemEffectApplier.Apply(clickedSlot.holdItem, playerAttack.gameObject.GetComponent<Player>(), playerAttack);
                playerInventory.consumeItems.Remove(clickedSlot.holdItem);
                Debug.Log("Đã sử dụng vật phẩm: " + clickedSlot.holdItem.itemName);
            }
            else if (clickedSlot.holdWeapon != null)
            {
                // Bắn Lên: Chuyển từ Túi đồ -> Trang bị
                WeaponBase weaponToEquip = clickedSlot.holdWeapon;

                // Ưu tiên ném vô ô Q (1) nếu ô Q trống
                if (playerAttack.weaponSlot1 == null)
                {
                    playerInventory.RemoveWeapon(weaponToEquip);
                    playerAttack.EquipWeaponToSlot(weaponToEquip, 1);
                }
                // Nếu Q đầy, ném vô ô E (2) nếu ô E trống
                else if (playerAttack.weaponSlot2 == null)
                {
                    playerInventory.RemoveWeapon(weaponToEquip);
                    playerAttack.EquipWeaponToSlot(weaponToEquip, 2);
                }
                // Nếu 2 ô đều full: Tự động đổi với ô Q (Slot 1)
                else
                {
                    WeaponBase oldWeapon = playerAttack.weaponSlot1;
                    
                    // Lột vũ khí cũ bỏ vào túi
                    playerInventory.AddWeapon(oldWeapon);
                    
                    // Tháo vũ khí mới khỏi túi đắp lên Slot 1
                    playerInventory.RemoveWeapon(weaponToEquip);
                    playerAttack.EquipWeaponToSlot(weaponToEquip, 1);
                }
            }
        }

        // Reset lại giao diện sau khi luân chuyển đồ đạc thành công
        RefreshUI();
    }
}
