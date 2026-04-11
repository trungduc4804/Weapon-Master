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
        if (attackCueOverride != null && attackCueOverride.HasClip)
        {
            AudioManager.Instance.PlaySFX(attackCueOverride);
        }
        else if (fallbackCue != null && fallbackCue.HasClip)
        {
            AudioManager.Instance.PlaySFX(fallbackCue);
        }
        else
        {
            Debug.LogWarning("No valid attack sound!");
        }

        AudioManager.Instance.PlaySFX(cueToPlay);
    }

    public abstract void Attack();
}
