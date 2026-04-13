using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BossDoorLock : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Player player;
    [SerializeField] private Room room;
    [SerializeField] private Door[] lockedDoors;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Behavior")]
    [SerializeField] private bool consumeKeyOnUnlock = true;
    [SerializeField] private string lockedMessage = "Ban can mua chia khoa trong shop de vao phong boss.";
    [SerializeField] private string unlockedMessage = "Da mo cua boss.";

    [Header("Trigger Placement")]
    [SerializeField] private float wallInset = 1.2f;
    [SerializeField] private float triggerWidth = 3f;
    [SerializeField] private float triggerDepth = 1.5f;

    private bool isUnlocked;
    private BoxCollider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        ResolvePlayer();
        ResolveRoom();
        ApplyLockedState(true);
    }

    private IEnumerator Start()
    {
        // Wait until DungeonGenerator has connected the boss room doors.
        yield return null;
        ConfigureTriggerAtEntrance();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUnlocked)
        {
            return;
        }

        Player targetPlayer = other.GetComponentInParent<Player>();
        if (targetPlayer == null)
        {
            return;
        }

        if (player == null)
        {
            player = targetPlayer;
        }

        if (player == null || !player.HasBossKey)
        {
            ShowMessage(lockedMessage);
            return;
        }

        if (consumeKeyOnUnlock && !player.TryConsumeBossKey())
        {
            ShowMessage(lockedMessage);
            return;
        }

        isUnlocked = true;
        ApplyLockedState(false);
        ShowMessage(unlockedMessage);
    }

    private void ResolvePlayer()
    {
        if (player == null && autoFindPlayer)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    private void ResolveRoom()
    {
        if (room == null)
        {
            room = GetComponentInParent<Room>();
        }
    }

    private void ConfigureTriggerAtEntrance()
    {
        if (triggerCollider == null || room == null)
        {
            return;
        }

        if (room.HasTopConnection)
        {
            triggerCollider.offset = new Vector2(0f, wallInset);
            triggerCollider.size = new Vector2(triggerWidth, triggerDepth);
            return;
        }

        if (room.HasBottomConnection)
        {
            triggerCollider.offset = new Vector2(0f, -wallInset);
            triggerCollider.size = new Vector2(triggerWidth, triggerDepth);
            return;
        }

        if (room.HasLeftConnection)
        {
            triggerCollider.offset = new Vector2(-wallInset, 0f);
            triggerCollider.size = new Vector2(triggerDepth, triggerWidth);
            return;
        }

        if (room.HasRightConnection)
        {
            triggerCollider.offset = new Vector2(wallInset, 0f);
            triggerCollider.size = new Vector2(triggerDepth, triggerWidth);
        }
    }

    private void ApplyLockedState(bool locked)
    {
        if (lockedDoors == null)
        {
            return;
        }

        for (int i = 0; i < lockedDoors.Length; i++)
        {
            if (lockedDoors[i] != null)
            {
                lockedDoors[i].SetLocked(locked);
            }
        }
    }

    private void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }
    }
}
