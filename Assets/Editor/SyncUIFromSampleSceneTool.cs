using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Events;
using TMPro;

public static class SyncUIFromSampleSceneTool
{
    private const string SOURCE_SCENE_PATH = "Assets/Scenes/SampleScene.unity";
    private static readonly string[] TARGET_SCENES = new string[] {
        "Assets/Scenes/Map2.unity",
        "Assets/Scenes/Map3.unity",
        "Assets/Scenes/Map4.unity",
        "Assets/Scenes/Map5.unity"
    };

    [MenuItem("Tools/Sync In-Game UI To All Maps (From SampleScene)")]
    public static void SyncUIToAllMaps()
    {
        // 1. Mở SampleScene để trích xuất hoặc tạo Prefab tạm thời của cụm "UI" và "PauseCanvas"
        var sampleScene = EditorSceneManager.OpenScene(SOURCE_SCENE_PATH, OpenSceneMode.Single);
        GameObject sourceUIObj = FindGameObjectIncludeInactive(sampleScene, "UI");
        GameObject sourcePauseObj = FindGameObjectIncludeInactive(sampleScene, "PauseCanvas");

        if (sourceUIObj == null)
        {
            Debug.LogError("Không tìm thấy GameObject 'UI' trong SampleScene!");
            return;
        }

        // Tạo thư mục tạm nếu chưa có
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        string tempUIPrefabPath = "Assets/Prefabs/UI_InGame_Template.prefab";
        GameObject savedUIPrefab = PrefabUtility.SaveAsPrefabAsset(sourceUIObj, tempUIPrefabPath);

        GameObject savedPausePrefab = null;
        if (sourcePauseObj != null)
        {
            string tempPausePrefabPath = "Assets/Prefabs/PauseCanvas_Template.prefab";
            savedPausePrefab = PrefabUtility.SaveAsPrefabAsset(sourcePauseObj, tempPausePrefabPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 2. Lần lượt mở từng target scene để đồng bộ
        foreach (string targetScenePath in TARGET_SCENES)
        {
            Scene targetScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);

            // Xóa cụm 'UI' cũ trong scene mục tiêu
            GameObject oldUIObj = FindGameObjectIncludeInactive(targetScene, "UI");
            if (oldUIObj != null)
            {
                Object.DestroyImmediate(oldUIObj);
            }

            // Xóa 'PauseCanvas' cũ trong scene mục tiêu nếu có
            GameObject oldPauseObj = FindGameObjectIncludeInactive(targetScene, "PauseCanvas");
            if (oldPauseObj != null)
            {
                Object.DestroyImmediate(oldPauseObj);
            }

            // A) Khởi tạo cụm UI mới từ prefab mẫu của SampleScene
            GameObject newUIObj = (GameObject)PrefabUtility.InstantiatePrefab(savedUIPrefab, targetScene);
            newUIObj.name = "UI";
            PrefabUtility.UnpackPrefabInstance(newUIObj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            // B) Khởi tạo cụm PauseCanvas mới từ prefab mẫu của SampleScene
            GameObject newPauseObj = null;
            if (savedPausePrefab != null)
            {
                newPauseObj = (GameObject)PrefabUtility.InstantiatePrefab(savedPausePrefab, targetScene);
                newPauseObj.name = "PauseCanvas";
                PrefabUtility.UnpackPrefabInstance(newPauseObj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                newPauseObj.SetActive(true);

                Transform darkOverlay = newPauseObj.transform.Find("DarkOverlay");
                if (darkOverlay != null) darkOverlay.gameObject.SetActive(false);
            }

            // BẬT / TẮT ACTIVE PHÙ HỢP CHO IN-GAME MAPS:
            newUIObj.SetActive(true);

            Transform uiMoneyTrans = newUIObj.transform.Find("UiMoney");
            if (uiMoneyTrans != null)
            {
                uiMoneyTrans.gameObject.SetActive(true);

                Transform upgradePanel = uiMoneyTrans.Find("UpgradeUI_Panel");
                if (upgradePanel != null) upgradePanel.gameObject.SetActive(false);

                Transform soundBtn = uiMoneyTrans.Find("Btn_SoundToggle_InGame");
                if (soundBtn != null) soundBtn.gameObject.SetActive(true);
            }

            Transform uiBuildTrans = newUIObj.transform.Find("UiBuild");
            if (uiBuildTrans != null)
            {
                uiBuildTrans.gameObject.SetActive(true);

                Transform buildPanel = uiBuildTrans.Find("BuildUI_Panel");
                if (buildPanel != null) buildPanel.gameObject.SetActive(false);
            }

            Transform uiGameOverTrans = newUIObj.transform.Find("UIGameOver");
            GameObject gameOverChildPanel = null;
            if (uiGameOverTrans != null)
            {
                uiGameOverTrans.gameObject.SetActive(true);

                // Tìm panel con GameOverPanel chứa nội dung Game Over
                Transform goPanel = uiGameOverTrans.Find("GameOverPanel");
                if (goPanel == null) goPanel = uiGameOverTrans.Find("UIGameOver");
                if (goPanel != null)
                {
                    gameOverChildPanel = goPanel.gameObject;
                    gameOverChildPanel.SetActive(false);
                }
            }

            // KẾT NỐI LẠI THAM CHIẾU VÀ SỰ KIỆN:
            // 1. UIManager
            UIManager uiManager = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
            if (uiManager != null)
            {
                Transform moneyTrans = newUIObj.transform.Find("UiMoney/MoneyText");
                if (moneyTrans != null) uiManager.moneyText = moneyTrans.GetComponent<TextMeshProUGUI>();

                Transform healthTrans = newUIObj.transform.Find("UiMoney/BaseHealthText");
                if (healthTrans != null) uiManager.baseHealthText = healthTrans.GetComponent<TextMeshProUGUI>();

                Transform scoreTrans = newUIObj.transform.Find("UiMoney/ScoreText");
                if (scoreTrans != null) uiManager.scoreText = scoreTrans.GetComponent<TextMeshProUGUI>();

                Transform noticeTrans = newUIObj.transform.Find("UiMoney/EventNotificationText");
                if (noticeTrans != null) uiManager.eventNotificationText = noticeTrans.GetComponent<TextMeshProUGUI>();

                if (gameOverChildPanel != null)
                {
                    uiManager.gameOverPanel = gameOverChildPanel;
                    Transform titleTrans = gameOverChildPanel.transform.Find("TitleText");
                    if (titleTrans != null)
                    {
                        uiManager.titleText = titleTrans.GetComponent<TextMeshProUGUI>();
                        if (uiManager.titleText != null)
                        {
                            uiManager.titleText.margin = Vector4.zero;
                            uiManager.titleText.alignment = TextAlignmentOptions.Center;
                            uiManager.titleText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                            uiManager.titleText.verticalAlignment = VerticalAlignmentOptions.Middle;
                            EditorUtility.SetDirty(uiManager.titleText);
                        }
                    }

                    Transform resultScoreTrans = gameOverChildPanel.transform.Find("ResultScoreText");
                    if (resultScoreTrans != null)
                    {
                        uiManager.resultScoreText = resultScoreTrans.GetComponent<TextMeshProUGUI>();
                    }
                }
                EditorUtility.SetDirty(uiManager);
            }

            // 2. BuildManager
            BuildManager buildManager = Object.FindFirstObjectByType<BuildManager>(FindObjectsInactive.Include);
            if (buildManager != null)
            {
                Transform buildPanelTrans = newUIObj.transform.Find("UiBuild/BuildUI_Panel");
                if (buildPanelTrans != null)
                {
                    buildManager.buildUI = buildPanelTrans.gameObject;

                    Transform cannonBtnTrans = buildPanelTrans.Find("Tháp Cannon");
                    if (cannonBtnTrans != null) buildManager.cannonButtonText = cannonBtnTrans.GetComponentInChildren<TextMeshProUGUI>();

                    Transform missileBtnTrans = buildPanelTrans.Find("Tháp Missile");
                    if (missileBtnTrans != null) buildManager.missileButtonText = missileBtnTrans.GetComponentInChildren<TextMeshProUGUI>();

                    Transform iceBtnTrans = buildPanelTrans.Find("Tháp Ice");
                    if (iceBtnTrans != null) buildManager.iceButtonText = iceBtnTrans.GetComponentInChildren<TextMeshProUGUI>();
                }

                Transform upgradePanelTrans = newUIObj.transform.Find("UiMoney/UpgradeUI_Panel");
                if (upgradePanelTrans != null)
                {
                    buildManager.upgradeUI = upgradePanelTrans.gameObject;

                    Transform upBtnTrans = upgradePanelTrans.Find("Upgrade_Button");
                    if (upBtnTrans != null) buildManager.upgradeButtonText = upBtnTrans.GetComponentInChildren<TextMeshProUGUI>();

                    Transform sellBtnTrans = upgradePanelTrans.Find("Sell_Button");
                    if (sellBtnTrans != null) buildManager.sellButtonText = sellBtnTrans.GetComponentInChildren<TextMeshProUGUI>();
                }

                EditorUtility.SetDirty(buildManager);
            }

            // 3. PauseManager
            PauseManager pauseManager = null;
            if (newPauseObj != null)
            {
                pauseManager = newPauseObj.GetComponent<PauseManager>();
                if (pauseManager == null) pauseManager = newPauseObj.AddComponent<PauseManager>();

                Transform darkOverlay = newPauseObj.transform.Find("DarkOverlay");
                if (darkOverlay != null) pauseManager.pausePanel = darkOverlay.gameObject;
                pauseManager.pauseCanvas = newPauseObj.GetComponent<Canvas>();

                EditorUtility.SetDirty(pauseManager);
            }

            // 4. Kết nối OnClick Persistent Events cho tất cả nút bấm
            ConnectAllButtonEvents(newUIObj, newPauseObj, buildManager, uiManager, pauseManager);

            // 5. MainMenuUIController (nếu map có)
            MainMenuUIController menuController = Object.FindFirstObjectByType<MainMenuUIController>(FindObjectsInactive.Include);
            if (menuController != null)
            {
                Transform uiMoney = newUIObj.transform.Find("UiMoney");
                Transform uiBuild = newUIObj.transform.Find("UiBuild");
                Transform uiGameOver = newUIObj.transform.Find("UIGameOver");

                System.Collections.Generic.List<GameObject> inGameList = new System.Collections.Generic.List<GameObject>();
                if (uiMoney != null) inGameList.Add(uiMoney.gameObject);
                if (uiBuild != null) inGameList.Add(uiBuild.gameObject);
                if (uiGameOver != null) inGameList.Add(uiGameOver.gameObject);
                inGameList.Add(newUIObj);

                menuController.inGameUIElements = inGameList.ToArray();

                Transform soundBtnTrans = newUIObj.transform.Find("UiMoney/Btn_SoundToggle_InGame");
                if (soundBtnTrans != null)
                {
                    menuController.inGameSoundButton = soundBtnTrans.gameObject;
                }

                EditorUtility.SetDirty(menuController);
            }

            EditorUtility.SetDirty(newUIObj);
            if (newPauseObj != null) EditorUtility.SetDirty(newPauseObj);
            EditorSceneManager.MarkSceneDirty(targetScene);
            EditorSceneManager.SaveScene(targetScene);
        }

        // Mở lại SampleScene ban đầu
        EditorSceneManager.OpenScene(SOURCE_SCENE_PATH, OpenSceneMode.Single);
        Debug.Log("<color=green><b>ĐÃ ĐỒNG BỘ TOÀN BỘ CỤM 'UI' VÀ 'PAUSECANVAS' TỪ SAMPLESCENE SANG MAP 2, 3, 4, 5 THÀNH CÔNG!</b></color>");
    }

    private static void ConnectAllButtonEvents(GameObject uiRoot, GameObject pauseRoot, BuildManager buildManager, UIManager uiManager, PauseManager pauseManager)
    {
        // 1. Build & Upgrade Buttons
        if (buildManager != null && uiRoot != null)
        {
            HookButton(uiRoot.transform.Find("UiBuild/BuildUI_Panel/Tháp Cannon"), buildManager, "BuildCannonTower");
            HookButton(uiRoot.transform.Find("UiBuild/BuildUI_Panel/Tháp Missile"), buildManager, "BuildMissileTower");
            HookButton(uiRoot.transform.Find("UiBuild/BuildUI_Panel/Tháp Ice"), buildManager, "BuildIceTower");
            HookButton(uiRoot.transform.Find("UiBuild/BuildUI_Panel/Thoát"), buildManager, "CloseUI");

            HookButton(uiRoot.transform.Find("UiMoney/UpgradeUI_Panel/Upgrade_Button"), buildManager, "UpgradeSelectedTower");
            HookButton(uiRoot.transform.Find("UiMoney/UpgradeUI_Panel/Sell_Button"), buildManager, "SellSelectedTower");
            HookButton(uiRoot.transform.Find("UiMoney/UpgradeUI_Panel/Close_Button"), buildManager, "CloseUI");
        }

        // 2. GameOver Buttons (Hỗ trợ cả tên nút RestartButton / Btn_Restart và MenuButton / Btn_Menu)
        if (uiManager != null && uiRoot != null)
        {
            HookButton(uiRoot.transform.Find("UIGameOver/GameOverPanel/RestartButton"), uiManager, "RestartGame");
            HookButton(uiRoot.transform.Find("UIGameOver/GameOverPanel/MenuButton"), uiManager, "GoToMenu");
            HookButton(uiRoot.transform.Find("UIGameOver/GameOverPanel/Btn_Restart"), uiManager, "RestartGame");
            HookButton(uiRoot.transform.Find("UIGameOver/GameOverPanel/Btn_Menu"), uiManager, "GoToMenu");

            HookButton(uiRoot.transform.Find("UIGameOver/UIGameOver/RestartButton"), uiManager, "RestartGame");
            HookButton(uiRoot.transform.Find("UIGameOver/UIGameOver/MenuButton"), uiManager, "GoToMenu");
            HookButton(uiRoot.transform.Find("UIGameOver/UIGameOver/Btn_Restart"), uiManager, "RestartGame");
            HookButton(uiRoot.transform.Find("UIGameOver/UIGameOver/Btn_Menu"), uiManager, "GoToMenu");
        }

        // 3. Pause Buttons (ResumeGame, BackToMenu)
        if (pauseManager != null && pauseRoot != null)
        {
            HookButton(pauseRoot.transform.Find("DarkOverlay/PauseMenu/Buttons/Btn_Resume"), pauseManager, "ResumeGame");
            HookButton(pauseRoot.transform.Find("DarkOverlay/PauseMenu/Buttons/Btn_BackMenu"), pauseManager, "BackToMenu");

            HookButton(pauseRoot.transform.Find("DarkOverlay/Buttons/Btn_Resume"), pauseManager, "ResumeGame");
            HookButton(pauseRoot.transform.Find("DarkOverlay/Buttons/Btn_BackMenu"), pauseManager, "BackToMenu");
        }
    }

    private static void HookButton(Transform btnTrans, UnityEngine.Object target, string methodName)
    {
        if (btnTrans == null || target == null) return;
        Button btn = btnTrans.GetComponent<Button>();
        if (btn == null) return;

        // Xóa các event cũ trỏ tới target bị null
        for (int i = btn.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(btn.onClick, i);
        }

        // Gắn listener mới
        System.Reflection.MethodInfo method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, System.Type.EmptyTypes, null);
        if (method != null)
        {
            UnityEngine.Events.UnityAction action = (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, method);
            UnityEventTools.AddPersistentListener(btn.onClick, action);
            EditorUtility.SetDirty(btn);
        }
    }

    private static GameObject FindGameObjectIncludeInactive(Scene scene, string name)
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
