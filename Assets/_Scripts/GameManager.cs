using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Tạo Singleton để các class khác dễ dàng truy cập vào GameManager
    public static GameManager Instance;

    [Header("Chỉ số Căn cứ")]
    public int maxBaseHP = 20; // Trùng khớp với thiết kế GDD
    public int currentBaseHP;

    [Header("Trạng thái Game")]
    public bool isGameOver = false;

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Đảm bảo chỉ có duy nhất 1 GameManager tồn tại
        }
    }

    private void Start()
    {
        currentBaseHP = maxBaseHP;
        Debug.Log("Game bắt đầu! Máu căn cứ: " + currentBaseHP);
    }

    // Hàm nhận sát thương cho Căn cứ
    public void TakeDamage(int amount)
    {
        if (isGameOver) return; // Nếu game over rồi thì không trừ máu nữa

        currentBaseHP -= amount;
        Debug.Log("Căn cứ bị lọt lưới! Bị trừ " + amount + " máu. Máu còn lại: " + currentBaseHP);

        // Kiểm tra điều kiện thua
        if (currentBaseHP <= 0)
        {
            currentBaseHP = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! Căn cứ đã bị phá hủy.");
        // Gọi UIManager hiển thị màn hình thua
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLoseScreen();
        }
        // Tạm thời dừng thời gian trong game lại khi thua
        Time.timeScale = 0f;

    }
    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("YOU WIN! Đã dọn sạch quái.");

        // Gọi UIManager hiển thị màn hình thắng
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowWinScreen();
        }

        Time.timeScale = 0f;
    }

}