using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Điểm số hiện tại")]
    public int currentScore = 0;
    public int enemiesKilled = 0;

    // Sự kiện phát ra khi điểm thay đổi (Observer Pattern để cập nhật UI)
    public event Action<int> OnScoreChanged;

    private const string HIGH_SCORE_KEY_PREFIX = "HighScore_";

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

    private void Start()
    {
        currentScore = 0;
        enemiesKilled = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    // Cộng điểm khi tiêu diệt quái vật (dựa theo loại quái hoặc số tiền thưởng)
    public void AddScore(int amount)
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        currentScore += amount;
        enemiesKilled++;
        OnScoreChanged?.Invoke(currentScore);
    }

    // Lấy điểm cao nhất của Scene/Map hiện tại
    public int GetHighScore()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY_PREFIX + sceneName, 0);
    }

    // Lấy điểm cao nhất của một Map bất kỳ theo tên scene
    public static int GetHighScoreForMap(string mapName)
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY_PREFIX + mapName, 0);
    }

    // Lưu lại điểm nếu đạt kỷ lục mới khi Thắng hoặc Thua
    public bool SaveScoreIfHighScore()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string key = HIGH_SCORE_KEY_PREFIX + sceneName;
        int oldHighScore = PlayerPrefs.GetInt(key, 0);

        if (currentScore > oldHighScore)
        {
            PlayerPrefs.SetInt(key, currentScore);
            PlayerPrefs.Save();
            Debug.Log($"<color=yellow><b>[ScoreManager]</b> KỶ LỤC MỚI CHO {sceneName}: {currentScore} ĐIỂM (Kỷ lục cũ: {oldHighScore})</color>");
            return true; // Đạt kỷ lục mới
        }

        return false;
    }

    // Đặt lại điểm số (dùng khi restart hoặc test)
    public void ResetCurrentScore()
    {
        currentScore = 0;
        enemiesKilled = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
}
