using UnityEngine;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro
using UnityEngine.SceneManagement; // Dùng để load lại màn chơi

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("UI Hiển thị")]
    public TextMeshProUGUI moneyText; // Khai báo biến chứa Text tiền
    public TextMeshProUGUI baseHealthText; // Text màu đỏ hiển thị Máu Căn cứ
    public TextMeshProUGUI eventNotificationText; // Text thông báo sự kiện (Game bắt đầu, Đợt 1, 2, 3...)

    private Coroutine notificationCoroutine; // Quản lý thời gian hiển thị thông báo

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
            return;
        }

        // Đảm bảo cụm in-game UI luôn hiển thị nếu Scene không có MainMenuUIController
        EnsureInGameUIActive();

        FindOrCreateBaseHealthText();
        FindOrCreateEventNotificationText();
        FindGameOverPanelIfNull();
    }

    private void FindGameOverPanelIfNull()
    {
        if (gameOverPanel == null)
        {
            var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var t in allTransforms)
            {
                if ((t.name == "GameOverPanel" || t.name == "UIGameOver") && t.hideFlags == HideFlags.None)
                {
                    // Chỉ lấy panel con có chứa RestartButton hoặc TitleText
                    if (t.Find("RestartButton") != null || t.Find("TitleText") != null)
                    {
                        gameOverPanel = t.gameObject;
                        if (titleText == null)
                        {
                            var titleT = t.Find("TitleText");
                            if (titleT != null) titleText = titleT.GetComponent<TextMeshProUGUI>();
                        }
                        break;
                    }
                }
            }
        }
    }

    private void EnsureInGameUIActive()
    {
        if (MainMenuUIController.Instance == null)
        {
            // Tìm root UI hoặc UiMoney kể cả khi đang bị inactive
            GameObject uiMoney = GameObject.Find("UiMoney");
            if (uiMoney == null)
            {
                var allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
                foreach (var t in allTransforms)
                {
                    if (t.name == "UiMoney" && t.hideFlags == HideFlags.None)
                    {
                        t.gameObject.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                uiMoney.SetActive(true);
            }
        }
    }

    // Tự động tìm hoặc tạo UI thông báo sự kiện nằm ngay dưới BaseHealthText trong UiMoney
    private void FindOrCreateEventNotificationText()
    {
        if (eventNotificationText != null) return;

        GameObject uiMoneyObj = GameObject.Find("UiMoney");
        if (uiMoneyObj == null)
        {
            if (moneyText != null && moneyText.transform.parent != null)
            {
                uiMoneyObj = moneyText.transform.parent.gameObject;
            }
        }

        if (uiMoneyObj != null)
        {
            Transform existingNotice = uiMoneyObj.transform.Find("EventNotificationText");
            if (existingNotice != null)
            {
                eventNotificationText = existingNotice.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                GameObject noticeObj = new GameObject("EventNotificationText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                noticeObj.transform.SetParent(uiMoneyObj.transform, false);

                RectTransform rect = noticeObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(175f, -145f); // Nằm ngay phía dưới Máu (y = -100f)
                rect.sizeDelta = new Vector2(350f, 50f);

                eventNotificationText = noticeObj.GetComponent<TextMeshProUGUI>();
                eventNotificationText.fontSize = 28;
                eventNotificationText.fontStyle = FontStyles.Bold;
                eventNotificationText.alignment = TextAlignmentOptions.Left;

                if (moneyText != null)
                {
                    eventNotificationText.font = moneyText.font;
                    eventNotificationText.fontSharedMaterial = moneyText.fontSharedMaterial;
                }
            }
        }
    }

    // Hàm hiển thị thông báo sự kiện với màu sắc và thời lượng tùy chỉnh
    public void ShowEventNotification(string message, Color color, float duration = 3.5f)
    {
        FindOrCreateEventNotificationText();

        if (eventNotificationText == null) return;

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(NotificationRoutine(message, color, duration));
    }

    private System.Collections.IEnumerator NotificationRoutine(string message, Color color, float duration)
    {
        eventNotificationText.text = message;
        eventNotificationText.color = color;
        eventNotificationText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        // Hiệu ứng mờ dần (Fade out)
        float fadeTime = 0.8f;
        float elapsed = 0f;
        Color initialColor = eventNotificationText.color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            eventNotificationText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        eventNotificationText.gameObject.SetActive(false);
        notificationCoroutine = null;
    }

    // Tự động tìm Text hiển thị Máu Căn cứ bên trong UiMoney nếu chưa được kéo thả ở Inspector
    private void FindOrCreateBaseHealthText()
    {
        if (baseHealthText != null) return;

        // Tìm UiMoney trong Canvas
        GameObject uiMoneyObj = GameObject.Find("UiMoney");
        if (uiMoneyObj == null)
        {
            if (moneyText != null && moneyText.transform.parent != null)
            {
                uiMoneyObj = moneyText.transform.parent.gameObject;
            }
        }

        if (uiMoneyObj != null)
        {
            // Tìm UI con BaseHealthText hoặc Text_BaseHealth
            Transform existingHealth = uiMoneyObj.transform.Find("BaseHealthText");
            if (existingHealth == null)
            {
                existingHealth = uiMoneyObj.transform.Find("Text_BaseHealth");
            }

            if (existingHealth != null)
            {
                baseHealthText = existingHealth.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                // Nếu chưa có trong Scene thì tự động tạo UI con BaseHealthText để tiện chỉnh sửa
                GameObject healthObj = new GameObject("BaseHealthText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                healthObj.transform.SetParent(uiMoneyObj.transform, false);

                RectTransform rect = healthObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(125f, -100f); // Nằm ngay phía dưới dòng Vàng (y = -55f)
                rect.sizeDelta = new Vector2(250f, 50f);

                baseHealthText = healthObj.GetComponent<TextMeshProUGUI>();
                baseHealthText.fontSize = 36;
                baseHealthText.fontStyle = FontStyles.Bold;
                baseHealthText.color = new Color(0.95f, 0.2f, 0.2f, 1f); // Màu đỏ nổi bật
                baseHealthText.alignment = TextAlignmentOptions.Left;

                // Kế thừa font từ moneyText nếu có
                if (moneyText != null)
                {
                    baseHealthText.font = moneyText.font;
                    baseHealthText.fontSharedMaterial = moneyText.fontSharedMaterial;
                }
            }
        }
    }

    // --- BẮT ĐẦU OBSERVER PATTERN CHO TIỀN VÀ MÁU CĂN CỨ ---
    private void OnEnable()
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged += UpdateMoneyUI;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBaseHealthChanged += UpdateBaseHealthUI;
        }
    }

    private void OnDisable()
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBaseHealthChanged -= UpdateBaseHealthUI;
        }
    }

    private void Start()
    {
        FindOrCreateBaseHealthText();

        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
            CurrencySystem.Instance.OnMoneyChanged += UpdateMoneyUI;
            UpdateMoneyUI(CurrencySystem.Instance.GetCurrentMoney());
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnBaseHealthChanged -= UpdateBaseHealthUI;
            GameManager.Instance.OnBaseHealthChanged += UpdateBaseHealthUI;
            UpdateBaseHealthUI(GameManager.Instance.currentBaseHP, GameManager.Instance.maxBaseHP);
        }
        else
        {
            UpdateBaseHealthUI(10, 10);
        }

        // Nếu Scene không có Menu (chơi thẳng từ Editor), hiển thị thông báo "Game bắt đầu!"
        if (MainMenuUIController.Instance == null)
        {
            ShowEventNotification("Game bắt đầu!", new Color(0.2f, 0.95f, 0.3f, 1f), 3.5f);
        }
    }

    // Hàm cập nhật UI Máu Căn Cứ
    public void UpdateBaseHealthUI(int currentHP, int maxHP)
    {
        FindOrCreateBaseHealthText();

        if (baseHealthText != null)
        {
            baseHealthText.text = $"Máu: {currentHP}/{maxHP}";
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