using UnityEngine;
using UnityEngine.UI;
using TMPro; // Bắt buộc phải có để dùng TextMeshPro
using UnityEngine.SceneManagement; // Dùng để load lại màn chơi

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("UI Hiển thị")]
    public TextMeshProUGUI moneyText; // Khai báo biến chứa Text tiền
    public TextMeshProUGUI baseHealthText; // Text màu đỏ hiển thị Máu Căn cứ
    public TextMeshProUGUI scoreText; // Text màu cam hiển thị Điểm số
    public TextMeshProUGUI eventNotificationText; // Text thông báo sự kiện (Game bắt đầu, Đợt 1, 2, 3...)

    private Coroutine notificationCoroutine; // Quản lý thời gian hiển thị thông báo

    [Header("Màn hình Kết thúc")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI resultScoreText; // Text hiển thị điểm số và kỷ lục ở màn hình kết thúc

    private GameObject mapSelectionPanel; // Panel chọn map với 5 nút map đẹp mắt

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
        FindOrCreateScoreText();
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

    // Tự động tìm hoặc tạo Text hiển thị Điểm Số bên trong UiMoney
    private void FindOrCreateScoreText()
    {
        if (scoreText != null) return;

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
            Transform existingScore = uiMoneyObj.transform.Find("ScoreText");
            if (existingScore != null)
            {
                scoreText = existingScore.GetComponent<TextMeshProUGUI>();
                if (scoreText != null)
                {
                    RectTransform rect = scoreText.rectTransform;
                    if (rect != null)
                    {
                        rect.anchoredPosition = new Vector2(149.4f, -254.2f);
                    }
                }
            }
            else
            {
                GameObject scoreObj = new GameObject("ScoreText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                scoreObj.transform.SetParent(uiMoneyObj.transform, false);

                RectTransform rect = scoreObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(149.4f, -254.2f); // Cập nhật Pos X: 149.4, Pos Y: -254.2
                rect.sizeDelta = new Vector2(250f, 45f);

                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
                scoreText.fontSize = 32;
                scoreText.fontStyle = FontStyles.Bold;
                scoreText.color = new Color(1f, 0.65f, 0.1f, 1f); // Màu vàng cam nổi bật
                scoreText.alignment = TextAlignmentOptions.Left;

                if (moneyText != null)
                {
                    scoreText.font = moneyText.font;
                    scoreText.fontSharedMaterial = moneyText.fontSharedMaterial;
                }
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
                if (eventNotificationText != null)
                {
                    RectTransform rect = eventNotificationText.rectTransform;
                    if (rect != null)
                    {
                        rect.anchoredPosition = new Vector2(272.9f, -193.2f);
                    }
                }
            }
            else
            {
                GameObject noticeObj = new GameObject("EventNotificationText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                noticeObj.transform.SetParent(uiMoneyObj.transform, false);

                RectTransform rect = noticeObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(272.9f, -193.2f); // Cập nhật Pos X: 272.9, Pos Y: -193.2
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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
        }
    }

    private void Start()
    {
        FindOrCreateBaseHealthText();
        FindOrCreateScoreText();

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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            UpdateScoreUI(ScoreManager.Instance.currentScore);
        }
        else
        {
            UpdateScoreUI(0);
        }

        // Nếu Scene không có Menu (chơi thẳng từ Editor), hiển thị thông báo "Game bắt đầu!"
        if (MainMenuUIController.Instance == null)
        {
            ShowEventNotification("Game bắt đầu!", new Color(0.2f, 0.95f, 0.3f, 1f), 3.5f);
        }
    }

    // Cập nhật UI Điểm số
    public void UpdateScoreUI(int score)
    {
        FindOrCreateScoreText();

        if (scoreText != null)
        {
            scoreText.text = "Điểm: " + score;
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

    // Căn giữa dòng Title và 2 Nút bấm để nằm chính xác tâm màn hình và thẳng hàng
    private void CenterGameOverUIElements()
    {
        if (gameOverPanel == null) return;

        // Căn chỉnh TitleText: Đặt chính giữa tâm trục X = 0, Y = 140
        if (titleText != null)
        {
            RectTransform titleRect = titleText.rectTransform;
            if (titleRect != null)
            {
                titleRect.anchorMin = new Vector2(0.5f, 0.5f);
                titleRect.anchorMax = new Vector2(0.5f, 0.5f);
                titleRect.pivot = new Vector2(0.5f, 0.5f);
                titleRect.anchoredPosition = new Vector2(0f, 140f);
                titleRect.sizeDelta = new Vector2(600f, 90f);
                titleRect.localScale = Vector3.one;
                titleRect.localRotation = Quaternion.identity;
            }

            // Xóa triệt để margin (lề âm/dương gây lệch chữ sang phải)
            titleText.margin = Vector4.zero;

            // Thiết lập căn giữa toàn diện cho TextMeshPro
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            titleText.verticalAlignment = VerticalAlignmentOptions.Middle;
        }

        // Căn chỉnh Text hiển thị Điểm ở Game Over: x = 0, y = 75, rộng 500, cao 50
        UpdateGameOverScoreDisplay();

        // Căn chỉnh Nút trên (RestartButton / Chọn Map): x = 0, y = 10, kích thước 220 x 65
        Transform topBtn = gameOverPanel.transform.Find("RestartButton");
        if (topBtn == null) topBtn = gameOverPanel.transform.Find("Btn_Restart");
        if (topBtn != null)
        {
            RectTransform topRect = topBtn.GetComponent<RectTransform>();
            if (topRect != null)
            {
                topRect.anchorMin = new Vector2(0.5f, 0.5f);
                topRect.anchorMax = new Vector2(0.5f, 0.5f);
                topRect.pivot = new Vector2(0.5f, 0.5f);
                topRect.anchoredPosition = new Vector2(0f, 10f);
                topRect.sizeDelta = new Vector2(220f, 65f);
                topRect.localScale = Vector3.one;
            }
        }

        // Căn chỉnh Nút dưới (MenuButton / Back): x = 0, y = -75, kích thước 220 x 65
        Transform botBtn = gameOverPanel.transform.Find("MenuButton");
        if (botBtn == null) botBtn = gameOverPanel.transform.Find("Btn_Menu");
        if (botBtn != null)
        {
            RectTransform botRect = botBtn.GetComponent<RectTransform>();
            if (botRect != null)
            {
                botRect.anchorMin = new Vector2(0.5f, 0.5f);
                botRect.anchorMax = new Vector2(0.5f, 0.5f);
                botRect.pivot = new Vector2(0.5f, 0.5f);
                botRect.anchoredPosition = new Vector2(0f, -75f);
                botRect.sizeDelta = new Vector2(220f, 65f);
                botRect.localScale = Vector3.one;
            }
        }
    }

    // Hiển thị Điểm Đạt Được và Kỷ Lục Cao Nhất trên màn hình kết thúc (Thắng / Thua)
    private void UpdateGameOverScoreDisplay()
    {
        if (gameOverPanel == null) return;

        if (resultScoreText == null)
        {
            Transform existingScore = gameOverPanel.transform.Find("ResultScoreText");
            if (existingScore != null)
            {
                resultScoreText = existingScore.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                GameObject scoreObj = new GameObject("ResultScoreText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                scoreObj.transform.SetParent(gameOverPanel.transform, false);

                resultScoreText = scoreObj.GetComponent<TextMeshProUGUI>();
                if (titleText != null)
                {
                    resultScoreText.font = titleText.font;
                    resultScoreText.fontSharedMaterial = titleText.fontSharedMaterial;
                }
            }
        }

        if (resultScoreText != null)
        {
            RectTransform scoreRect = resultScoreText.rectTransform;
            scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
            scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
            scoreRect.pivot = new Vector2(0.5f, 0.5f);
            scoreRect.anchoredPosition = new Vector2(0f, 75f); // Nằm giữa TitleText (140) và nút trên (10)
            scoreRect.sizeDelta = new Vector2(500f, 50f);
            scoreRect.localScale = Vector3.one;

            resultScoreText.margin = Vector4.zero;
            resultScoreText.fontSize = 26;
            resultScoreText.fontStyle = FontStyles.Bold;
            resultScoreText.alignment = TextAlignmentOptions.Center;
            resultScoreText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            resultScoreText.verticalAlignment = VerticalAlignmentOptions.Middle;
            resultScoreText.color = new Color(1f, 0.9f, 0.3f, 1f);

            int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.currentScore : 0;
            int highScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetHighScore() : currentScore;

            resultScoreText.text = $"Điểm: {currentScore}  |  Kỷ lục: {highScore}";
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

            // Căn thẳng hàng và chính giữa
            CenterGameOverUIElements();

            // Cập nhật tiêu đề Game Over
            if (titleText != null)
            {
                titleText.text = "GAME OVER";
                titleText.color = Color.red;
            }

            // Thiết lập nút cho màn hình Thua:
            // Nút 1: Chơi Lại (RestartGame)
            // Nút 2: Về Menu (GoToMenu)
            Transform restartBtn = gameOverPanel.transform.Find("RestartButton");
            if (restartBtn == null) restartBtn = gameOverPanel.transform.Find("Btn_Restart");
            if (restartBtn != null)
            {
                TextMeshProUGUI btnText = restartBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "Chơi Lại";
                Button b = restartBtn.GetComponent<Button>();
                if (b != null)
                {
                    b.onClick = new Button.ButtonClickedEvent();
                    b.onClick.AddListener(RestartGame);
                    b.navigation = new Navigation { mode = Navigation.Mode.None };
                }
            }

            Transform menuBtn = gameOverPanel.transform.Find("MenuButton");
            if (menuBtn == null) menuBtn = gameOverPanel.transform.Find("Btn_Menu");
            if (menuBtn != null)
            {
                TextMeshProUGUI btnText = menuBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "Về Menu";
                Button b = menuBtn.GetComponent<Button>();
                if (b != null)
                {
                    b.onClick = new Button.ButtonClickedEvent();
                    b.onClick.AddListener(GoToMenu);
                    b.navigation = new Navigation { mode = Navigation.Mode.None };
                }
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

            // Căn thẳng hàng và chính giữa
            CenterGameOverUIElements();

            // Title: WIN
            if (titleText != null)
            {
                titleText.text = "WIN";
                titleText.color = new Color(0.2f, 1f, 0.3f, 1f); // Màu xanh lá chiến thắng rực rỡ
            }

            // 2 Nút:
            // Nút 1 (MenuButton hoặc Back): Quay lại UIMenu (GoToMenu)
            // Nút 2 (RestartButton / Chọn Map): Mở MapSelectionPanel
            Transform menuBtn = gameOverPanel.transform.Find("MenuButton");
            if (menuBtn == null) menuBtn = gameOverPanel.transform.Find("Btn_Menu");
            if (menuBtn != null)
            {
                TextMeshProUGUI btnText = menuBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "Back (Menu)";
                Button b = menuBtn.GetComponent<Button>();
                if (b != null)
                {
                    b.onClick = new Button.ButtonClickedEvent();
                    b.onClick.AddListener(GoToMenu);
                    b.navigation = new Navigation { mode = Navigation.Mode.None };
                }
            }

            Transform mapBtn = gameOverPanel.transform.Find("RestartButton");
            if (mapBtn == null) mapBtn = gameOverPanel.transform.Find("Btn_Restart");
            if (mapBtn != null)
            {
                TextMeshProUGUI btnText = mapBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "Chọn Map";
                Button b = mapBtn.GetComponent<Button>();
                if (b != null)
                {
                    b.onClick = new Button.ButtonClickedEvent();
                    b.onClick.AddListener(OpenMapSelectionUI);
                    b.navigation = new Navigation { mode = Navigation.Mode.None };
                }
            }

            GameObject uiMoney = GameObject.Find("UiMoney");
            if (uiMoney != null) uiMoney.SetActive(false);

            if (MainMenuUIController.Instance != null && MainMenuUIController.Instance.inGameSoundButton != null)
            {
                MainMenuUIController.Instance.inGameSoundButton.SetActive(false);
            }
        }
    }

    // Mở bảng chọn Map với 5 nút có hình ảnh giao diện chuẩn giống nút Chọn Map
    public void OpenMapSelectionUI()
    {
        CreateOrShowMapSelectionPanel();
    }

    private void CreateOrShowMapSelectionPanel()
    {
        if (mapSelectionPanel != null)
        {
            mapSelectionPanel.SetActive(true);
            mapSelectionPanel.transform.SetAsLastSibling();
            return;
        }

        // Lấy Canvas cha từ gameOverPanel hoặc tìm Canvas UIGameOver
        Transform canvasTrans = gameOverPanel != null ? gameOverPanel.transform.parent : transform;

        // Lấy hình ảnh Sprite và Font mẫu từ nút RestartButton
        Sprite btnSprite = null;
        TMP_FontAsset fontAsset = null;
        Material fontMat = null;

        if (gameOverPanel != null)
        {
            Transform sampleBtn = gameOverPanel.transform.Find("RestartButton");
            if (sampleBtn != null)
            {
                Image sampleImg = sampleBtn.GetComponent<Image>();
                if (sampleImg != null) btnSprite = sampleImg.sprite;

                TextMeshProUGUI sampleText = sampleBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (sampleText != null)
                {
                    fontAsset = sampleText.font;
                    fontMat = sampleText.fontSharedMaterial;
                }
            }
        }

        // 1. Tạo Panel Chọn Map cha
        mapSelectionPanel = new GameObject("MapSelectionPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        mapSelectionPanel.transform.SetParent(canvasTrans, false);

        RectTransform panelRect = mapSelectionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(520f, 620f);

        // Nền mờ sang trọng cho Popup Chọn Map
        Image panelBg = mapSelectionPanel.GetComponent<Image>();
        panelBg.color = new Color(0.08f, 0.08f, 0.1f, 0.95f);

        // 2. Tạo Tiêu đề "CHỌN BẢN ĐỒ"
        GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(mapSelectionPanel.transform, false);

        RectTransform tRect = titleObj.GetComponent<RectTransform>();
        tRect.anchorMin = new Vector2(0.5f, 1f);
        tRect.anchorMax = new Vector2(0.5f, 1f);
        tRect.pivot = new Vector2(0.5f, 1f);
        tRect.anchoredPosition = new Vector2(0f, -25f);
        tRect.sizeDelta = new Vector2(400f, 60f);

        TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
        titleTMP.text = "CHỌN BẢN ĐỒ";
        titleTMP.fontSize = 38;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
        if (fontAsset != null) titleTMP.font = fontAsset;
        if (fontMat != null) titleTMP.fontSharedMaterial = fontMat;

        // 3. Danh sách 5 Map
        string[] mapNames = new string[] {
            "Map 1 (SampleScene)",
            "Map 2 (Zic-zac)",
            "Map 3 (Spiral Maze)",
            "Map 4 (Large 16x16)",
            "Map 5 (Extreme Boss)"
        };

        string[] sceneNames = new string[] {
            "SampleScene",
            "Map2",
            "Map3",
            "Map4",
            "Map5"
        };

        float startY = -110f;
        float spacing = 75f;

        for (int i = 0; i < 5; i++)
        {
            string sceneToLoad = sceneNames[i];
            string displayName = mapNames[i];

            GameObject mapBtnObj = new GameObject($"Btn_{sceneToLoad}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            mapBtnObj.transform.SetParent(mapSelectionPanel.transform, false);

            RectTransform bRect = mapBtnObj.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0.5f, 1f);
            bRect.anchorMax = new Vector2(0.5f, 1f);
            bRect.pivot = new Vector2(0.5f, 1f);
            bRect.anchoredPosition = new Vector2(0f, startY - (i * spacing));
            bRect.sizeDelta = new Vector2(360f, 60f);

            Image bImg = mapBtnObj.GetComponent<Image>();
            if (btnSprite != null)
            {
                bImg.sprite = btnSprite;
                bImg.type = Image.Type.Simple;
            }
            bImg.color = Color.white;

            Button bComp = mapBtnObj.GetComponent<Button>();
            bComp.onClick = new Button.ButtonClickedEvent();
            bComp.onClick.AddListener(() => {
                Time.timeScale = 1f;
                SceneManager.LoadScene(sceneToLoad);
            });
            bComp.navigation = new Navigation { mode = Navigation.Mode.None };

            // Text hiển thị trên nút map
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(mapBtnObj.transform, false);

            RectTransform txtRect = textObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            TextMeshProUGUI txtTMP = textObj.GetComponent<TextMeshProUGUI>();
            int mapHigh = ScoreManager.GetHighScoreForMap(sceneToLoad);
            txtTMP.text = mapHigh > 0 ? $"{displayName} (Kỷ lục: {mapHigh})" : displayName;
            txtTMP.fontSize = 19;
            txtTMP.fontStyle = FontStyles.Bold;
            txtTMP.alignment = TextAlignmentOptions.Center;
            txtTMP.color = Color.white;
            if (fontAsset != null) txtTMP.font = fontAsset;
            if (fontMat != null) txtTMP.fontSharedMaterial = fontMat;
        }

        // 4. Nút "ĐÓNG" ở dưới cùng của bảng chọn map
        GameObject closeBtnObj = new GameObject("Btn_CloseMapSelection", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeBtnObj.transform.SetParent(mapSelectionPanel.transform, false);

        RectTransform closeRect = closeBtnObj.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.5f, 0f);
        closeRect.anchorMax = new Vector2(0.5f, 0f);
        closeRect.pivot = new Vector2(0.5f, 0f);
        closeRect.anchoredPosition = new Vector2(0f, 25f);
        closeRect.sizeDelta = new Vector2(180f, 50f);

        Image closeImg = closeBtnObj.GetComponent<Image>();
        if (btnSprite != null)
        {
            closeImg.sprite = btnSprite;
        }
        closeImg.color = new Color(0.85f, 0.4f, 0.4f, 1f);

        Button closeBtn = closeBtnObj.GetComponent<Button>();
        closeBtn.onClick = new Button.ButtonClickedEvent();
        closeBtn.onClick.AddListener(() => {
            mapSelectionPanel.SetActive(false);
        });
        closeBtn.navigation = new Navigation { mode = Navigation.Mode.None };

        GameObject closeTextObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        closeTextObj.transform.SetParent(closeBtnObj.transform, false);

        RectTransform cTxtRect = closeTextObj.GetComponent<RectTransform>();
        cTxtRect.anchorMin = Vector2.zero;
        cTxtRect.anchorMax = Vector2.one;
        cTxtRect.offsetMin = Vector2.zero;
        cTxtRect.offsetMax = Vector2.zero;

        TextMeshProUGUI closeTxtTMP = closeTextObj.GetComponent<TextMeshProUGUI>();
        closeTxtTMP.text = "ĐÓNG";
        closeTxtTMP.fontSize = 18;
        closeTxtTMP.fontStyle = FontStyles.Bold;
        closeTxtTMP.alignment = TextAlignmentOptions.Center;
        closeTxtTMP.color = Color.white;
        if (fontAsset != null) closeTxtTMP.font = fontAsset;
        if (fontMat != null) closeTxtTMP.fontSharedMaterial = fontMat;

        mapSelectionPanel.SetActive(true);
        mapSelectionPanel.transform.SetAsLastSibling();
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