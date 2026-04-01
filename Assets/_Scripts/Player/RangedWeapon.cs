using UnityEngine;

public class RangedWeapon : WeaponBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float damage = 10f;

    public float delayBetweenShots = 0.5f;
    private float lastShotTime = 0f;

    public override void Attack()
    {
        if (Time.time < lastShotTime + delayBetweenShots)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        FirePoint projectileDamage = projectile.GetComponent<FirePoint>();
        if (projectileDamage != null)
        {
            projectileDamage.damage = damage;
        }

        lastShotTime = Time.time;
                
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
