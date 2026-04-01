using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public abstract void Attack();
}
