using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("Target & Follow")]
    [Tooltip("The Player to find current room.")]
    public Player player;
    [Tooltip("How fast the minimap camera catches up.")]
    public float followSpeed = 15f;
    [Tooltip("Z axis offset to stay above the map.")]
    public float zOffset = -10f;

    [Header("Zoom & Style")]
    [Tooltip("Orthographic size of the camera. Higher means see more rooms.")]
    public float zoomLevel = 35f;

    [Tooltip("Bật True: Camera chốt cứng vào căn phòng hiện tại (Giống The Binding of Isaac). Bật False: Camera trôi mượt theo người (Giống Diablo).")]
    public bool snapToRoomCenter = true;

    private Camera minimapCam;

    private void Awake()
    {
        minimapCam = GetComponent<Camera>();
        if (minimapCam != null)
        {
            minimapCam.orthographicSize = zoomLevel;
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.transform.position;

        // Nếu chế độ Isaac được bập, camera sẽ ghim thẳng vào giữa phòng hiện tại thay vì giữa lưng người chơi
        if (snapToRoomCenter && player.CurrentRoom != null)
        {
            targetPos = player.CurrentRoom.transform.position;
        }

        Vector3 desiredPosition = new Vector3(targetPos.x, targetPos.y, zOffset);

        // Di chuyển camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Đảm bảo zoom luôn chạy
        if (minimapCam != null && minimapCam.orthographicSize != zoomLevel)
        {
            minimapCam.orthographicSize = Mathf.Lerp(minimapCam.orthographicSize, zoomLevel, Time.deltaTime * 5f);
        }
    }
}
