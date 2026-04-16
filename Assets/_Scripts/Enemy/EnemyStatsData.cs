using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    [Header("Stats")]
    public float maxHp = 100f;
    public float speedEnemy = 3f;
    public float damage = 10f;

    [Header("Combat")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackTime = 0.15f;

    [Header("Detection")]
    public float detectionRange = 5f;

    [Header("Loot")]
    public GameObject goldPrefab;
    [Range(0, 1)] public float dropChance = 0.5f;
}
