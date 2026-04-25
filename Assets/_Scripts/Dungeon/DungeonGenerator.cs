using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject shopRoom; // Add shopRoom reference
    public GameObject gachaRoom; // Add gachaRoom reference
    public GameObject[] normalRooms;

    public int roomCount = 8;
    public float roomDistance = 16f;

    Dictionary<Vector2Int, GameObject> spawnedRooms =
        new Dictionary<Vector2Int, GameObject>();

    List<Vector2Int> possiblePositions =
        new List<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        spawnedRooms.Clear();
        possiblePositions.Clear();

        Vector2Int startPos = Vector2Int.zero;

        SpawnRoom(startRoom, startPos);

        possiblePositions.Add(Vector2Int.up);
        possiblePositions.Add(Vector2Int.down);
        possiblePositions.Add(Vector2Int.left);
        possiblePositions.Add(Vector2Int.right);

        for (int i = 0; i < roomCount - 2; i++)
        {
            if (possiblePositions.Count == 0)
                break;

            int index = Random.Range(0, possiblePositions.Count);
            Vector2Int pos = possiblePositions[index];

            possiblePositions.RemoveAt(index);

            if (spawnedRooms.ContainsKey(pos))
                continue;

            if (normalRooms.Length == 0)
                continue;

            GameObject randomRoom =
                normalRooms[Random.Range(0, normalRooms.Length)];

            SpawnRoom(randomRoom, pos);

            AddNewPositions(pos);
        }

        SpawnBossRoom();
        SpawnShopRoom();
        SpawnGachaRoom();

        ConnectRooms();

        MovePlayerToStart();
    }

    void SpawnBossRoom()
    {
        if (possiblePositions.Count == 0)
            return;

        Vector2Int bestPos = possiblePositions[0];
        float bestDistance = Vector2Int.Distance(Vector2Int.zero, bestPos);

        foreach (Vector2Int pos in possiblePositions)
        {
            float dist = Vector2Int.Distance(Vector2Int.zero, pos);

            if (dist > bestDistance)
            {
                bestDistance = dist;
                bestPos = pos;
            }
        }

        SpawnRoom(bossRoom, bestPos);
        possiblePositions.Remove(bestPos); // Remove pos to prevent overlaps
    }

    void SpawnShopRoom()
    {
        if (shopRoom == null) return;
        if (possiblePositions.Count == 0) return;

        int index = Random.Range(0, possiblePositions.Count);
        Vector2Int pos = possiblePositions[index];

        SpawnRoom(shopRoom, pos);
        possiblePositions.RemoveAt(index);
    }

    void SpawnGachaRoom()
    {
        if (gachaRoom == null) return;
        if (possiblePositions.Count == 0) return;

        int index = Random.Range(0, possiblePositions.Count);
        Vector2Int pos = possiblePositions[index];

        SpawnRoom(gachaRoom, pos);
        possiblePositions.RemoveAt(index);
    }

    void SpawnRoom(GameObject prefab, Vector2Int gridPos)
    {
        if (spawnedRooms.ContainsKey(gridPos))
            return;

        Vector3 worldPos = new Vector3(
            gridPos.x * roomDistance,
            gridPos.y * roomDistance,
            0
        );

        GameObject room =
            Instantiate(prefab, worldPos, Quaternion.identity);

        spawnedRooms.Add(gridPos, room);
    }

    void AddNewPositions(Vector2Int pos)
    {
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int newPos = pos + dir;

            if (!spawnedRooms.ContainsKey(newPos)
                && !possiblePositions.Contains(newPos))
            {
                possiblePositions.Add(newPos);
            }
        }
    }

    void ConnectRooms()
    {
        foreach (var pair in spawnedRooms)
        {
            Vector2Int pos = pair.Key;

            Room room = pair.Value.GetComponent<Room>();

            if (!room) continue;

            bool hasTop = spawnedRooms.ContainsKey(pos + Vector2Int.up);
            bool hasBottom = spawnedRooms.ContainsKey(pos + Vector2Int.down);
            bool hasLeft = spawnedRooms.ContainsKey(pos + Vector2Int.left);
            bool hasRight = spawnedRooms.ContainsKey(pos + Vector2Int.right);

            room.SetDoors(hasTop, hasBottom, hasLeft, hasRight);

            if (room.doorTop) room.doorTop.SetClosed(!hasTop, false);
            if (room.doorBottom) room.doorBottom.SetClosed(!hasBottom, false);
            if (room.doorLeft) room.doorLeft.SetClosed(!hasLeft, false);
            if (room.doorRight) room.doorRight.SetClosed(!hasRight, false);
        }
    }

    void MovePlayerToStart()
    {
        Player player = FindFirstObjectByType<Player>();

        if (player == null) return;

        GameObject startRoomInstance = spawnedRooms[Vector2Int.zero];

        player.transform.position = startRoomInstance.transform.position;

        Room startRoomComp = startRoomInstance.GetComponent<Room>();

        if (startRoomComp != null)
        {
            player.SetCurrentRoom(startRoomComp);

            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SnapToRoom(startRoomComp);
            }
        }
    }
}