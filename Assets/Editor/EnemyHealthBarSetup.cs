using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public static class EnemyHealthBarSetup
{
    [MenuItem("TowerDefense/Setup Enemy Health Bars")]
    public static void SetupSelectedEnemyHealthBars()
    {
        // Lấy danh sách các đối tượng hoặc Prefab đang được chọn trong Project window
        Object[] selectedObjects = Selection.objects;
        int processedCount = 0;

        // Nếu người dùng không chọn gì, tự động tìm và cài đặt cho tất cả Prefab trong Assets/Prefabs/Enemy
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Enemy" });
            ListSelectedAndProcess(guids, ref processedCount);
        }
        else
        {
            foreach (Object obj in selectedObjects)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".prefab"))
                {
                    // Nếu là GameObject trên Scene
                    if (obj is GameObject sceneGo && sceneGo.GetComponent<EnemyMovement>() != null)
                    {
                        SetupHealthBarOnGameObject(sceneGo);
                        EditorUtility.SetDirty(sceneGo);
                        processedCount++;
                    }
                    continue;
                }

                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
                if (prefabRoot != null)
                {
                    SetupHealthBarOnGameObject(prefabRoot);
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                    processedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (processedCount > 0)
        {
            Debug.Log($"<color=green><b>[EnemyHealthBarSetup]</b> Đã thiết lập thành công Thanh Máu (Health Bar) cho {processedCount} Prefab Enemy!</color>");
            EditorUtility.DisplayDialog("Thành công", $"Đã thiết lập thành công Thanh Máu (Health Bar) cho {processedCount} Enemy!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Thông báo", "Vui lòng chọn ít nhất một Prefab Enemy trong thư mục Project (hoặc đảm bảo có Prefab trong Assets/Prefabs/Enemy).", "OK");
        }
    }

    private static void ListSelectedAndProcess(string[] guids, ref int processedCount)
    {
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            if (prefabRoot != null)
            {
                SetupHealthBarOnGameObject(prefabRoot);
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                processedCount++;
            }
        }
    }

    public static void SetupHealthBarOnGameObject(GameObject enemyRoot)
    {
        // 1. Kiểm tra xem đã có HealthBar_Canvas cũ chưa, nếu có thì xóa đi làm mới
        Transform existingCanvas = enemyRoot.transform.Find("HealthBar_Canvas");
        if (existingCanvas != null)
        {
            Object.DestroyImmediate(existingCanvas.gameObject);
        }

        // 2. Tạo Canvas ở chế độ World Space làm con của Prefab
        GameObject canvasObj = new GameObject("HealthBar_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObj.transform.SetParent(enemyRoot.transform, false);

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        RectTransform canvasRt = canvasObj.GetComponent<RectTransform>();
        canvasRt.sizeDelta = new Vector2(1f, 0.2f);        // Kích thước nhỏ gọn theo yêu cầu
        canvasRt.localPosition = new Vector3(0f, 2f, 0f);  // Y offset = 2
        canvasRt.localRotation = Quaternion.identity;
        canvasRt.localScale = Vector3.one;

        // 3. Tạo Image 'Background' (Màu xám/đen)
        GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgObj.transform.SetParent(canvasObj.transform, false);

        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        bgRt.anchoredPosition = Vector2.zero;

        Image bgImg = bgObj.GetComponent<Image>();
        bgImg.color = new Color(0.12f, 0.12f, 0.12f, 0.85f); // Đen xám đậm
        bgImg.sprite = GetDefaultWhiteSprite();

        // 4. Tạo Image 'Fill' (Màu đỏ/xanh lá)
        GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillObj.transform.SetParent(canvasObj.transform, false);

        RectTransform fillRt = fillObj.GetComponent<RectTransform>();
        // Co nhẹ lề 0.03 để lộ viền background đẹp mắt
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = new Vector2(0.02f, 0.02f);
        fillRt.offsetMax = new Vector2(-0.02f, -0.02f);

        Image fillImg = fillObj.GetComponent<Image>();
        fillImg.sprite = GetDefaultWhiteSprite();
        fillImg.color = new Color(0.9f, 0.2f, 0.2f, 1f); // Màu đỏ tươi nổi bật
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 1f;

        // 5. Gắn script EnemyHealthUI vào Canvas vừa tạo
        EnemyHealthUI healthUI = canvasObj.AddComponent<EnemyHealthUI>();
        healthUI.fillImage = fillImg;
        healthUI.useBillboarding = true;

        // 6. Gán liên kết vào EnemyMovement nếu có
        EnemyMovement enemyMovement = enemyRoot.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.healthUI = healthUI;
            EditorUtility.SetDirty(enemyMovement);
        }

        EditorUtility.SetDirty(canvasObj);
    }

    private static Sprite GetDefaultWhiteSprite()
    {
        // Nạp sprite chuẩn của Unity UI hoặc tạo texture trắng 1x1
        Sprite sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        if (sprite == null)
        {
            sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }
        return sprite;
    }
}