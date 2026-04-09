using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Current Weapon")]
    public WeaponBase currentWeapon;

    [Header("Weapons")]
    public WeaponBase meleeWeapon;
    public WeaponBase rangedWeapon;

    [Header("Input")]
    [SerializeField] private KeyCode meleeWeaponKey = KeyCode.Q;
    [SerializeField] private KeyCode rangedWeaponKey = KeyCode.E;

    void Start()
    {
        SwitchWeapon(meleeWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(meleeWeaponKey))
        {
            SwitchWeapon(meleeWeapon);
        }

        if (Input.GetKeyDown(rangedWeaponKey))
        {
            SwitchWeapon(rangedWeapon);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (currentWeapon != null)
                currentWeapon.Attack();
        }
    }

    void SwitchWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null)
        {
            return;
        }

        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);

    }

    public void AddDamage(float amount)
    {
        if (amount <= 0f) return;

        if (meleeWeapon is MeleeWeapon melee)
        {
            melee.damage += amount;
        }

        if (rangedWeapon is RangedWeapon ranged)
        {
            ranged.AddProjectileDamage(amount);
        }
    }
}
