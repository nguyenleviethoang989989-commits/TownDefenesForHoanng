using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

public static class PauseUISetupTool
{
    [MenuItem("Tools/Setup Pause Menu UI (Base Breaker)")]
    public static void SetupPauseMenuUI()
    {
        // 1. Dọn dẹp PauseCanvas cũ nếu có
        GameObject oldCanvasObj = GameObject.Find("PauseCanvas");
        if (oldCanvasObj != null) Object.DestroyImmediate(oldCanvasObj);

        GameObject oldMgrObj = GameObject.Find("PauseManager");
        if (oldMgrObj != null) Object.DestroyImmediate(oldMgrObj);

        // 2. Tạo riêng 1 Canvas độc lập cho Pause Menu với Sorting Order = 95
        GameObject pauseCanvasObj = new GameObject("PauseCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas pauseCanvas = pauseCanvasObj.GetComponent<Canvas>();
        pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pauseCanvas.sortingOrder = 95; // Cao hơn toàn bộ UI trong game (0, 50) và dưới Menu (100)

        CanvasScaler scaler = pauseCanvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // 3. Tạo PauseManager
        PauseManager pauseMgr = pauseCanvasObj.AddComponent<PauseManager>();
        pauseMgr.pauseCanvas = pauseCanvas;

        // 4. Tạo Overlay làm mờ nền đen mờ
        GameObject overlayObj = new GameObject("DarkOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlayObj.transform.SetParent(pauseCanvasObj.transform, false);

        RectTransform overlayRt = overlayObj.GetComponent<RectTransform>();
        overlayRt.anchorMin = Vector2.zero;
        overlayRt.anchorMax = Vector2.one;
        overlayRt.sizeDelta = Vector2.zero;

        Image overlayImg = overlayObj.GetComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.6f); // Đen mờ 60%

        // 5. Tạo Khung Pause Card chính (theo mẫu ảnh: hộp chữ nhật bo góc màu xám nhạt viền đen)
        GameObject cardObj = new GameObject("PauseCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cardObj.transform.SetParent(overlayObj.transform, false);

        RectTransform cardRt = cardObj.GetComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.pivot = new Vector2(0.5f, 0.5f);
        cardRt.anchoredPosition = Vector2.zero;
        cardRt.sizeDelta = new Vector2(480f, 420f); // Tỉ lệ chuẩn như ảnh

        Image cardImg = cardObj.GetComponent<Image>();
        cardImg.color = new Color(0.85f, 0.85f, 0.85f, 1f); // Màu xám nhạt như hình mẫu

        // 6. Tiêu đề "Base Breaker"
        GameObject titleObj = new GameObject("TitleText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        titleObj.transform.SetParent(cardObj.transform, false);

        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.anchoredPosition = new Vector2(0f, 125f);
        titleRt.sizeDelta = new Vector2(440f, 60f);

        TextMeshProUGUI titleTmp = titleObj.GetComponent<TextMeshProUGUI>();
        titleTmp.text = "Base Breaker";
        titleTmp.fontSize = 38;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = new Color(0.12f, 0.12f, 0.12f, 1f); // Đen đậm

        // 7. Tạo Khung Trắng chứa 2 nút (Nền trắng bo góc như trong ảnh mẫu)
        GameObject buttonsBox = new GameObject("ButtonsBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        buttonsBox.transform.SetParent(cardObj.transform, false);

        RectTransform boxRt = buttonsBox.GetComponent<RectTransform>();
        boxRt.anchorMin = new Vector2(0.5f, 0.5f);
        boxRt.anchorMax = new Vector2(0.5f, 0.5f);
        boxRt.anchoredPosition = new Vector2(0f, -40f);
        boxRt.sizeDelta = new Vector2(340f, 190f);

        Image boxImg = buttonsBox.GetComponent<Image>();
        boxImg.color = Color.white; // Màu trắng tinh khôi

        // Đường kẻ phân cách giữa 2 nút (Divider Line)
        GameObject divider = new GameObject("DividerLine", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        divider.transform.SetParent(buttonsBox.transform, false);

        RectTransform divRt = divider.GetComponent<RectTransform>();
        divRt.anchorMin = new Vector2(0.5f, 0.5f);
        divRt.anchorMax = new Vector2(0.5f, 0.5f);
        divRt.anchoredPosition = Vector2.zero;
        divRt.sizeDelta = new Vector2(340f, 2f);

        Image divImg = divider.GetComponent<Image>();
        divImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);

        // 8. Tạo Nút "Resume"
        GameObject resumeBtnObj = CreateButton("Btn_Resume", buttonsBox.transform, new Vector2(0f, 47f), new Vector2(340f, 92f), "Resume");
        Button resumeBtn = resumeBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(resumeBtn.onClick, pauseMgr.ResumeGame);

        // 9. Tạo Nút "Back Menu"
        GameObject backMenuBtnObj = CreateButton("Btn_BackMenu", buttonsBox.transform, new Vector2(0f, -47f), new Vector2(340f, 92f), "Back Menu");
        Button backMenuBtn = backMenuBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(backMenuBtn.onClick, pauseMgr.BackToMenu);

        // 10. Gán liên kết và thiết lập trạng thái ban đầu
        pauseMgr.pausePanel = overlayObj;
        overlayObj.SetActive(false); // Mặc định ẩn khi mới vào game

        EditorUtility.SetDirty(pauseMgr);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("<color=green><b>ĐÃ THIẾT LẬP THÀNH CÔNG UI PAUSE GAME (BASE BREAKER)!</b></color>");
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 pos, Vector2 size, string text)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0f); // Trong suốt để lấy nền trắng của ButtonsBox

        // Hiệu ứng hover/nhấn nút
        Button btn = btnObj.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 0.01f);
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        btn.colors = cb;

        // Text bên trong nút
        GameObject textObj = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.12f, 0.12f, 0.12f, 1f);

        return btnObj;
    }
}