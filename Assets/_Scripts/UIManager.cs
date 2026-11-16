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
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged += UpdateMoneyUI;
        }
    }

    private void OnDisable()
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
        }
    }

    private void Start()
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
            CurrencySystem.Instance.OnMoneyChanged += UpdateMoneyUI;
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
            // Đưa gameOverPanel lên trên cùng
            Canvas gameOverCanvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (gameOverCanvas != null)
            {
                gameOverCanvas.sortingOrder = 90; // Cao hơn UiMoney (0) và thấp hơn Menu (100)
            }
            gameOverPanel.transform.SetAsLastSibling();
            gameOverPanel.SetActive(true);

            // Cập nhật tiêu đề Game Over
            if (titleText != null)
            {
                titleText.text = "GAME OVER";
                titleText.color = Color.red;
            }

            // Ẩn UiMoney khi hiện GameOver
            GameObject uiMoney = GameObject.Find("UiMoney");
            if (uiMoney != null) uiMoney.SetActive(false);

            // Ẩn nút Speaker In-Game khi hiện GameOver
            if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.inGameSoundButton != null)
            {
                MainMenuUIController.Instance.inGameSoundButton.SetActive(false);
            }
        }
    }

    // Hàm gọi khi Thắng
    public void ShowWinScreen()
    {
        if (gameOverPanel != null)
        {
            Canvas gameOverCanvas = gameOverPanel.GetComponentInParent<Canvas>();
            if (gameOverCanvas != null)
            {
                gameOverCanvas.sortingOrder = 90;
            }
            gameOverPanel.transform.SetAsLastSibling();
            gameOverPanel.SetActive(true);

            if (titleText != null)
            {
                titleText.text = "VICTORY!";
                titleText.color = Color.green;
            }

            GameObject uiMoney = GameObject.Find("UiMoney");
            if (uiMoney != null) uiMoney.SetActive(false);

            if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.inGameSoundButton != null)
            {
                MainMenuUIController.Instance.inGameSoundButton.SetActive(false);
            }
        }
    }

    // Gắn vào nút "Chơi Lại"
    public void RestartGame()
    {
        Time.timeScale = 1f; // Trả lại timeScale bình thường
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Gắn vào nút "Về Menu"
    public void GoToMenu()
    {
        Time.timeScale = 1f; // Bắt buộc trả timeScale về 1f

        // Nếu Scene hiện tại có MainMenuUIController, mở lại Menu ngay lập tức
        if (MainMenuUIController.Instance != null)
        {
            // Reset trạng thái và tải lại Scene hiện tại để bắt đầu từ UI Menu
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Hoặc load về SampleScene (Màn chính)
            SceneManager.LoadScene(0);
        }
    }
}