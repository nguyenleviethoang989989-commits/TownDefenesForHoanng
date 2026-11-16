using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIController : MonoBehaviour
{
    public static MainMenuUIController Instance;

    [Header("Canvas của Menu & Loading")]
    public Canvas menuCanvas;            // Canvas riêng của Menu (Sorting Order cao nhất = 100)

    [Header("Các Panel UI")]
    public GameObject menuPanel;          // Panel Main Menu
    public GameObject loadingPanel;       // Panel Loading Game
    public GameObject inGameSoundButton;  // Nút Speaker của màn hình Game

    [Header("Các Canvas/UI trong Game cần ẩn khi ở Menu")]
    public GameObject[] inGameUIElements; // UIMoney, UIBuild, UIGameOver, ...

    [Header("Loading UI Elements")]
    public Slider loadingSlider;            // Thanh Slider tiến trình load
    public TextMeshProUGUI loadingText;     // Text hiển thị "Loading... (XX%)"
    public float loadDuration = 2.5f;       // Thời gian thanh loading chạy đầy (giây)

    [Header("Trạng thái game")]
    public bool isGameStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // 1. Tạm ngưng WaveManager nếu có
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.enabled = false;
        }

        // 2. ẨN TẤT CẢ CÁC UI TRONG GAME (UIMoney, UIBuild, UIGameOver, ...)
        SetInGameUIActive(false);

        // 3. Ẩn nút Speaker In-Game khi đang ở Menu
        if (inGameSoundButton != null)
        {
            inGameSoundButton.SetActive(false);
        }

        // 4. Bật Menu Canvas và đưa lên trên cùng
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(true);
            menuCanvas.sortingOrder = 100; // Cao hơn hẳn tất cả các Canvas khác
        }

        // Bật Menu Panel, ẩn Loading Panel
        if (menuPanel != null)
        {
            menuPanel.transform.SetAsLastSibling();
            menuPanel.SetActive(true);
        }
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        // 5. Phát nhạc nền Menu
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    // Gắn vào sự kiện OnClick của Nút Play
    public void OnPlayButtonClicked()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (loadingPanel != null)
        {
            loadingPanel.transform.SetAsLastSibling();
            loadingPanel.SetActive(true);
        }

        if (inGameSoundButton != null) inGameSoundButton.SetActive(false);

        StartCoroutine(LoadingRoutine());
    }

    // Gắn vào sự kiện OnClick của Nút Quit
    public void OnQuitButtonClicked()
    {
        Debug.Log("Đã bấm QUIT GAME!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadingRoutine()
    {
        if (loadingSlider != null) loadingSlider.value = 0f;

        float timer = 0f;
        while (timer < loadDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / loadDuration);

            if (loadingSlider != null)
            {
                loadingSlider.value = progress;
            }

            if (loadingText != null)
            {
                int percent = Mathf.RoundToInt(progress * 100f);
                loadingText.text = $"Loading... {percent}%";
            }

            yield return null;
        }

        if (loadingSlider != null) loadingSlider.value = 1f;
        if (loadingText != null) loadingText.text = "Loading... 100%";

        yield return new WaitForSecondsRealtime(0.3f);

        EnterGamePlay();
    }

    private void EnterGamePlay()
    {
        isGameStarted = true;

        // 1. Tắt toàn bộ Menu Canvas
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
        }
        else
        {
            if (loadingPanel != null) loadingPanel.SetActive(false);
            if (menuPanel != null) menuPanel.SetActive(false);
        }

        // 2. HIỆN LẠI CÁC UI TRONG GAME (UIMoney, UIBuild, ...)
        SetInGameUIActive(true);

        // 3. Hiện nút Speaker In-Game
        if (inGameSoundButton != null) inGameSoundButton.SetActive(true);

        // 4. Kích hoạt WaveManager bắt đầu đợt quái đầu tiên
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.enabled = true;
        }

        // 5. Chuyển sang nhạc nền Gameplay
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        Debug.Log("<color=green>Đã load 100%! Bắt đầu màn chơi game!</color>");
    }

    private void SetInGameUIActive(bool active)
    {
        if (inGameUIElements != null)
        {
            foreach (var elem in inGameUIElements)
            {
                if (elem != null) elem.SetActive(active);
            }
        }
    }
}