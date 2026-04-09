using UnityEngine;

public class EnemyBoss : EnemyAI
{
    [SerializeField] private string hurtTrigger = "isHurt";
    [SerializeField] private string dieBool = "isDie";

    private bool bossMusicActive;

    protected override void Start()
    {
        base.Start();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isDead || bossMusicActive || player == null || room == null)
        {
            return;
        }

        if (player.CurrentRoom != room)
        {
            return;
        }

        bossMusicActive = true;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicState(AudioMusicState.Boss);
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusicState(AudioMusicState.Gameplay);
        }

        base.Die();
    }
}
