using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public float health = 100f;
    public float speedPlayer = 5f;
    private Vector2 movement;
    private Room currentRoom;  // Thêm để track phòng hiện tại
    private bool isKnockedBack = false;  // Thêm flag knockback
    public float knockbackForce = 5f;  // Lực knockback
    public int gold = 0;
    public int gachaRolls = 0;
    public int remainingQuestions = 0;
    [SerializeField] private int bossKeyCount = 0;
    public Room CurrentRoom => currentRoom;
    public int BossKeyCount => bossKeyCount;
    public bool HasBossKey => bossKeyCount > 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // TẢI DỮ LIỆU: Lấy số vàng đã lưu từ trước
        if (SaveManager.Instance != null)
        {
            gold = SaveManager.Instance.gameData.totalGold;
        }
    }

    void Update()
    {
        if (!isKnockedBack)  // Không di chuyển nếu đang knockback
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (movement != Vector2.zero)
            {
                animator.SetBool("isRun", true);
            }
            else
            {
                animator.SetBool("isRun", false);
            }

            // Lật mặt nhân vật theo hướng di chuyển
            if (movement.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        animator.SetTrigger("isHurt");

        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.CueLibrary.PlayerHurt);
        }

        // --- ADDED SCREEN SHAKE ---
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.Shake(0.2f, 0.15f);
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Thêm knockback
            EnemyBase enemy = FindFirstObjectByType<EnemyBase>();
            if (enemy != null)
            {
                Vector2 knockDir = (transform.position - enemy.transform.position).normalized;
                StartCoroutine(Knockback(knockDir));
            }
        }
    }

    private System.Collections.IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);  // Thời gian knockback
        isKnockedBack = false;
    }

    void FixedUpdate()
    {
        if (!isKnockedBack)
        {
            movement = movement.normalized;
            Vector2 newPos = rb.position + movement * speedPlayer * Time.fixedDeltaTime;

            // Kiểm tra boundary phòng (đơn giản: nếu cửa đóng, không cho ra ngoài trigger)
            if (currentRoom != null && !IsPointInRoomBounds(newPos))
            {
                rb.MovePosition(rb.position);
                return;
            }

            rb.MovePosition(newPos);
        }
    }

    private bool IsPointInRoomBounds(Vector2 point)
    {
        // TODO: Implement kiểm tra point có trong bounds của phòng không
        // Ví dụ: Sử dụng Bounds của room hoặc trigger collider
        return true;  // Tạm thời return true để test
    }

    public void SetCurrentRoom(Room room)
    {
        currentRoom = room;

        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.MoveToRoom(room);
        }
    }

    public void AddBossKey(int amount = 1)
    {
        if (amount <= 0)
        {
            return;
        }

        bossKeyCount += amount;
    }

    public bool TryConsumeBossKey(int amount = 1)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (bossKeyCount < amount)
        {
            return false;
        }

        bossKeyCount -= amount;
        return true;
    }

    private void Die()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isDie", true);

        // LƯU TIẾN TRÌNH KHI CHẾT
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.gameData.totalGold = gold;
            // Ví dụ: Nếu bạn muốn lưu kỷ lục level
            // if (currentRoomNumber > SaveManager.Instance.gameData.highscore) 
            //     SaveManager.Instance.gameData.highscore = currentRoomNumber;
            
            SaveManager.Instance.SaveGame();
        }

        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.CueLibrary.PlayerDeath);
        }

        enabled = false;
    }
}
