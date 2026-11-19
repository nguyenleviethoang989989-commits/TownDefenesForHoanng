using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class WaveConfigSetupTool
{
    private static readonly string[] ALL_SCENES = {
        "Assets/Scenes/SampleScene.unity",
        "Assets/Scenes/Map2.unity",
        "Assets/Scenes/Map3.unity",
        "Assets/Scenes/Map4.unity",
        "Assets/Scenes/Map5.unity"
    };

    [MenuItem("Tools/Setup Wave Config For All Maps")]
    public static void SetupWavesForAllScenes()
    {
        string currentScenePath = SceneManager.GetActiveScene().path;

        foreach (string scenePath in ALL_SCENES)
        {
            if (!System.IO.File.Exists(scenePath)) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            string sceneName = scene.name;

            WaveManager waveManager = Object.FindFirstObjectByType<WaveManager>();
            if (waveManager != null)
            {
                waveManager.endlessWaves = true;
                waveManager.timeBetweenWaves = 5f;

                // Cấu hình 3 đợt chuẩn cho từng Map
                List<Wave> newWaves = new List<Wave>();

                // Xác định loại quái cho Đợt 1 và Đợt 2 theo Map
                string w1Tag = "EnemyNormal";
                string w2TagA = "EnemyFast";
                string w2TagB = "EnemyTanker";

                if (sceneName == "Map2")
                {
                    w1Tag = "EnemyFast";
                    w2TagA = "EnemyNormal";
                    w2TagB = "EnemyTanker";
                }
                else if (sceneName == "Map3")
                {
                    w1Tag = "EnemyTanker";
                    w2TagA = "EnemyFast";
                    w2TagB = "EnemyNormal";
                }
                else if (sceneName == "Map4")
                {
                    w1Tag = "EnemyFast";
                    w2TagA = "EnemyTanker";
                    w2TagB = "EnemyNormal";
                }
                else if (sceneName == "Map5")
                {
                    w1Tag = "EnemyTanker";
                    w2TagA = "EnemyFast";
                    w2TagB = "EnemyTanker";
                }

                // 1. Đợt 1 - Khởi động (Count: 5, SpawnRate: 2)
                Wave wave1 = new Wave();
                wave1.waveName = $"Đợt 1 - Khởi động ({w1Tag})";
                wave1.elements = new List<WaveElement> {
                    new WaveElement { enemyTag = w1Tag, count = 5, spawnRate = 2f }
                };
                newWaves.Add(wave1);

                // 2. Đợt 2 - Tăng cường (Enemy A: count 3, rate 1.5; Enemy B: count 2, rate 0.5)
                Wave wave2 = new Wave();
                wave2.waveName = $"Đợt 2 - Tăng cường ({w2TagA} & {w2TagB})";
                wave2.elements = new List<WaveElement> {
                    new WaveElement { enemyTag = w2TagA, count = 3, spawnRate = 1.5f },
                    new WaveElement { enemyTag = w2TagB, count = 2, spawnRate = 0.5f }
                };
                newWaves.Add(wave2);

                // 3. Đợt 3 - Boss (Giữ nguyên EnemyBoss: count 3, rate 1.5)
                Wave wave3 = new Wave();
                wave3.waveName = "Đợt 3 - Boss (EnemyBoss)";
                wave3.elements = new List<WaveElement> {
                    new WaveElement { enemyTag = "EnemyBoss", count = 3, spawnRate = 1.5f }
                };
                newWaves.Add(wave3);

                waveManager.waves = newWaves;

                EditorUtility.SetDirty(waveManager);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        Debug.Log("<color=green><b>[WaveConfigSetupTool]</b> Đã cập nhật xong cấu hình Waves cho cả 5 Map: Hỗ trợ lặp vô tận khi Base còn máu & Hoán đổi Enemy mới theo từng Map!</color>");
        EditorUtility.DisplayDialog("Thành công", "Đã cập nhật xong cấu hình Waves cho cả 5 Map!", "OK");
    }
}