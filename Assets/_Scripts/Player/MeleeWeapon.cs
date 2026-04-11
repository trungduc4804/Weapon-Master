using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeWeapon : WeaponBase
{
    public float damage = 10f;
    public float attackDuration = 0.3f;
    public BoxCollider2D attackCollider;

    private bool isAttacking = false;
    private readonly HashSet<EnemyBase> hitEnemies = new HashSet<EnemyBase>();

    protected override void Awake()
    {
        base.Awake();

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    public override void Attack()
    {
        if (isAttacking || attackCollider == null) return;

        if (animator != null)
            animator.SetTrigger("isAttack");

        AudioCue fallbackCue = null;
        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            fallbackCue = AudioManager.Instance.CueLibrary.SwordAttack;
        }

        PlayAttackSound(fallbackCue);

        StartCoroutine(EnableCollider());
        
    }

    IEnumerator EnableCollider()
    {
        isAttacking = true;
        hitEnemies.Clear();

        attackCollider.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        attackCollider.enabled = false;

        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDealDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDealDamage(collision);
    }

    private void TryDealDamage(Collider2D collision)
    {
        if (!isAttacking || attackCollider == null || !attackCollider.enabled) return;
        if (!collision.CompareTag("Enemy")) return;

        EnemyBase enemy = collision.GetComponentInParent<EnemyBase>();
        if (enemy == null || hitEnemies.Contains(enemy)) return;

        enemy.TakeDamage(damage);
        hitEnemies.Add(enemy);
    }
}
