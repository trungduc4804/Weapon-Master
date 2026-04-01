using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public Room room;
    Room targetRoom;

    void Awake()
    {
        // Always prefer the room on parent root to avoid wrong references
        // when prefabs accidentally contain duplicate Room components.
        targetRoom = GetComponentInParent<Room>();

        if (targetRoom == null)
        {
            targetRoom = room;
        }
        else if (room != null && room != targetRoom)
        {
            Debug.LogWarning(
                $"{name}: RoomTrigger.room is set to '{room.name}' but parent room is '{targetRoom.name}'. Using parent room."
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetRoom == null) return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null) return;

        targetRoom.PlayerEntered();
        player.SetCurrentRoom(targetRoom);
    }
}
