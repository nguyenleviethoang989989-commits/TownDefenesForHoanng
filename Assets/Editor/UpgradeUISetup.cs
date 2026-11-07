using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

public static class UpgradeUISetup
{
    [MenuItem("Tools/Setup Upgrade UI")]
    public static void SetupUI()
    {
        // 1. Tìm hoặc mở SampleScene
        var activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "SampleScene")
        {
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        }

        // 2. Tìm Canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Không tìm thấy Canvas trong Scene!");
            return;
        }

        // 3. Tìm BuildManager
        BuildManager buildManager = Object.FindFirstObjectByType<BuildManager>();
        if (buildManager == null)
        {
            Debug.LogError("Không tìm thấy BuildManager trong Scene!");
            return;
        }

        // 4. Kiểm tra xem UpgradeUI_Panel đã tồn tại chưa, nếu có thì xóa đi làm mới
        Transform existingPanel = canvas.transform.Find("UpgradeUI_Panel");
        if (existingPanel != null)
        {
            Object.DestroyImmediate(existingPanel.gameObject);
        }

        // 5. Tạo UpgradeUI_Panel
        GameObject panelObj = new GameObject("UpgradeUI_Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObj.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        // Bottom-Center Anchor
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0f, 90f);
        panelRect.sizeDelta = new Vector2(340f, 100f);

        Image panelImg = panelObj.GetComponent<Image>();
        panelImg.color = new Color(0.12f, 0.12f, 0.12f, 0.85f); // #1E1E1ECC

        // 6. Tạo Nút Nâng Cấp
        GameObject upgradeBtnObj = CreateButton("Upgrade_Button", panelObj.transform, new Vector2(-80f, 0f), new Vector2(140f, 50f), new Color(0.15f, 0.68f, 0.38f), "NÂNG CẤP");
        Button upgradeBtn = upgradeBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(upgradeBtn.onClick, buildManager.UpgradeSelectedTower);

        // 7. Tạo Nút Bán
        GameObject sellBtnObj = CreateButton("Sell_Button", panelObj.transform, new Vector2(80f, 0f), new Vector2(140f, 50f), new Color(0.91f, 0.30f, 0.24f), "BÁN");
        Button sellBtn = sellBtnObj.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(sellBtn.onClick, buildManager.SellSelectedTower);

        // 8. Gán vào BuildManager
        buildManager.upgradeUI = panelObj;
        EditorUtility.SetDirty(buildManager);

        // Mặc định ẩn UI khi bắt đầu hoặc trong Editor
        panelObj.SetActive(false);

        // Lưu scene
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("<color=green>ĐÃ THIẾT LẬP THÀNH CÔNG UpgradeUI_Panel, CÁC NÚT VÀ SỰ KIỆN ONCLICK VÀO SCENE!</color>");
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 pos, Vector2 size, Color btnColor, string textContent)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = size;

        Image btnImg = btnObj.GetComponent<Image>();
        btnImg.color = btnColor;

        // Tạo Text TMP
        GameObject textObj = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = textContent;
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return btnObj;
    }
}