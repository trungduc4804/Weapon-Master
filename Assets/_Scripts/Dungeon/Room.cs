using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Door doorTop;
    public Door doorBottom;
    public Door doorLeft;
    public Door doorRight;
    public bool HasTopConnection => hasTop;
    public bool HasBottomConnection => hasBottom;
    public bool HasLeftConnection => hasLeft;
    public bool HasRightConnection => hasRight;

    public EnemySpawner enemySpawner;

    int enemiesAlive = 0;
    bool cleared = false;
    bool encounterStarted = false;
    Coroutine closeDoorsRoutine;

    bool hasTop;
    bool hasBottom;
    bool hasLeft;
    bool hasRight;

    [Header("Minimap")]
    [Tooltip("If checked, the room icon on the minimap layer will hide until the player enters it.")]
    public bool hideMinimapIconUntilEntered = true;
    private readonly System.Collections.Generic.List<GameObject> minimapIcons = new System.Collections.Generic.List<GameObject>();

    private void Start()
    {
        int minimapLayer = LayerMask.NameToLayer("Minimap");
        if (minimapLayer != -1 && hideMinimapIconUntilEntered)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.gameObject.layer == minimapLayer && child != transform)
                {
                    minimapIcons.Add(child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetDoors(bool top, bool bottom, bool left, bool right)
    {
        hasTop = top;
        hasBottom = bottom;
        hasLeft = left;
        hasRight = right;

    }

    public bool IsCleared()
    {
        return cleared;
    }

    public void PlayerEntered(Vector2Int entryDir)
    {
        foreach (var icon in minimapIcons)
        {
            if (icon != null) icon.SetActive(true);
        }

        if (cleared)
        {
            OpenDoors();
            return;
        }

        // Prevent spawning enemies more than once per room.
        if (encounterStarted)
        {
            if (enemiesAlive > 0)
            {
                QueueCloseDoors();
            }
            else
            {
                OpenDoors();
            }
            return;
        }

        encounterStarted = true;

        if (enemySpawner != null)
        {
            enemiesAlive = enemySpawner.SpawnEnemies(this);

            if (enemiesAlive > 0)
            {
                QueueCloseDoors();
            }
            else
            {
                cleared = true;
                OpenDoors();
            }
        }
        else
        {
            cleared = true;
            OpenDoors();
        }
    }

    public void PlayerEntered()
    {
        PlayerEntered(Vector2Int.zero);
    }

    public void EnemyKilled()
    {
        if (!encounterStarted || cleared)
        {
            return;
        }

        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        if (enemiesAlive <= 0)
        {
            cleared = true;
            if (closeDoorsRoutine != null)
            {
                StopCoroutine(closeDoorsRoutine);
                closeDoorsRoutine = null;
            }
            OpenDoors();
        }
    }

    void QueueCloseDoors()
    {
        if (closeDoorsRoutine != null)
        {
            return;
        }

        closeDoorsRoutine = StartCoroutine(CloseDoorsDelayed());
    }

    IEnumerator CloseDoorsDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        closeDoorsRoutine = null;

        if (cleared || enemiesAlive <= 0)
        {
            OpenDoors();
            yield break;
        }

        CloseDoors();
    }

    public void CloseDoors()
    {
        if (hasTop && doorTop) 
        {
            doorTop.SetClosed(true);
        }
        if (hasBottom && doorBottom) 
        {
            doorBottom.SetClosed(true);
        }
        if (hasLeft && doorLeft) 
        {
            doorLeft.SetClosed(true);
        }
        if (hasRight && doorRight) 
        {
            doorRight.SetClosed(true);
        }
    }

    void OpenDoors()
    {
        if (hasTop && doorTop) doorTop.SetClosed(false);
        if (hasBottom && doorBottom) doorBottom.SetClosed(false);
        if (hasLeft && doorLeft) doorLeft.SetClosed(false);
        if (hasRight && doorRight) doorRight.SetClosed(false);
    }
}
