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
        // Nếu game chưa bắt đầu (vẫn đang ở UI Main Menu hoặc Loading), không nhận phím chuyển map
        if (MainMenuUIController.Instance != null && !MainMenuUIController.Instance.isGameStarted)
        {
            return;
        }

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
        try
        {
            if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab))
            {
                showMenu = !showMenu;
            }
        }
        catch
        {
        }
#endif
    }

    private void OnGUI()
    {
        // ẨN TOÀN BỘ KHI Ở MENU, LOADING HOẶC ĐÃ GAME OVER / WIN!
        if (MainMenuUIController.Instance != null && !MainMenuUIController.Instance.isGameStarted)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        // Ẩn nút Chọn Map khi đang Pause Game
        if (PauseManager.Instance != null && PauseManager.Instance.isPaused)
        {
            return;
        }

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

        // Chỉ hiển thị nút toggle khi đã vào màn hình chơi game
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