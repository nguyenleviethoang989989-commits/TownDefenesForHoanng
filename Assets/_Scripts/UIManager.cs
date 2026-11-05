using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro
using UnityEngine.SceneManagement; // Dùng để load lại màn chơi

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("UI Hiển thị")]
    public TextMeshProUGUI moneyText; // Khai báo biến chứa Text tiền
    [Header("Màn hình Kết thúc")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI titleText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // --- BẮT ĐẦU OBSERVER PATTERN CHO TIỀN ---
    private void OnEnable()
    {
        // Khi UIManager được bật, lập tức lắng nghe sự kiện OnMoneyChanged từ CurrencySystem
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged += UpdateMoneyUI;
        }
    }

    private void OnDisable()
    {
        // Phải HỦY lắng nghe khi bị tắt để tránh lỗi Memory Leak
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
        }
    }
    private void Start()
    {
        // Chủ động lấy số tiền hiện tại để ép UI cập nhật một lần ngay khi mới vào Game
        if (CurrencySystem.Instance != null)
        {
            UpdateMoneyUI(CurrencySystem.Instance.GetCurrentMoney());
        }
    }
    // Hàm này sẽ TỰ ĐỘNG CHẠY mỗi khi CurrencySystem gọi OnMoneyChanged
    private void UpdateMoneyUI(int currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = "Vàng: " + currentMoney;
        }
    }
    // Hàm gọi khi Thua
    public void ShowLoseScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Bật panel lên
            titleText.text = "CĂN CỨ BỊ PHÁ HỦY!";
            titleText.color = Color.red;
        }
    }

    // Hàm gọi khi Thắng
    public void ShowWinScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            titleText.text = "BẠN ĐÃ CHIẾN THẮNG!";
            titleText.color = Color.green;
        }
    }

    // Gắn vào nút "Chơi Lại"
    public void RestartGame()
    {
        Time.timeScale = 1f; // Phải đưa time về lại 1 trước khi load
        // Load lại Scene hiện tại đang chơi
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Gắn vào nút "Về Menu" (Nếu bạn có Scene Menu)
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene"); // Đổi tên này cho đúng tên file Scene menu của bạn
    }
}