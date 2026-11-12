using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class EnvironmentSetupTool
{
    [MenuItem("Tools/Setup & Generate Environment (RPGPP_LT)")]
    public static void SetupAndGenerate()
    {
        // 1. Tìm hoặc tạo GameObject EnvironmentManager
        GameObject envObj = GameObject.Find("EnvironmentManager");
        if (envObj == null)
        {
            envObj = new GameObject("EnvironmentManager");
            Undo.RegisterCreatedObjectUndo(envObj, "Create EnvironmentManager");
        }

        EnvironmentSpawner spawner = envObj.GetComponent<EnvironmentSpawner>();
        if (spawner == null)
        {
            spawner = Undo.AddComponent<EnvironmentSpawner>(envObj);
        }

        // 2. Tìm các thành phần cần thiết trong Scene
        spawner.gridManager = Object.FindFirstObjectByType<GridManager>();
        spawner.pathfinding = Object.FindFirstObjectByType<Pathfinding>();

        GameObject startObj = GameObject.Find("spawnstart");
        if (startObj != null) spawner.spawnPoint = startObj.transform;

        GameObject endObj = GameObject.Find("spawnend");
        if (endObj != null) spawner.basePoint = endObj.transform;

        // 3. Tự động nạp các Prefab Tree, Rock, Grass từ RPGPP_LT
        spawner.treePrefabs = LoadPrefabsFromPath("Assets/RPGPP_LT/Prefabs/Nature/Vegetation/Trees");
        spawner.rockPrefabs = LoadPrefabsFromPath("Assets/RPGPP_LT/Prefabs/Nature/Rocks");
        spawner.grassPrefabs = LoadPrefabsFromPath("Assets/RPGPP_LT/Prefabs/Nature/Vegetation/Grass");
        spawner.flowerPrefabs = LoadPrefabsFromPath("Assets/RPGPP_LT/Prefabs/Nature/Vegetation/Flowers");

        // Cài đặt tỉ lệ và kích thước cân đối với Enemy (cellSize = 1)
        spawner.treeSpawnRate = 0.12f;
        spawner.rockSpawnRate = 0.10f;
        spawner.grassSpawnRate = 0.25f;
        spawner.treeScaleRange = new Vector2(0.25f, 0.35f);
        spawner.rockScaleRange = new Vector2(0.30f, 0.45f);
        spawner.grassScaleRange = new Vector2(0.35f, 0.50f);
        spawner.yOffset = 0f;

        EditorUtility.SetDirty(spawner);

        Debug.Log("<color=green>Đã cập nhật cấu hình EnvironmentManager với Material URP và tỷ lệ kích thước chuẩn tương thích với Enemy!</color>");
    }

    private static List<GameObject> LoadPrefabsFromPath(string folderPath)
    {
        List<GameObject> list = new List<GameObject>();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                list.Add(prefab);
            }
        }
        return list;
    }
}