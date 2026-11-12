using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class MapGenerator
{
    private const string BASE_SCENE_PATH = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/Generate Maps (Map 2, 3, 4, 5)")]
    public static void GenerateAllMaps()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // Đảm bảo SampleScene có MapSelectorUI để tiện chuyển qua lại
        AddMapSelectorToScene(BASE_SCENE_PATH);

        // Map 2: 10x10, Zic-Zac 3 khúc cua
        CreateMap("Assets/Scenes/Map2.unity", 10, 10,
            new Vector3(0.5f, 0f, 1.5f),
            new Vector3(9.5f, 0f, 8.5f),
            new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(3, 1), new Vector2Int(4, 1), new Vector2Int(5, 1), new Vector2Int(6, 1), new Vector2Int(7, 1),
                new Vector2Int(7, 2), new Vector2Int(7, 3), new Vector2Int(7, 4),
                new Vector2Int(6, 4), new Vector2Int(5, 4), new Vector2Int(4, 4), new Vector2Int(3, 4), new Vector2Int(2, 4), new Vector2Int(1, 4),
                new Vector2Int(1, 5), new Vector2Int(1, 6), new Vector2Int(1, 7),
                new Vector2Int(2, 7), new Vector2Int(3, 7), new Vector2Int(4, 7), new Vector2Int(5, 7), new Vector2Int(6, 7), new Vector2Int(7, 7), new Vector2Int(8, 7), new Vector2Int(9, 7), new Vector2Int(9, 8)
            },
            enemyCountMultiplier: 1.3f,
            spawnRateMultiplier: 1.2f,
            hpMultiplier: 1.2f
        );

        // Map 3: 12x12, Xoắn ốc (Spiral Maze)
        CreateMap("Assets/Scenes/Map3.unity", 12, 12,
            new Vector3(0.5f, 0f, 0.5f),
            new Vector3(5.5f, 0f, 5.5f),
            new List<Vector2Int> {
                new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0), new Vector2Int(4,0), new Vector2Int(5,0), new Vector2Int(6,0), new Vector2Int(7,0), new Vector2Int(8,0), new Vector2Int(9,0), new Vector2Int(10,0), new Vector2Int(11,0),
                new Vector2Int(11,1), new Vector2Int(11,2), new Vector2Int(11,3), new Vector2Int(11,4), new Vector2Int(11,5), new Vector2Int(11,6), new Vector2Int(11,7), new Vector2Int(11,8), new Vector2Int(11,9), new Vector2Int(11,10), new Vector2Int(11,11),
                new Vector2Int(10,11), new Vector2Int(9,11), new Vector2Int(8,11), new Vector2Int(7,11), new Vector2Int(6,11), new Vector2Int(5,11), new Vector2Int(4,11), new Vector2Int(3,11), new Vector2Int(2,11), new Vector2Int(1,11),
                new Vector2Int(1,10), new Vector2Int(1,9), new Vector2Int(1,8), new Vector2Int(1,7), new Vector2Int(1,6), new Vector2Int(1,5), new Vector2Int(1,4), new Vector2Int(1,3), new Vector2Int(1,2),
                new Vector2Int(2,2), new Vector2Int(3,2), new Vector2Int(4,2), new Vector2Int(5,2), new Vector2Int(6,2), new Vector2Int(7,2), new Vector2Int(8,2), new Vector2Int(9,2),
                new Vector2Int(9,3), new Vector2Int(9,4), new Vector2Int(9,5), new Vector2Int(9,6), new Vector2Int(9,7), new Vector2Int(9,8), new Vector2Int(9,9),
                new Vector2Int(8,9), new Vector2Int(7,9), new Vector2Int(6,9), new Vector2Int(5,9), new Vector2Int(4,9), new Vector2Int(3,9),
                new Vector2Int(3,8), new Vector2Int(3,7), new Vector2Int(3,6), new Vector2Int(3,5), new Vector2Int(3,4),
                new Vector2Int(4,4), new Vector2Int(5,4), new Vector2Int(5,5)
            },
            enemyCountMultiplier: 1.6f,
            spawnRateMultiplier: 1.4f,
            hpMultiplier: 1.5f
        );

        // Map 4: 16x16, Bản đồ lớn đường đi dài
        CreateMap("Assets/Scenes/Map4.unity", 16, 16,
            new Vector3(0.5f, 0f, 14.5f),
            new Vector3(15.5f, 0f, 1.5f),
            new List<Vector2Int> {
                new Vector2Int(0, 14), new Vector2Int(1, 14), new Vector2Int(2, 14), new Vector2Int(3, 14), new Vector2Int(4, 14),
                new Vector2Int(4, 13), new Vector2Int(4, 12), new Vector2Int(4, 11),
                new Vector2Int(5, 11), new Vector2Int(6, 11), new Vector2Int(7, 11), new Vector2Int(8, 11), new Vector2Int(9, 11),
                new Vector2Int(9, 12), new Vector2Int(9, 13), new Vector2Int(9, 14),
                new Vector2Int(10, 14), new Vector2Int(11, 14), new Vector2Int(12, 14), new Vector2Int(13, 14),
                new Vector2Int(13, 13), new Vector2Int(13, 12), new Vector2Int(13, 11), new Vector2Int(13, 10), new Vector2Int(13, 9), new Vector2Int(13, 8), new Vector2Int(13, 7),
                new Vector2Int(12, 7), new Vector2Int(11, 7), new Vector2Int(10, 7), new Vector2Int(9, 7), new Vector2Int(8, 7), new Vector2Int(7, 7), new Vector2Int(6, 7), new Vector2Int(5, 7), new Vector2Int(4, 7), new Vector2Int(3, 7), new Vector2Int(2, 7),
                new Vector2Int(2, 6), new Vector2Int(2, 5), new Vector2Int(2, 4), new Vector2Int(2, 3),
                new Vector2Int(3, 3), new Vector2Int(4, 3), new Vector2Int(5, 3), new Vector2Int(6, 3), new Vector2Int(7, 3), new Vector2Int(8, 3), new Vector2Int(9, 3), new Vector2Int(10, 3), new Vector2Int(11, 3), new Vector2Int(12, 3), new Vector2Int(13, 3), new Vector2Int(14, 3),
                new Vector2Int(14, 2), new Vector2Int(14, 1), new Vector2Int(15, 1)
            },
            enemyCountMultiplier: 2.0f,
            spawnRateMultiplier: 1.6f,
            hpMultiplier: 1.8f
        );

        // Map 5: 18x18, Siêu Thử Thách
        CreateMap("Assets/Scenes/Map5.unity", 18, 18,
            new Vector3(1.5f, 0f, 16.5f),
            new Vector3(16.5f, 0f, 1.5f),
            new List<Vector2Int> {
                new Vector2Int(1, 16), new Vector2Int(2, 16), new Vector2Int(3, 16), new Vector2Int(4, 16), new Vector2Int(5, 16), new Vector2Int(6, 16),
                new Vector2Int(6, 15), new Vector2Int(6, 14), new Vector2Int(6, 13), new Vector2Int(6, 12),
                new Vector2Int(5, 12), new Vector2Int(4, 12), new Vector2Int(3, 12), new Vector2Int(2, 12), new Vector2Int(1, 12),
                new Vector2Int(1, 11), new Vector2Int(1, 10), new Vector2Int(1, 9), new Vector2Int(1, 8),
                new Vector2Int(2, 8), new Vector2Int(3, 8), new Vector2Int(4, 8), new Vector2Int(5, 8), new Vector2Int(6, 8), new Vector2Int(7, 8), new Vector2Int(8, 8),
                new Vector2Int(8, 9), new Vector2Int(8, 10), new Vector2Int(8, 11), new Vector2Int(8, 12), new Vector2Int(8, 13), new Vector2Int(8, 14), new Vector2Int(8, 15), new Vector2Int(8, 16),
                new Vector2Int(9, 16), new Vector2Int(10, 16), new Vector2Int(11, 16), new Vector2Int(12, 16), new Vector2Int(13, 16), new Vector2Int(14, 16),
                new Vector2Int(14, 15), new Vector2Int(14, 14), new Vector2Int(14, 13), new Vector2Int(14, 12), new Vector2Int(14, 11),
                new Vector2Int(13, 11), new Vector2Int(12, 11), new Vector2Int(11, 11), new Vector2Int(10, 11),
                new Vector2Int(10, 10), new Vector2Int(10, 9), new Vector2Int(10, 8), new Vector2Int(10, 7), new Vector2Int(10, 6),
                new Vector2Int(11, 6), new Vector2Int(12, 6), new Vector2Int(13, 6), new Vector2Int(14, 6), new Vector2Int(15, 6), new Vector2Int(16, 6),
                new Vector2Int(16, 5), new Vector2Int(16, 4), new Vector2Int(16, 3), new Vector2Int(16, 2), new Vector2Int(16, 1)
            },
            enemyCountMultiplier: 2.8f,
            spawnRateMultiplier: 2.0f,
            hpMultiplier: 2.5f
        );

        AddScenesToBuildSettings();

        // Mở lại SampleScene ban đầu
        EditorSceneManager.OpenScene(BASE_SCENE_PATH);

        Debug.Log("<color=green><b>ĐÃ TẠO THÀNH CÔNG TẤT CẢ MAP (Map2, Map3, Map4, Map5)!</b></color>");
    }

    private static void AddMapSelectorToScene(string scenePath)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (Object.FindFirstObjectByType<MapSelectorUI>() == null)
        {
            GameObject selectorObj = new GameObject("MapSelectorUI");
            selectorObj.AddComponent<MapSelectorUI>();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void CreateMap(string scenePath, int width, int height, Vector3 startPos, Vector3 endPos, List<Vector2Int> pathTiles, float enemyCountMultiplier, float spawnRateMultiplier, float hpMultiplier)
    {
        AssetDatabase.CopyAsset(BASE_SCENE_PATH, scenePath);
        AssetDatabase.Refresh();

        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // 1. Tinh chỉnh GridManager
        GridManager gridManager = Object.FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            gridManager.width = width;
            gridManager.height = height;
            gridManager.gridSizeX = width;
            gridManager.gridSizeY = height;
            EditorUtility.SetDirty(gridManager);
        }

        // 2. Di chuyển spawnstart và spawnend
        GameObject startObj = GameObject.Find("spawnstart");
        if (startObj != null)
        {
            startObj.transform.position = startPos;
            EditorUtility.SetDirty(startObj);
        }

        GameObject endObj = GameObject.Find("spawnend");
        if (endObj != null)
        {
            endObj.transform.position = endPos;
            EditorUtility.SetDirty(endObj);
        }

        // 3. Cấu hình các khối đường đi (PathMap)
        GameObject pathMapObj = GameObject.Find("PathMap");
        if (pathMapObj != null)
        {
            for (int i = pathMapObj.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(pathMapObj.transform.GetChild(i).gameObject);
            }

            pathMapObj.transform.position = Vector3.zero;

            GameObject roadPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Tower Defense Kit/tile-straight.fbx");

            foreach (Vector2Int tile in pathTiles)
            {
                Vector3 tilePos = new Vector3(tile.x * 1f, 0.05f, tile.y * 1f);
                GameObject roadBlock = roadPrefab != null ? (GameObject)PrefabUtility.InstantiatePrefab(roadPrefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
                roadBlock.name = $"Road_{tile.x}_{tile.y}";
                roadBlock.transform.position = tilePos;
                roadBlock.transform.SetParent(pathMapObj.transform, true);

                roadBlock.layer = 8;
                foreach (Transform child in roadBlock.transform)
                {
                    child.gameObject.layer = 8;
                }
            }
            EditorUtility.SetDirty(pathMapObj);
        }

        // 4. Điều chỉnh độ khó của WaveManager
        WaveManager waveManager = Object.FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.spawnPoint = startObj != null ? startObj.transform : null;
            waveManager.targetPoint = endObj != null ? endObj.transform : null;

            if (waveManager.waves != null)
            {
                for (int w = 0; w < waveManager.waves.Count; w++)
                {
                    Wave currentWave = waveManager.waves[w];
                    List<WaveElement> newElements = new List<WaveElement>();

                    foreach (var el in currentWave.elements)
                    {
                        WaveElement newEl = el;
                        newEl.count = Mathf.RoundToInt(el.count * enemyCountMultiplier);
                        newEl.spawnRate = el.spawnRate * spawnRateMultiplier;
                        newElements.Add(newEl);
                    }

                    currentWave.elements = newElements;
                    currentWave.waveName = $"{currentWave.waveName} (Độ khó: x{hpMultiplier:F1})";
                    waveManager.waves[w] = currentWave;
                }
            }

            waveManager.timeBetweenWaves = Mathf.Max(3f, 5f - (hpMultiplier - 1f) * 1.5f);
            EditorUtility.SetDirty(waveManager);
        }

        // 5. Cấu hình Camera góc nhìn bao quát Map kèm CameraController
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraController camCtrl = mainCam.GetComponent<CameraController>();
            if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<CameraController>();
            camCtrl.gridManager = gridManager;
            camCtrl.autoFitToGrid = true;
            camCtrl.FitCameraToGrid();
            EditorUtility.SetDirty(mainCam);
        }

        // 6. Tự động kết nối EnvironmentManager
        EnvironmentSpawner envSpawner = Object.FindFirstObjectByType<EnvironmentSpawner>();
        if (envSpawner != null)
        {
            envSpawner.spawnPoint = startObj != null ? startObj.transform : null;
            envSpawner.basePoint = endObj != null ? endObj.transform : null;
            envSpawner.gridManager = gridManager;
            EditorUtility.SetDirty(envSpawner);
        }

        // 7. Thêm menu chuyển đổi map
        if (Object.FindFirstObjectByType<MapSelectorUI>() == null)
        {
            GameObject selectorObj = new GameObject("MapSelectorUI");
            selectorObj.AddComponent<MapSelectorUI>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void AddScenesToBuildSettings()
    {
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        buildScenes.Add(new EditorBuildSettingsScene(BASE_SCENE_PATH, true));
        buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Map2.unity", true));
        buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Map3.unity", true));
        buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Map4.unity", true));
        buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Map5.unity", true));

        EditorBuildSettings.scenes = buildScenes.ToArray();
    }
}