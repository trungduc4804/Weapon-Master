using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Door doorTop;
    public Door doorBottom;
    public Door doorLeft;
    public Door doorRight;

    public EnemySpawner enemySpawner;

    int enemiesAlive = 0;
    bool cleared = false;
    bool encounterStarted = false;
    Coroutine closeDoorsRoutine;

    bool hasTop;
    bool hasBottom;
    bool hasLeft;
    bool hasRight;

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
