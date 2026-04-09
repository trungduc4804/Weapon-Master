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
                PlayPickupSound(AudioManager.Instance != null ? AudioManager.Instance.CueLibrary?.GoldPickup : null);
                break;

            case ItemType.Health:
                player.health += healthAmount;
                PlayPickupSound(AudioManager.Instance != null ? AudioManager.Instance.CueLibrary?.HealthPickup : null);
                break;

            case ItemType.Damage:
                PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
                if (playerAttack != null)
                {
                    playerAttack.AddDamage(damageAmount);
                }
                PlayPickupSound(AudioManager.Instance != null ? AudioManager.Instance.CueLibrary?.BuffPickup : null);
                break;
        }

        Destroy(gameObject);
    }

    private void PlayPickupSound(AudioCue cue)
    {
        if (AudioManager.Instance == null || cue == null)
        {
            return;
        }

        AudioManager.Instance.PlaySFXAtPoint(cue, transform.position);
    }
}
