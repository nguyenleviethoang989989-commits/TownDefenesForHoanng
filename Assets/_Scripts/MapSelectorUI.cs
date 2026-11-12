using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MapSelectorUI : MonoBehaviour
{
    private bool showMenu = false;
    private GUIStyle buttonStyle;
    private GUIStyle headerStyle;

    private void Update()
    {
        // Hỗ trợ cả New Input System và Legacy Input để không bao giờ bị InvalidOperationException
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.mKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame))
        {
            showMenu = !showMenu;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
        {
            showMenu = !showMenu;
        }
#else
        // Dự phòng nếu không xác định được define symbol
        try
        {
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
            {
                showMenu = !showMenu;
            }
        }
        catch
        {
            // Bỏ qua lỗi nếu dự án đang bật chế độ thuần New Input System
        }
#endif
    }

    private void OnGUI()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 14;
            buttonStyle.fontStyle = FontStyle.Bold;

            headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.fontSize = 16;
            headerStyle.fontStyle = FontStyle.Bold;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // Nút toggle cố định bằng chuột ở góc trên bên phải màn hình
        if (GUI.Button(new Rect(Screen.width - 150, 10, 140, 35), showMenu ? "Đóng Menu Map" : "Chọn Map (Phím M)"))
        {
            showMenu = !showMenu;
        }

        if (showMenu)
        {
            GUI.Box(new Rect(Screen.width - 230, 50, 220, 290), $"BẢN ĐỒ HIỆN TẠI:\n[{currentScene}]", headerStyle);

            if (GUI.Button(new Rect(Screen.width - 215, 100, 190, 32), "Map 1 (SampleScene)"))
            {
                SceneManager.LoadScene("SampleScene");
            }

            if (GUI.Button(new Rect(Screen.width - 215, 140, 190, 32), "Map 2 (Zic-zac)"))
            {
                SceneManager.LoadScene("Map2");
            }

            if (GUI.Button(new Rect(Screen.width - 215, 180, 190, 32), "Map 3 (Spiral Maze)"))
            {
                SceneManager.LoadScene("Map3");
            }

            if (GUI.Button(new Rect(Screen.width - 215, 220, 190, 32), "Map 4 (Large Map 16x16)"))
            {
                SceneManager.LoadScene("Map4");
            }

            if (GUI.Button(new Rect(Screen.width - 215, 260, 190, 32), "Map 5 (Extreme Boss)"))
            {
                SceneManager.LoadScene("Map5");
            }
        }
    }
}