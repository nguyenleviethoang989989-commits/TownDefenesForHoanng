using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class SpawnPointModelSetup
{
    private static readonly string[] ALL_SCENES = {
        "Assets/Scenes/SampleScene.unity",
        "Assets/Scenes/Map2.unity",
        "Assets/Scenes/Map3.unity",
        "Assets/Scenes/Map4.unity",
        "Assets/Scenes/Map5.unity"
    };

    private const string START_MODEL_PATH = "Assets/Models/Tower Defense Kit/tower-round-build-a.fbx";
    private const string END_MODEL_PATH = "Assets/Models/Tower Defense Kit/tower-square-build-f.fbx";
    private const string MAIN_COLOR_MAT_PATH = "Assets/Materials/MainColor.mat";

    [MenuItem("Tools/Setup Spawn Points Model (5 Maps)")]
    public static void SetupModelsForAllMaps()
    {
        string currentScenePath = SceneManager.GetActiveScene().path;

        GameObject startModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(START_MODEL_PATH);
        GameObject endModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(END_MODEL_PATH);
        Material mainColorMat = AssetDatabase.LoadAssetAtPath<Material>(MAIN_COLOR_MAT_PATH);

        if (startModelPrefab == null || endModelPrefab == null)
        {
            Debug.LogError("Không tìm thấy một trong hai file model FBX!");
            return;
        }

        if (mainColorMat == null)
        {
            Debug.LogWarning("Không tìm thấy Material tại: " + MAIN_COLOR_MAT_PATH);
        }

        int updatedSceneCount = 0;

        foreach (string scenePath in ALL_SCENES)
        {
            if (!System.IO.File.Exists(scenePath)) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            GridManager gridManager = Object.FindFirstObjectByType<GridManager>();
            float maxX = 9f;
            float maxZ = 9f;
            if (gridManager != null)
            {
                maxX = (gridManager.width - 1) * gridManager.cellSize;
                maxZ = (gridManager.height - 1) * gridManager.cellSize;
            }

            // 1. Căn chỉnh vị trí & Gắn model cho spawnstart
            GameObject startObj = GameObject.Find("spawnstart");
            if (startObj != null)
            {
                // Đảm bảo spawnstart nằm gọn hoàn toàn bên trong mép bản đồ [0.5, maxX - 0.5]
                Vector3 sPos = startObj.transform.position;
                sPos.x = Mathf.Clamp(sPos.x, 0.5f, maxX - 0.5f);
                sPos.z = Mathf.Clamp(sPos.z, 0.5f, maxZ - 0.5f);
                startObj.transform.position = sPos;

                AttachOrUpdateModel(startObj, startModelPrefab, "Visual_SpawnStartModel", mainColorMat);
                EditorUtility.SetDirty(startObj);
            }

            // 2. Căn chỉnh vị trí & Gắn model cho spawnend
            GameObject endObj = GameObject.Find("spawnend");
            if (endObj != null)
            {
                // Đảm bảo spawnend nằm gọn hoàn toàn bên trong mép bản đồ [0.5, maxX - 0.5]
                // Khắc phục lỗi ở Map 1 bị đặt x = 9.94 khiến model bị tràn ra ngoài
                Vector3 ePos = endObj.transform.position;
                ePos.x = Mathf.Clamp(ePos.x, 0.5f, maxX - 0.5f);
                ePos.z = Mathf.Clamp(ePos.z, 0.5f, maxZ - 0.5f);
                endObj.transform.position = ePos;

                AttachOrUpdateModel(endObj, endModelPrefab, "Visual_SpawnEndModel", mainColorMat);
                EditorUtility.SetDirty(endObj);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            updatedSceneCount++;
        }

        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        Debug.Log($"<color=green><b>[SpawnPointModelSetup]</b> Đã gắn thành công model kèm Material MainColor và căn chỉnh nằm gọn trong map cho cả {updatedSceneCount} Scene!</color>");
        EditorUtility.DisplayDialog("Thành công", $"Đã gắn Material MainColor và đưa các model nằm hoàn toàn bên trong Map cho cả {updatedSceneCount} Map!", "OK");
    }

    private static void AttachOrUpdateModel(GameObject parentObj, GameObject modelPrefab, string childName, Material matToApply)
    {
        for (int i = parentObj.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentObj.transform.GetChild(i);
            if (child.name.StartsWith("Visual_") || child.name.Contains("tower-"))
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }

        GameObject visualInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        visualInstance.name = childName;
        visualInstance.transform.SetParent(parentObj.transform, false);

        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;
        visualInstance.transform.localScale = Vector3.one;

        // Vô hiệu hóa Collider trên model con
        Collider[] colliders = visualInstance.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // GẮN MATERIAL MAINCOLOR CHO TẤT CẢ RENDERER CỦA MODEL
        if (matToApply != null)
        {
            Renderer[] renderers = visualInstance.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in renderers)
            {
                Material[] mats = new Material[rend.sharedMaterials.Length];
                for (int m = 0; m < mats.Length; m++)
                {
                    mats[m] = matToApply;
                }
                rend.sharedMaterials = mats;
                EditorUtility.SetDirty(rend);
            }
        }

        EditorUtility.SetDirty(visualInstance);
    }
}