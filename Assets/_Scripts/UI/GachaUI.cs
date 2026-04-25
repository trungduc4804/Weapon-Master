using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private GachaSystem gachaSystem;
    [SerializeField] private Player player;
    [SerializeField] private GachaDropRateData dropRateData; // Để lấy Icon fake lúc animation

    [Header("Basic UI")]
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private TMP_Text rollsText;
    [SerializeField] private Button rollButton;

    [Header("Animation View (Trong Khung Trắng)")]
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private TMP_Text rewardNameText;
    [SerializeField] private float spinningDuration = 4f;

    private bool isSpinning = false;

    private void Awake()
    {
        if (gachaSystem == null) gachaSystem = FindFirstObjectByType<GachaSystem>();
        if (player == null) player = FindFirstObjectByType<Player>();

        if (rollButton != null)
        {
            rollButton.onClick.AddListener(OnRollButtonClicked);
        }
        
        // Tắt tạm Hình ảnh và Text lúc chưa quay
        if (rewardIconImage != null) rewardIconImage.gameObject.SetActive(false);
        if (rewardNameText != null) rewardNameText.text = "Bấm Quay để thử vận may!";
    }

    private void OnEnable()
    {
        GachaEvents.OnGachaRollsCountChanged += UpdateRollsText;
        RefreshUI();
    }

    private void OnDisable()
    {
        GachaEvents.OnGachaRollsCountChanged -= UpdateRollsText;
    }

    public void OpenUI()
    {
        gachaPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseUI()
    {
        if (isSpinning) return; // Đang quay không cho đóng panel
        gachaPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnRollButtonClicked()
    {
        if (isSpinning) return;
        
        if (player == null || player.gachaRolls <= 0)
        {
            rewardNameText.text = "Bạn đã hết lượt quay!";
            return;
        }

        // 1. Tính toán trước xem trúng cái gì
        ShopItemData rewardedItem = gachaSystem.PreCalculateRoll();
        
        if (rewardedItem != null)
        {
            // 2. Chạy Animation 3 giây
            StartCoroutine(SpinAnimationRoutine(rewardedItem));
        }
    }

    private IEnumerator SpinAnimationRoutine(ShopItemData finalReward)
    {
        isSpinning = true;
        rollButton.interactable = false;
        if (rewardIconImage != null) rewardIconImage.gameObject.SetActive(true);

        float timer = 0f;
        // Bắt đầu nhanh (0.05s) rồi chậm dần lại
        float currentDelay = 0.05f; 

        // Lấy danh sách các item có thể rơi để làm hiệu ứng roll giả (fake list)
        List<ShopItemData> possibleItems = new List<ShopItemData>();
        if (dropRateData != null)
        {
            foreach (var item in dropRateData.items)
                possibleItems.Add(item.itemData);
        }

        // Phát âm thanh quay 4 giây (chỉ phát 1 lần đầu tiên)
        if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
        {
            AudioManager.Instance.PlayUI(AudioManager.Instance.CueLibrary.GachaSpin);
        }

        // Loop đổi hình liên tục
        // Dùng WaitForSecondsRealtime vì Time.timeScale đang = 0
        while (timer < spinningDuration)
        {
            // Random hình ảnh fake để hiện
            if (possibleItems.Count > 0)
            {
                ShopItemData fakeItem = possibleItems[Random.Range(0, possibleItems.Count)];
                if (rewardIconImage != null && fakeItem.itemIcon != null)
                {
                    rewardIconImage.sprite = fakeItem.itemIcon;
                }
                if (rewardNameText != null)
                {
                    rewardNameText.text = "...Đang quay...";
                }
            }

            yield return new WaitForSecondsRealtime(currentDelay); 
            timer += currentDelay;
            //Tăng delay lên một chút để tạo cảm giác vòng quay Gacha quay chậm dần
            float t = Mathf.Clamp01(timer / spinningDuration);
            // Dùng t * t * t giúp vòng quay giữ tốc độ nhanh ở phần lớn thời gian đầu,
            // và chỉ thực sự chậm lại dần (delay tăng mạnh) ở những giây cuối cùng.
            currentDelay = Mathf.Lerp(0.05f, 0.6f, t * t * t);
        }

        // ---- KẾT THÚC ANIMATION ----
        // Show đồ thật
        if (rewardIconImage != null && finalReward.itemIcon != null)
        {
            rewardIconImage.sprite = finalReward.itemIcon;
        }
        if (rewardNameText != null)
        {
            rewardNameText.text = finalReward.itemName + "!";
        }

        // Nhét đồ vào Player
        gachaSystem.GiveItemToPlayer(finalReward);

        isSpinning = false;
        rollButton.interactable = true;
    }

    private void UpdateRollsText(int count)
    {
        if (rollsText != null)
        {
            rollsText.text = $"LƯỢT: {count}";
        }
    }

    private void RefreshUI()
    {
        if (player != null)
        {
            UpdateRollsText(player.gachaRolls);
        }
    }
}
