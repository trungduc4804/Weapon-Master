using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public int SpawnEnemies(Room room)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs in EnemySpawner!");
            return 0;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points in EnemySpawner!");
            return 0;
        }

        int count = 0;

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            GameObject enemy = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                spawn.position,
                Quaternion.identity
            );

            EnemyBase e = enemy.GetComponent<EnemyBase>();

            if (e != null)
            {
                e.SetRoom(room);
            }

            count++;
        }

        return count;
    }
}
