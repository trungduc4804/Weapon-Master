using UnityEngine;

public class FirePoint : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;
    private float timer;

    void OnEnable()
    {
        timer = lifeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            ReturnToPool();
        }
        else if (collision.CompareTag("Wall")) // Giả định có tag Wall để đạn biến mất khi đập tường
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
