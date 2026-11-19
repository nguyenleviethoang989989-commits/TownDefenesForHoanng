using UnityEngine;
using UnityEditor;
using TMPro;

public static class BaseHealthUISetupTool
{
    [MenuItem("Tools/Setup Base Health UI (5 Maps)")]
    public static void SetupBaseHealthUIForAllMaps()
    {
        string[] scenes = new string[] {
            "Assets/Scenes/SampleScene.unity",
            "Assets/Scenes/Map2.unity",
            "Assets/Scenes/Map3.unity",
            "Assets/Scenes/Map4.unity",
            "Assets/Scenes/Map5.unity"
        };

        foreach (string scenePath in scenes)
        {
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);

            // Tìm UiMoney (kể cả khi đang bị SetActive(false))
            GameObject uiMoneyObj = FindGameObjectIncludeInactive(scene, "UiMoney");
            if (uiMoneyObj == null)
            {
                Debug.LogWarning("Không tìm thấy UiMoney trong scene: " + scenePath);
                continue;
            }

            // Tìm UIManager (kể cả inactive)
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);

            // Tìm hoặc tạo GameObject BaseHealthText là con trực tiếp của UiMoney
            Transform existingHealth = uiMoneyObj.transform.Find("BaseHealthText");
            GameObject healthObj;
            if (existingHealth != null)
            {
                healthObj = existingHealth.gameObject;
            }
            else
            {
                healthObj = new GameObject("BaseHealthText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                healthObj.transform.SetParent(uiMoneyObj.transform, false);
            }

            healthObj.layer = 5; // UI Layer

            // Cấu hình RectTransform
            RectTransform rect = healthObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(125f, -100f); // Nằm ngay dưới Vàng (y = -55)
            rect.sizeDelta = new Vector2(250f, 50f);

            // Cấu hình TextMeshProUGUI
            TextMeshProUGUI tmp = healthObj.GetComponent<TextMeshProUGUI>();
            tmp.text = "Máu: 10/10";
            tmp.fontSize = 36;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = new Color(0.95f, 0.2f, 0.2f, 1f); // Màu đỏ nổi bật
            tmp.alignment = TextAlignmentOptions.Left;

            // Kế thừa font từ MoneyText nếu có
            Transform moneyTextTrans = uiMoneyObj.transform.Find("MoneyText");
            if (moneyTextTrans != null)
            {
                TextMeshProUGUI moneyTmp = moneyTextTrans.GetComponent<TextMeshProUGUI>();
                if (moneyTmp != null)
                {
                    tmp.font = moneyTmp.font;
                    tmp.fontSharedMaterial = moneyTmp.fontSharedMaterial;
                }
            }

            // Gán vào UIManager
            if (uiManager != null)
            {
                uiManager.baseHealthText = tmp;
                EditorUtility.SetDirty(uiManager);
            }

            // Tạo thêm UI EventNotificationText nằm dưới BaseHealthText
            Transform existingNotice = uiMoneyObj.transform.Find("EventNotificationText");
            GameObject noticeObj;
            if (existingNotice != null)
            {
                noticeObj = existingNotice.gameObject;
            }
            else
            {
                noticeObj = new GameObject("EventNotificationText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                noticeObj.transform.SetParent(uiMoneyObj.transform, false);
            }

            noticeObj.layer = 5;
            RectTransform noticeRect = noticeObj.GetComponent<RectTransform>();
            noticeRect.anchorMin = new Vector2(0f, 1f);
            noticeRect.anchorMax = new Vector2(0f, 1f);
            noticeRect.pivot = new Vector2(0.5f, 0.5f);
            noticeRect.anchoredPosition = new Vector2(200f, -145f); // Nằm ngay dưới Máu (y = -100)
            noticeRect.sizeDelta = new Vector2(400f, 50f);

            TextMeshProUGUI noticeTmp = noticeObj.GetComponent<TextMeshProUGUI>();
            noticeTmp.text = "";
            noticeTmp.fontSize = 28;
            noticeTmp.fontStyle = FontStyles.Bold;
            noticeTmp.alignment = TextAlignmentOptions.Left;
            if (moneyTextTrans != null)
            {
                TextMeshProUGUI moneyTmp = moneyTextTrans.GetComponent<TextMeshProUGUI>();
                if (moneyTmp != null)
                {
                    noticeTmp.font = moneyTmp.font;
                    noticeTmp.fontSharedMaterial = moneyTmp.fontSharedMaterial;
                }
            }

            if (uiManager != null)
            {
                uiManager.eventNotificationText = noticeTmp;
                EditorUtility.SetDirty(uiManager);
            }

            EditorUtility.SetDirty(noticeObj);
            EditorUtility.SetDirty(healthObj);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        }

        // Mở lại SampleScene
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
        Debug.Log("<color=green><b>ĐÃ TẠO BaseHealthText LÀM UI CON CỦA UiMoney TRÊN TOÀN BỘ 5 MAPS THÀNH CÔNG!</b></color>");
    }

    private static GameObject FindGameObjectIncludeInactive(UnityEngine.SceneManagement.Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name) return root;
            Transform[] allChildren = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == name) return t.gameObject;
            }
        }
        return null;
    }
}
