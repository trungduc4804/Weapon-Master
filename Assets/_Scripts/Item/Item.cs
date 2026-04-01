using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        Gold,
        Health,
        Damage
    }

    public ItemType itemType;

    public int goldAmount = 1;
    public float healthAmount = 2f;
    public float damageAmount = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player == null) return;

        switch (itemType)
        {
            case ItemType.Gold:
                player.gold += goldAmount;
                break;

            case ItemType.Health:
                player.health += healthAmount;
                break;

            case ItemType.Damage:
                PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
                if (playerAttack != null)
                {
                    playerAttack.AddDamage(damageAmount);
                }
                break;
        }

        Destroy(gameObject);
    }
}
