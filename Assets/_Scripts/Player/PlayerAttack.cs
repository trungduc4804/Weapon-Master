using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Current Weapon")]
    public WeaponBase currentWeapon;

    [Header("Weapons")]
    public WeaponBase weaponSlot1;
    public WeaponBase weaponSlot2;

    [Header("Input")]
    [SerializeField] private KeyCode slot1Key = KeyCode.Q;
    [SerializeField] private KeyCode slot2Key = KeyCode.E;

    void Start()
    {
        SwitchWeapon(weaponSlot1);
    }

    void Update()
    {
        if (Input.GetKeyDown(slot1Key))
        {
            SwitchWeapon(weaponSlot1);
        }

        if (Input.GetKeyDown(slot2Key))
        {
            SwitchWeapon(weaponSlot2);
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

    public void EquipWeaponToSlot(WeaponBase newWeapon, int slotIndex)
    {
        if (newWeapon == null) return;
        
        // Ensure weapon is attached to the player
        newWeapon.transform.SetParent(transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.gameObject.SetActive(false);

        if (slotIndex == 1)
        {
            if (weaponSlot1 != null && weaponSlot1 != currentWeapon) 
                weaponSlot1.gameObject.SetActive(false);
            
            weaponSlot1 = newWeapon;
            if (currentWeapon == null || Input.GetKey(slot1Key)) SwitchWeapon(weaponSlot1);
        }
        else if (slotIndex == 2)
        {
            if (weaponSlot2 != null && weaponSlot2 != currentWeapon) 
                weaponSlot2.gameObject.SetActive(false);
                
            weaponSlot2 = newWeapon;
            if (currentWeapon == null || Input.GetKey(slot2Key)) SwitchWeapon(weaponSlot2);
        }
    }

    public void AddDamage(float amount)
    {
        if (amount <= 0f || currentWeapon == null) return;

        if (currentWeapon is MeleeWeapon melee)
        {
            melee.damage += amount;
        }
        else if (currentWeapon is RangedWeapon ranged)
        {
            ranged.AddProjectileDamage(amount);
        }
    }
}
