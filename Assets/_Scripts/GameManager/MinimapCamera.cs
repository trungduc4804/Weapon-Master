using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("Target & Follow")]
    [Tooltip("The Transform that the minimap camera should follow. Typically the Player.")]
    public Transform target;
    [Tooltip("How fast the minimap camera catches up strictly. Put higher values for instant snap.")]
    public float followSpeed = 10f;
    [Tooltip("Z axis offset to stay above the map.")]
    public float zOffset = -10f;

    [Header("Zoom")]
    [Tooltip("Orthographic size of the camera. The smaller the number, the closer the zoom.")]
    public float zoomLevel = 15f;

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
        if (target == null)
        {
            // Auto-find the player if target not manually set
            Player p = FindFirstObjectByType<Player>();
            if (p != null)
            {
                target = p.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Smoothly follow the target while keeping the Z offset
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, zOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Optional: Ensure minimap cam is properly zoomed in case it's changed at runtime
        if (minimapCam != null && minimapCam.orthographicSize != zoomLevel)
        {
            minimapCam.orthographicSize = Mathf.Lerp(minimapCam.orthographicSize, zoomLevel, Time.deltaTime * 5f);
        }
    }
}
