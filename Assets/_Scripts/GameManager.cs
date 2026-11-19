using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Tạo Singleton để các class khác dễ dàng truy cập vào GameManager
    public static GameManager Instance;

    [Header("Chỉ số Căn cứ")]
    public int maxBaseHP = 10; // Bắt đầu với 10 máu căn cứ
    public int currentBaseHP;

    // Sự kiện phát ra khi máu căn cứ thay đổi (Observer Pattern dùng cho UI)
    public event Action<int, int> OnBaseHealthChanged;

    [Header("Trạng thái Game")]
    public bool isGameOver = false;

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
            currentBaseHP = maxBaseHP;
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
        OnBaseHealthChanged?.Invoke(currentBaseHP, maxBaseHP);
    }

    // Hàm nhận sát thương cho Căn cứ
    public void TakeDamage(int amount)
    {
        if (isGameOver) return; // Nếu game over rồi thì không trừ máu nữa

        currentBaseHP -= amount;
        if (currentBaseHP < 0) currentBaseHP = 0;

        Debug.Log("Căn cứ bị lọt lưới! Bị trừ " + amount + " máu. Máu còn lại: " + currentBaseHP);

        // Phát sự kiện cập nhật UI
        OnBaseHealthChanged?.Invoke(currentBaseHP, maxBaseHP);

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