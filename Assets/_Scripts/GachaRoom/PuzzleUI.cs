using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUI : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private PuzzleSystem puzzleSystem;
    [SerializeField] private Player player;

    [Header("Basic UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TMP_Text remainingQuestionsText;
    
    [Header("Puzzle Area (Khung Trắng)")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons = new Button[4];
    [SerializeField] private TMP_Text[] answerTexts = new TMP_Text[4];

    [Header("Colors & Feedback")]
    [SerializeField] private Color normalBtnColor = Color.white;
    [SerializeField] private Color correctBtnColor = Color.green;
    [SerializeField] private Color wrongBtnColor = Color.red;
    [SerializeField] private float delayAfterAnswer = 1.0f;

    private QuestionData currentQuestion;
    private bool isProcessingAnswer = false;

    private void Awake()
    {
        if (puzzleSystem == null) puzzleSystem = FindFirstObjectByType<PuzzleSystem>();
        if (player == null) player = FindFirstObjectByType<Player>();

        // Lắng nghe sự kiện click từ 4 nút
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Cache giá trị i cho lambda expression
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
        }
    }

    private void OnEnable()
    {
        GachaEvents.OnRemainingQuestionsChanged += UpdateRemainingText;
        RefreshUI();
        LoadNewQuestion();
    }

    private void OnDisable()
    {
        GachaEvents.OnRemainingQuestionsChanged -= UpdateRemainingText;
    }

    // --- CÁC HÀM XỬ LÝ NÚT BẤM (GẮN VÀO ONCLICK) ---

    public void OnBuyQuestionsClicked()
    {
        if (puzzleSystem != null)
        {
            puzzleSystem.BuyQuestions();
            // Nếu mua xong mà UI trống câu hỏi thì load luôn
            if (currentQuestion == null && player.remainingQuestions > 0)
            {
                LoadNewQuestion();
            }
        }
    }

    public void OpenUI()
    {
        puzzlePanel.SetActive(true);
        Time.timeScale = 0f; // Tạm dừng game
    }

    public void CloseUI()
    {
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f; // Tiếp tục game
    }

    // --- LOGIC CÂU HỎI ---

    private void LoadNewQuestion()
    {
        if (player == null || player.remainingQuestions <= 0)
        {
            ShowEmptyState("Bạn đã hết câu hỏi. Hãy mua thêm!");
            return;
        }

        currentQuestion = puzzleSystem.GetRandomQuestion();
        
        if (currentQuestion == null)
        {
            ShowEmptyState("Chưa có dữ liệu câu hỏi trong hệ thống!");
            return;
        }

        // Điền Text Câu Hỏi
        questionText.text = currentQuestion.questionText;

        // Điền Đáp Án và Reset màu
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = currentQuestion.answers[i];
                answerButtons[i].GetComponent<Image>().color = normalBtnColor;
                answerButtons[i].interactable = true;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        isProcessingAnswer = false;
    }

    private void OnAnswerButtonClicked(int selectedIndex)
    {
        // Chặn spam click
        if (isProcessingAnswer || currentQuestion == null || player.remainingQuestions <= 0) return;
        isProcessingAnswer = true;

        bool isCorrect = (selectedIndex == currentQuestion.correctIndex);

        // Đổi màu nút User chọn và nút kết quả đúng
        StartCoroutine(ShowAnswerFeedbackRoutine(selectedIndex, currentQuestion.correctIndex, isCorrect));

        // Submit cho Data xử lý (trừ lượt, gọi cộng Gacha roll nếu đúng)
        puzzleSystem.SubmitAnswer(currentQuestion, selectedIndex);
    }

    private IEnumerator ShowAnswerFeedbackRoutine(int selectedIndex, int correctIndex, bool isCorrect)
    {
        // Khóa các nút lại
        foreach (var btn in answerButtons) { btn.interactable = false; }

        Image selectedImage = answerButtons[selectedIndex].GetComponent<Image>();
        Image correctImage = answerButtons[correctIndex].GetComponent<Image>();

        if (isCorrect)
        {
            selectedImage.color = correctBtnColor;
            // Play âm thanh Đúng (VD)
            if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.CueLibrary.BuffPickup); 
        }
        else
        {
            selectedImage.color = wrongBtnColor;
            correctImage.color = correctBtnColor; // Show cho player biết đáp án đúng
            // Play âm thanh Sai (VD)
            if (AudioManager.Instance != null && AudioManager.Instance.CueLibrary != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.CueLibrary.PlayerHurt);
        }

        // Đợi 1 thời gian cho player nhìn kết quả (Dùng WaitForSecondsRealtime vì Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(delayAfterAnswer);

        // Load câu tiếp theo
        LoadNewQuestion();
    }

    private void UpdateRemainingText(int count)
    {
        if (remainingQuestionsText != null)
        {
            remainingQuestionsText.text = $"câu hỏi: {count}";
        }
    }

    private void RefreshUI()
    {
        if (player != null)
        {
            UpdateRemainingText(player.remainingQuestions);
        }
    }

    private void ShowEmptyState(string message)
    {
        currentQuestion = null;
        questionText.text = message;
        foreach (var btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }
}
