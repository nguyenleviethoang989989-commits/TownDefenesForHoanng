using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Pause")]
    public GameObject pausePanel; // Panel giao diện Pause Menu
    public Canvas pauseCanvas;    // Canvas của Pause Menu (Sorting Order = 95)

    [Header("Trạng thái")]
    public bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseCanvas == null && pausePanel != null)
        {
            pauseCanvas = pausePanel.GetComponentInParent<Canvas>();
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.sortingOrder = 95; // Nằm trên In-game (0, 50) và dưới Menu (100)
        }
    }

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 1. Không nhận ESC nếu chưa bắt đầu vào màn chơi (vẫn ở Menu hoặc Loading)
        if (MainMenuUIController.Instance != null && !MainMenuUIController.Instance.isGameStarted)
        {
            return;
        }

        // 2. Không nhận ESC nếu game đã kết thúc (Game Over hoặc Win)
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        // 3. Bắt sự kiện phím ESC (hỗ trợ cả New Input System và Legacy)
        bool escPressed = false;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escPressed = true;
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escPressed = true;
        }
#endif

        if (escPressed)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Tạm dừng game
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Dừng toàn bộ thời gian trong game

        if (pausePanel != null)
        {
            pausePanel.transform.SetAsLastSibling();
            pausePanel.SetActive(true);
        }

        // Ẩn các UI có thể đè lên
        GameObject uiMoney = GameObject.Find("UiMoney");
        if (uiMoney != null) uiMoney.SetActive(false);

        if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.inGameSoundButton != null)
        {
            MainMenuUIController.Instance.inGameSoundButton.SetActive(false);
        }

        Debug.Log("Game đã TẠM DỪNG (PAUSED)!");
    }

    // Tiếp tục chơi (Nút Resume)
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Khôi phục thời gian game

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Hiện lại UI trong game
        GameObject uiMoney = GameObject.Find("UiMoney");
        if (uiMoney != null) uiMoney.SetActive(true);

        if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.inGameSoundButton != null)
        {
            MainMenuUIController.Instance.inGameSoundButton.SetActive(true);
        }

        Debug.Log("Game TIẾP TỤC (RESUMED)!");
    }

    // Quay về Menu (Nút Back Menu)
    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f; // Bắt buộc trả timeScale về 1f

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Tải lại Scene để quay về UI Main Menu ban đầu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}