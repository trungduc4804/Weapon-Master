using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats Configuration")]
    [SerializeField] protected EnemyStatsData stats;

    [HideInInspector] public float speedEnemy = 3f;
    [HideInInspector] public float hp = 100f;
    [HideInInspector] public float damage = 10f;
    [HideInInspector] public float attackRange = 1f;
    [HideInInspector] public float attackCooldown = 1f;
    [HideInInspector] public float knockbackForce = 8f;
    [HideInInspector] public float knockbackTime = 0.15f;
    [HideInInspector] public GameObject goldPrefab;
    [HideInInspector] public float dropChance = 0.5f;
    [HideInInspector] public float detectionRange = 5f;

    protected float lastAttackTime = 0f;

    protected Rigidbody2D rb;
    protected Player player;

    protected bool isDead = false;
    protected bool canMove = true;
    protected bool isKnocked = false;
    protected bool isChasing = false;

    protected Room room;
    protected virtual void Awake()
    {
        if (stats != null)
        {
            hp = stats.maxHp;
            speedEnemy = stats.speedEnemy;
            damage = stats.damage;
            attackRange = stats.attackRange;
            attackCooldown = stats.attackCooldown;
            knockbackForce = stats.knockbackForce;
            knockbackTime = stats.knockbackTime;
            goldPrefab = stats.goldPrefab;
            dropChance = stats.dropChance;
            detectionRange = stats.detectionRange;
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name} chua g?n EnemyStatsData!");
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>();
    }

    protected virtual void Update()
    {
        if (!canMove || isDead || player == null)
            return;

        if (player.health <= 0)
            return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        isChasing = distance <= detectionRange;

        if (isChasing)
            MoveTowardsPlayer();
    }

    // ROOM LINK
    public virtual void SetRoom(Room newRoom)
    {
        room = newRoom;
    }

    // MOVE TO PLAYER
    protected virtual void MoveTowardsPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = (player.transform.position - transform.position).normalized;

        rb.linearVelocity = dir * speedEnemy;

        Flip(dir.x);
    }

    // FLIP SPRITE
    protected void Flip(float x)
    {
        if (x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // DAMAGE
    public virtual void TakeDamage(float dmg)
    {
        if (isDead) return;

        hp -= dmg;

        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlaySFXAtPoint(AudioManager.Instance.CueLibrary.EnemyHurt, transform.position);
        }

        Vector2 knockDir = Vector2.zero;
        if (player != null)
        {
            knockDir = (transform.position - player.transform.position).normalized;
        }

        StartCoroutine(Knockback(knockDir));

        if (hp <= 0)
            Die();
    }

    // KNOCKBACK
    protected IEnumerator Knockback(Vector2 direction)
    {
        if (rb == null)
        {
            yield break;
        }

        isKnocked = true;
        canMove = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        if (rb == null)
        {
            yield break;
        }

        rb.linearVelocity = Vector2.zero;

        if (isDead)
        {
            yield break;
        }

        isKnocked = false;
        canMove = true;
    }

    // DIE
    public virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        canMove = false;

        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlaySFXAtPoint(AudioManager.Instance.CueLibrary.EnemyDeath, transform.position);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false;
        }

        // báo room enemy đã chết
        if (room != null)
            room.EnemyKilled();
        DropItem();
        Destroy(gameObject, 2f);
    }

    public virtual void DropItem()
    {
        if (Random.value > dropChance)
        {
            return;
        }

        if (goldPrefab == null)
        {
            return;
        }

        Instantiate(goldPrefab, transform.position, Quaternion.identity);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if (stats != null)
        {
            // Vẽ tầm nhìn (Màu vàng)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.detectionRange);

            // Vẽ tầm đánh (Màu đỏ)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRange);
        }
        else
        {
            // Dự phòng nếu chưa gán stats (trong chế độ Edit)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
#endif
}

