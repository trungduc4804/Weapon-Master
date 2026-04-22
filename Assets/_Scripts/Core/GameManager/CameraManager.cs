using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float moveSpeed = 6f;

    private bool isMoving = false;

    void Awake()
    {
        Instance = this;
    }

    public void MoveToRoom(Room room)
    {
        if (room == null) return;
        if (isMoving) return;

        Vector3 target = room.transform.position;
        target.z = -10;

        StartCoroutine(SmoothMove(target));
    }

    IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;

        Vector3 start = transform.position;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        transform.position = target;

        isMoving = false;
    }

    public void SnapToRoom(Room room)
    {
        if (room == null) return;

        Vector3 pos = room.transform.position;
        pos.z = -10;

        transform.position = pos;
    }
}