using UnityEngine;

public class InventoryTestHelper : MonoBehaviour
{
    [Header("Gắn Player vào 2 ô này")]
    public InventoryChestUI inventoryChestUI;
    public PlayerInventory playerInventory;

    public void Test_TrangBiVuKhiThuNhat_Vao_Slot1()
    {
        if (playerInventory.weapons.Count > 0)
        {
            // Lấy vũ khí đứng cớ vị trí đầu tiên trong túi đồ (số 0)
            WeaponBase vuKhi = playerInventory.weapons[0];
            
            // Trang bị
            inventoryChestUI.EquipWeaponToSlot1(vuKhi);
            Debug.Log("Đã trang bị thành công: " + vuKhi.gameObject.name);
        }
        else
        {
            Debug.LogWarning("Túi đồ của Player đang trống rỗng! Hãy ra quay Gacha để nhận vũ khí trước.");
        }
    }

    public void Test_CatVuKhThuNhat_Vao_Ruong()
    {
        if (playerInventory.weapons.Count > 0)
        {
            WeaponBase vuKhi = playerInventory.weapons[0];
            inventoryChestUI.MoveWeaponToChest(vuKhi);
            Debug.Log("Đã cất vũ khí vào Rương!");
        }
        else
        {
            Debug.LogWarning("Túi đồ đang trống rỗng!");
        }
    }

    public void Test_LayVuKhiThuNhat_Tu_Ruong()
    {
        ChestSystem ruongHienTai = inventoryChestUI.GetChestSystem();

        if (ruongHienTai != null && ruongHienTai.storedWeapons.Count > 0)
        {
            WeaponBase vuKhi = ruongHienTai.storedWeapons[0];
            inventoryChestUI.MoveWeaponToInventory(vuKhi);
            Debug.Log("Đã lấy vũ khí từ Rương về Túi đồ!");
        }
        else
        {
            Debug.LogWarning("Ruong không có đồ hoặc chưa tìm thấy rương!");
        }
    }
}
