using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [Tooltip("Súng = true (Xoay theo chuột). Đao/Kiếm = false (Chỉ lật theo Player)")]
    public bool isAimable = true;

    [Header("Item Data Binding")]
    public ShopItemData originData;

    [Header("Audio")]
    [SerializeField] private AudioCue attackCueOverride;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        animator = GetComponentInParent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (isAimable && Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 aimDirection = (mousePos - transform.position).normalized;
            
            // Xoay súng theo hướng chuột
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            
            // Do Player có lật Scale X (-1) khi quay trái, việc tính toán góc bị ngược với thế giới thực.
            // Để khắc phục, nếu Player đang lật trái, ta đảo ngược góc.
            if (transform.parent != null && transform.parent.localScale.x < 0)
            {
                // Khi parent có scale.x = -1, trục tọa độ local bị lật.
                // Để nòng súng luôn chĩa đúng con chuột ngoài thế giới thực,
                // ta cần tính lại Local Angle. Cách đơn giản nhất là set góc tuyệt đối nhưng dùng Euler local:
                angle = Mathf.Atan2(aimDirection.y, -aimDirection.x) * Mathf.Rad2Deg;
            }

            transform.localRotation = Quaternion.Euler(0, 0, angle);

            // Xử lý chống ngược súng (Flip Y) nếu chuột nằm bên trái nhân vật
            // Chuột nằm bên trái khi góc > 90 hoặc < -90
            bool isAimingLeft = Mathf.Abs(Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg) > 90f;
            
            if (spriteRenderer != null)
            {
                // Nếu súng chỉa sang trái thì phải FlipY để súng không lộn ngược bụng.
                // Do Player tự lật ScaleX khi quay trái, Súng cũng bị lật ScaleX theo!
                // Vì vậy, ta phải kết hợp: Nếu Player đang lật (-1) và súng cũng lật (-1) -> triệt tiêu nhau.
                // Nhưng Player đã tự lật hướng mặt rồi, nên ta chỉ cần Flip Y khi góc local yêu cầu.
                spriteRenderer.flipY = isAimingLeft;
            }
        }
        else
        {
            // Trả về mặc định cho Đao Kiếm
            transform.localRotation = Quaternion.identity;
            if (spriteRenderer != null) spriteRenderer.flipY = false;
        }
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
