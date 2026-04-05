using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float speedEnemy = 3f;
    [SerializeField] protected float hp = 100f;
    [SerializeField] protected float damage = 10f;

    [Header("Combat")]
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] protected float knockbackForce = 8f;
    [SerializeField] protected float knockbackTime = 0.15f;
    [SerializeField] protected GameObject goldPrefab;

    [Range(0,1)] [SerializeField] float dropChance = 0.5f;

    [Header("Detection")]
    public float detectionRange = 5f;

    protected float lastAttackTime = 0f;

    protected Rigidbody2D rb;
    protected Player player;

    protected bool isDead = false;
    protected bool canMove = true;
    protected bool isKnocked = false;
    protected bool isChasing = false;

    protected Room room;

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
}
