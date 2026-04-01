using UnityEngine;

public class EnemySkeleton2 : EnemyAI
{
    [SerializeField] private string hurtTrigger = "isHurt";
    [SerializeField] private string dieBool = "isDie";

    protected override void Start()
    {
        base.Start();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public override void TakeDamage(float incomingDamage)
    {
        if (animator != null && !string.IsNullOrEmpty(hurtTrigger))
        {
            animator.SetTrigger(hurtTrigger);
        }

        base.TakeDamage(incomingDamage);
    }

    public override void Die()
    {
        if (animator != null && !string.IsNullOrEmpty(dieBool))
        {
            animator.SetBool(dieBool, true);
        }

        base.Die();
    }
}
