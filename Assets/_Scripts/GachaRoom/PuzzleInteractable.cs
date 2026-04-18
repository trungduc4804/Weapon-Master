using UnityEngine;

public class PuzzleInteractable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Khoảng cách tối đa để người chơi tương tác với NPC giải đố.")]
    public float interactDistance = 3f;

    private PuzzleUI puzzleUI;
    private Transform playerTransform;

    private void Start()
    {
        puzzleUI = FindFirstObjectByType<PuzzleUI>();
        
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void OpenPuzzleInterface()
    {
        if (puzzleUI == null)
        {
            Debug.LogWarning("Không tìm thấy PuzzleUI trong scene. Vui lòng check lại.");
            return;
        }

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance > interactDistance)
            {
                Debug.Log("Bạn đang đứng quá xa NPC!");
                return;
            }
        }

        puzzleUI.OpenUI();
    }

    private void OnMouseDown()
    {
        OpenPuzzleInterface();
    }
}
