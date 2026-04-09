using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioCue attackCueOverride;

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    protected void PlayAttackSound(AudioCue fallbackCue)
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioCue cueToPlay = attackCueOverride != null ? attackCueOverride : fallbackCue;
        if (cueToPlay == null)
        {
            return;
        }

        AudioManager.Instance.PlaySFX(cueToPlay);
    }

    public abstract void Attack();
}
