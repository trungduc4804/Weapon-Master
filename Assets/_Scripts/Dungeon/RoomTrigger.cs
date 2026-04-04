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

        // Keep serialized reference in sync without spamming warnings.
        room = targetRoom;
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
