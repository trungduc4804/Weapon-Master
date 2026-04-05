using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private TMP_Text hpTextTMP;


    [Header("Coin UI")]
    [SerializeField] private TMP_Text coinTextTMP;

    [Header("Damage UI")]
    [SerializeField] private TMP_Text damageTextTMP;

    private float maxHealth;
    private float lastHealth = float.MinValue;
    private int lastCoin = int.MinValue;
    private float lastDamage = float.MinValue;

    void Awake()
    {
        if (player == null && autoFindPlayer)
        {
            player = FindFirstObjectByType<Player>();
        }

        TryResolvePlayerAttack();
    }

    void Start()
    {
        if (!TryResolvePlayer())
        {
            return;
        }

        maxHealth = Mathf.Max(1f, player.health);
        ForceRefresh();
    }

    void Update()
    {
        if (!TryResolvePlayer())
        {
            return;
        }

        if (playerAttack == null)
        {
            TryResolvePlayerAttack();
        }

        RefreshIfChanged();
    }

    private void RefreshIfChanged()
    {
        float currentDamage = GetCurrentDamage();

        if (!Mathf.Approximately(lastHealth, player.health) ||
            lastCoin != player.gold ||
            !Mathf.Approximately(lastDamage, currentDamage))
        {
            UpdateHealthUI(player.health);
            UpdateCoinUI(player.gold);
            UpdateDamageUI(currentDamage);
            lastHealth = player.health;
            lastCoin = player.gold;
            lastDamage = currentDamage;
        }
    }

    private void ForceRefresh()
    {
        float currentDamage = GetCurrentDamage();

        UpdateHealthUI(player.health);
        UpdateCoinUI(player.gold);
        UpdateDamageUI(currentDamage);
        lastHealth = player.health;
        lastCoin = player.gold;
        lastDamage = currentDamage;
    }

    private void UpdateHealthUI(float currentHealth)
    {
        if (currentHealth > maxHealth)
        {
            maxHealth = currentHealth;
        }

        float clampedHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = clampedHealth;
        }

        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = maxHealth > 0f ? clampedHealth / maxHealth : 0f;
        }

        string hpDisplay = $" {Mathf.CeilToInt(clampedHealth)}";

        if (hpTextTMP != null)
        {
            hpTextTMP.text = hpDisplay;
        }
    }

    private void UpdateCoinUI(int currentCoin)
    {
        string coinDisplay = $"{currentCoin}";

        if (coinTextTMP != null)
        {
            coinTextTMP.text = coinDisplay;
        }
    }

    private void UpdateDamageUI(float currentDamage)
    {
        string damageDisplay = $"{currentDamage:0.##}";

        if (damageTextTMP != null)
        {
            damageTextTMP.text = damageDisplay;
        }
    }

    private bool TryResolvePlayer()
    {
        if (player != null)
        {
            return true;
        }

        if (!autoFindPlayer)
        {
            return false;
        }

        player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            maxHealth = Mathf.Max(1f, player.health);
            ForceRefresh();
            TryResolvePlayerAttack();
            return true;
        }

        return false;
    }

    private void TryResolvePlayerAttack()
    {
        if (playerAttack != null)
        {
            return;
        }

        if (player != null)
        {
            playerAttack = player.GetComponent<PlayerAttack>();
        }

        if (playerAttack == null && autoFindPlayer)
        {
            playerAttack = FindFirstObjectByType<PlayerAttack>();
        }
    }

    private float GetCurrentDamage()
    {
        if (playerAttack == null || playerAttack.currentWeapon == null)
        {
            return 0f;
        }

        if (playerAttack.currentWeapon is MeleeWeapon melee)
        {
            return melee.damage;
        }

        if (playerAttack.currentWeapon is RangedWeapon ranged)
        {
            return ranged.GetProjectileDamage();
        }

        return 0f;
    }
}
