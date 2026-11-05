using System;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    // Tạo Singleton để có thể gọi ở mọi nơi: CurrencySystem.Instance.AddMoney()
    public static CurrencySystem Instance { get; private set; }

    [Header("Cài đặt Tiền tệ")]
    [SerializeField] private int startingMoney = 100; // Số tiền ban đầu khi bắt đầu Level
    private int currentMoney;

    // Sự kiện (Event) phát ra mỗi khi số tiền thay đổi (Dùng cho UI Update)
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Có nhiều hơn 1 CurrencySystem trong Scene! Đang xóa bản sao.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Khởi tạo tiền ban đầu
        currentMoney = startingMoney;
        Debug.Log("Game bắt đầu! Số tiền khởi điểm: " + currentMoney);

        // Gọi sự kiện để UI cập nhật số tiền ngay lúc bắt đầu
        OnMoneyChanged?.Invoke(currentMoney);
    }

    // Hàm lấy số tiền hiện tại (Chỉ đọc)
    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    // Hàm kiểm tra xem có đủ tiền để mua không
    public bool HasEnoughMoney(int amount)
    {
        return currentMoney >= amount;
    }

    // Hàm CỘNG TIỀN (Gọi khi giết quái)
    public void AddMoney(int amount)
    {
        if (amount < 0) return; // Không cho phép cộng số âm

        currentMoney += amount;
        Debug.Log($"+{amount} Vàng. Tiền hiện tại: {currentMoney}");

        // Phát sự kiện cho UI biết tiền đã thay đổi
        OnMoneyChanged?.Invoke(currentMoney);
    }

    // Hàm TRỪ TIỀN (Gọi khi xây tháp)
    public void SpendMoney(int amount)
    {
        if (amount < 0) return;

        if (HasEnoughMoney(amount))
        {
            currentMoney -= amount;
            Debug.Log($"-{amount} Vàng. Tiền hiện tại: {currentMoney}");

            // Phát sự kiện cho UI biết tiền đã thay đổi
            OnMoneyChanged?.Invoke(currentMoney);
        }
        else
        {
            Debug.LogWarning("KHÔNG ĐỦ TIỀN ĐỂ TIÊU!");
        }
    }
}