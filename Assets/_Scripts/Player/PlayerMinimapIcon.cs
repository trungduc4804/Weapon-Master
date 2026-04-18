using UnityEngine;

public class PlayerMinimapIcon : MonoBehaviour
{
    [Tooltip("Kéo script Player của bạn vào đây")]
    public Player player;

    private void Start()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    private void LateUpdate()
    {
        if (player != null && player.CurrentRoom != null)
        {
            // Ép vị trí của thẻ (icon) này ở mãi ngay vị trí giữ trọn vẹn center của căn phòng.
            // Điều này khiến Icon to oạch này không bị lọt sang ô bên cạnh trên Minimap khi nhân vật di chuyển 
            transform.position = new Vector3(
                player.CurrentRoom.transform.position.x, 
                player.CurrentRoom.transform.position.y, 
                transform.position.z // Giữ nguyên độ cao Z để render không lỗi
            );
        }
    }
}
