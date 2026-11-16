using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class MenuUISetupTool
{
    private const string BG_SPRITE_PATH = "Assets/UI_Sprites/ImageBackground.png";
    private const string SPEAKER_SPRITE_PATH = "Assets/UI_Sprites/Speaker.png";
    private const string MENU_MUSIC_PATH = "Assets/Pirate Music Pack/mp3/Pirate 1.mp3";
    private const string GAME_MUSIC_PATH = "Assets/Pirate Music Pack/mp3/Pirate 2.mp3";

    [MenuItem("Tools/Setup Menu & Loading UI (Base Breaker)")]
    public static void SetupMenuAndLoadingUI()
    {
        // 1. TÌM VÀ DỌN DẸP CÁC PHẦN TỬ MENU CŨ (Kể cả bị lọt vào trong UiMoney hay nơi khác)
        var oldObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in oldObjects)
        {
            if (obj == null || !obj.scene.IsValid()) continue;
            if (obj.name == "UI_MainMenu" || obj.name == "UI_LoadingGame" || obj.name == "MainMenuController" || obj.name == "MenuCanvas")
            {
                Object.DestroyImmediate(obj);
            }
        }

        // 2. Nạp Sprites và AudioClips
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BG_SPRITE_PATH);
        Sprite speakerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SPEAKER_SPRITE_PATH);
        AudioClip menuMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(MENU_MUSIC_PATH);
        AudioClip gameMusic = AssetDatabase.LoadAssetAtPath<AudioClip>(GAME_MUSIC_PATH);

        // 3. Thiết lập AudioManager
        GameObject audioMgrObj = GameObject.Find("AudioManager");
        if (audioMgrObj == null)
        {
            audioMgrObj = new GameObject("AudioManager");
            Undo.RegisterCreatedObjectUndo(audioMgrObj, "Create AudioManager");
        }
        AudioManager audioManager = audioMgrObj.GetComponent<AudioManager>();
        if (audioManager == null) audioManager = audioMgrObj.AddComponent<AudioManager>();
        audioManager.menuMusic = menuMusic;
        audioManager.gameplayMusic = gameMusic;
        audioManager.musicVolume = 0.5f;
        EditorUtility.SetDirty(audioManager);

        // 4. TẠO RIÊNG 1 ROOT CANVAS ĐỘC LẬP: "MenuCanvas" VỚI SORTING ORDER CAO NHẤT (100)
        GameObject menuCanvasObj = new GameObject("MenuCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas menuCanvas = menuCanvasObj.GetComponent<Canvas>();
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        menuCanvas.sortingOrder = 100; // CAO NHẤT: ĐÈ LÊN MỌI CANVAS KHÁC TRONG GAME!

        CanvasScaler scaler = menuCanvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // 5. Tạo MainMenuController
        MainMenuUIController menuController = menuCanvasObj.AddComponent<MainMenuUIController>();
        menuController.menuCanvas = menuCanvas;

        // Tìm các UI trong game (UiMoney, UiBuild, UIGameOver,...) để tự động ẩn khi ở Menu
        List<GameObject> inGameList = new List<GameObject>();
        GameObject uiMoney = GameObject.Find("UiMoney");
        if (uiMoney != null) inGameList.Add(uiMoney);
        GameObject uiBuild = GameObject.Find("UiBuild");
        if (uiBuild != null) inGameList.Add(uiBuild);
        GameObject uiGameOver = GameObject.Find("UIGameOver");
        if (uiGameOver != null) inGameList.Add(uiGameOver);
        GameObject uiRoot = GameObject.Find("UI");
        if (uiRoot != null && !inGameList.Contains(uiRoot)) inGameList.Add(uiRoot);

        menuController.inGameUIElements = inGameList.ToArray();

        // ==========================================
        // 6. TẠO GIAO DIỆN UI MAIN MENU (NẰM TRONG MenuCanvas)
        // ==========================================
        GameObject menuPanel = CreateFullPanel("UI_MainMenu", menuCanvasObj.transform, bgSprite);
        menuController.menuPanel = menuPanel;

        CreateTitleText(menuPanel.transform, "Base Breaker", 90f);

        // Nút Play
        GameObject playBtnObj = CreateButton("Btn_Play", menuPanel.transform, new Vector2(0f, -40f), new Vector2(160f, 45f), "Play", new Color(0.78f, 0.78f, 0.78f));
        Button playBtn = playBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(playBtn.onClick, menuController.OnPlayButtonClicked);

        // Nút Quit
        GameObject quitBtnObj = CreateButton("Btn_Quit", menuPanel.transform, new Vector2(0f, -100f), new Vector2(160f, 45f), "Quit", new Color(0.78f, 0.78f, 0.78f));
        Button quitBtn = quitBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(quitBtn.onClick, menuController.OnQuitButtonClicked);

        // Nút Speaker ở Menu
        // Khong tao nut Speaker o Menu vi user chi muon Speaker o trong game

        // ==========================================
        // 7. TẠO GIAO DIỆN UI LOADING GAME (NẰM TRONG MenuCanvas)
        // ==========================================
        GameObject loadingPanel = CreateFullPanel("UI_LoadingGame", menuCanvasObj.transform, bgSprite);
        menuController.loadingPanel = loadingPanel;

        CreateTitleText(loadingPanel.transform, "Base Breaker", 90f);

        GameObject sliderObj = CreateProgressBar(loadingPanel.transform, new Vector2(0f, -160f), new Vector2(480f, 40f));
        Slider slider = sliderObj.GetComponent<Slider>();
        menuController.loadingSlider = slider;

        TextMeshProUGUI loadTmp = sliderObj.GetComponentInChildren<TextMeshProUGUI>();
        menuController.loadingText = loadTmp;

        // ==========================================
        // 8. TẠO NÚT SPEAKER IN-GAME (GẮN VÀO UiMoney HOẶC Canvas In-game)
        // ==========================================
        Transform parentForInGameSound = uiMoney != null ? uiMoney.transform : (uiRoot != null ? uiRoot.transform : null);
        if (parentForInGameSound != null)
        {
            // Xóa nút sound in-game cũ nếu có
            Transform oldInGame = parentForInGameSound.Find("Btn_SoundToggle_InGame");
            if (oldInGame != null) Object.DestroyImmediate(oldInGame.gameObject);

            GameObject inGameSoundBtn = CreateSoundToggleButton("Btn_SoundToggle_InGame", parentForInGameSound, new Vector2(-40f, -40f), new Vector2(1f, 1f), speakerSprite);
            menuController.inGameSoundButton = inGameSoundBtn;
            inGameSoundBtn.SetActive(false);
        }

        // ==========================================
        // 9. THIẾT LẬP TRẠNG THÁI BAN ĐẦU
        // ==========================================
        menuPanel.SetActive(true);
        loadingPanel.SetActive(false);

        EditorUtility.SetDirty(menuController);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("<color=green><b>ĐÃ TÁCH RIÊNG MenuCanvas ĐỘC LẬP: Menu luôn nằm trên cùng 100%, ẩn hoàn toàn UiMoney khi ở Menu!</b></color>");
    }

    private static GameObject CreateFullPanel(string name, Transform parent, Sprite bgSprite)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        Image img = panel.GetComponent<Image>();
        if (bgSprite != null)
        {
            img.sprite = bgSprite;
            img.color = Color.white;
            img.type = Image.Type.Simple;
            img.preserveAspect = false;
        }
        else
        {
            img.color = new Color(0.95f, 0.95f, 0.95f, 1f);
        }

        return panel;
    }

    private static void CreateTitleText(Transform parent, string title, float posY)
    {
        GameObject textObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(parent, false);

        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, posY);
        rt.sizeDelta = new Vector2(600f, 100f);

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = title;
        tmp.fontSize = 54;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.12f, 0.12f, 0.12f, 1f);
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 pos, Vector2 size, string text, Color btnColor)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = btnObj.GetComponent<Image>();
        img.color = btnColor;

        GameObject textObj = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        return btnObj;
    }

    private static GameObject CreateProgressBar(Transform parent, Vector2 pos, Vector2 size)
    {
        GameObject sliderObj = new GameObject("LoadingBar_Slider", typeof(RectTransform), typeof(Slider));
        sliderObj.transform.SetParent(parent, false);

        RectTransform sliderRt = sliderObj.GetComponent<RectTransform>();
        sliderRt.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRt.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRt.anchoredPosition = pos;
        sliderRt.sizeDelta = size;

        Slider slider = sliderObj.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.interactable = false;

        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObj.transform.SetParent(sliderObj.transform, false);

        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;

        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.color = new Color(0.85f, 0.85f, 0.85f, 0.9f);

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);

        RectTransform fillAreaRt = fillArea.GetComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillObj.transform.SetParent(fillArea.transform, false);

        RectTransform fillRt = fillObj.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.sizeDelta = Vector2.zero;

        Image fillImg = fillObj.GetComponent<Image>();
        fillImg.color = new Color(0.3f, 0.85f, 0.45f, 1f);

        slider.targetGraphic = fillImg;
        slider.fillRect = fillRt;

        GameObject textObj = new GameObject("LoadingText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(sliderObj.transform, false);

        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "Loading... 0%";
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.1f, 0.1f, 0.1f, 1f);

        return sliderObj;
    }

    public static GameObject CreateSoundToggleButton(string name, Transform parent, Vector2 pos, Vector2 anchor, Sprite speakerSprite)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(SoundToggleButton));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(50f, 50f);

        Image btnBg = btnObj.GetComponent<Image>();
        btnBg.color = new Color(0.15f, 0.15f, 0.15f, 0.6f);

        GameObject iconObj = new GameObject("SpeakerIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObj.transform.SetParent(btnObj.transform, false);

        RectTransform iconRt = iconObj.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 0.5f);
        iconRt.anchorMax = new Vector2(0.5f, 0.5f);
        iconRt.anchoredPosition = Vector2.zero;
        iconRt.sizeDelta = new Vector2(34f, 34f);

        Image iconImg = iconObj.GetComponent<Image>();
        if (speakerSprite != null)
        {
            iconImg.sprite = speakerSprite;
            iconImg.color = Color.white;
            iconImg.preserveAspect = true;
        }

        GameObject crossLine = new GameObject("CrossLine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        crossLine.transform.SetParent(btnObj.transform, false);

        RectTransform lineRt = crossLine.GetComponent<RectTransform>();
        lineRt.anchorMin = new Vector2(0.5f, 0.5f);
        lineRt.anchorMax = new Vector2(0.5f, 0.5f);
        lineRt.anchoredPosition = Vector2.zero;
        lineRt.sizeDelta = new Vector2(40f, 4f);
        lineRt.localRotation = Quaternion.Euler(0f, 0f, -45f);

        Image lineImg = crossLine.GetComponent<Image>();
        lineImg.color = new Color(0.95f, 0.2f, 0.2f, 1f);

        crossLine.SetActive(false);

        SoundToggleButton soundBtn = btnObj.GetComponent<SoundToggleButton>();
        soundBtn.button = btnObj.GetComponent<Button>();
        soundBtn.speakerIcon = iconImg;
        soundBtn.crossLine = crossLine;

        return btnObj;
    }
}