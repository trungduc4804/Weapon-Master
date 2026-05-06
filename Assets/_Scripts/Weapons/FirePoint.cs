using UnityEngine;

public class FirePoint : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;
    private float timer;

    void OnEnable()
    {
        timer = lifeTime;
        // Đảm bảo tọa độ Z luôn bằng 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset vận tốc cũ
            rb.angularVelocity = 0f;
            rb.WakeUp();
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToPool("Timeout");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu là Player thì luôn bỏ qua
        if (collision.CompareTag("Player")) return;

        // Nếu là Trigger thì bỏ qua, TRỪ KHI đó là Enemy
        if (collision.isTrigger && !collision.CompareTag("Enemy"))
        {
            return;
        }
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            ReturnToPool("Hit Enemy");
        }
        else if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Default", "Obstacle")) != 0) 
        {
            ReturnToPool("Hit Wall/Obstacle");
        }
    }

    private void ReturnToPool(string reason = "Lifetime/Other")
    {
        if (CorePoolManager.Instance != null)
        {
            CorePoolManager.Instance.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
