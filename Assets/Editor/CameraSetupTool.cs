using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class CameraSetupTool
{
    private static readonly string[] ALL_SCENES = {
        "Assets/Scenes/SampleScene.unity",
        "Assets/Scenes/Map2.unity",
        "Assets/Scenes/Map3.unity",
        "Assets/Scenes/Map4.unity",
        "Assets/Scenes/Map5.unity"
    };

    [MenuItem("Tools/Setup Dynamic Camera For All Maps")]
    public static void SetupCameraForAllScenes()
    {
        string currentScenePath = SceneManager.GetActiveScene().path;

        foreach (string scenePath in ALL_SCENES)
        {
            if (!System.IO.File.Exists(scenePath)) continue;

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                CameraController controller = mainCam.GetComponent<CameraController>();
                if (controller == null)
                {
                    controller = Undo.AddComponent<CameraController>(mainCam.gameObject);
                }

                controller.gridManager = Object.FindFirstObjectByType<GridManager>();
                controller.autoFitToGrid = true;
                controller.panSpeed = 25f;
                controller.zoomSpeed = 1.5f;

                // Căn chỉnh trực tiếp
                controller.FitCameraToGrid();

                EditorUtility.SetDirty(mainCam);
                EditorUtility.SetDirty(controller);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        Debug.Log("<color=green><b>ĐÃ CĂN CHỈNH CAMERA CHUẨN XÁC CHO TẤT CẢ MAP (KỂ CẢ ORTHOGRAPHIC)!</b></color>");
    }
}