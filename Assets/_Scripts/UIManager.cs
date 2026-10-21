using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro
using UnityEngine.SceneManagement; // Dùng để load lại màn chơi

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

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