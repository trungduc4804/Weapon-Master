using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public WeaponBase currentWeapon;

    public WeaponBase meleeWeapon;
    public WeaponBase rangedWeapon;

    void Start()
    {
        SwitchWeapon(meleeWeapon);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(meleeWeapon);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
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
        if (currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

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
