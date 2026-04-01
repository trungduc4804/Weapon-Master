using UnityEngine;
using System.Collections;

public class MeleeWeapon : WeaponBase
{
    public float damage = 10f;
    public float attackDuration = 0.3f;
    public BoxCollider2D attackCollider;

    private bool isAttacking = false;

    public override void Attack()
    {
        if (isAttacking) return;

        if (animator != null)
            animator.SetTrigger("isAttack");

        StartCoroutine(EnableCollider());
        
    }

    IEnumerator EnableCollider()
    {
        isAttacking = true;

        attackCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        attackCollider.enabled = false;

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!attackCollider.enabled) return;


        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}