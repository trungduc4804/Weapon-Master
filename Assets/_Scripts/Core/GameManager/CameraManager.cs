using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public float moveSpeed = 6f;

    private bool isMoving = false;

    private Vector3 currentTargetPosition;

    void Awake()
    {
        Instance = this;
        currentTargetPosition = transform.position;
    }

    public void MoveToRoom(Room room)
    {
        if (room == null) return;
        
        Vector3 target = room.transform.position;
        target.z = -10;
        currentTargetPosition = target;

        if (isMoving) return;

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

        currentTargetPosition = pos;
        transform.position = pos;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(currentTargetPosition.x + x, currentTargetPosition.y + y, currentTargetPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = currentTargetPosition;
    }
}