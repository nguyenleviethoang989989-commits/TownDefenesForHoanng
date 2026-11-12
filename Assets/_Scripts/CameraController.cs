using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CameraController : MonoBehaviour
{
    [Header("Cài đặt Tự động Fit Map")]
    public bool autoFitToGrid = true;
    public GridManager gridManager;

    [Header("Tốc độ Di chuyển")]
    public float panSpeed = 25f;

    [Header("Cài đặt Zoom")]
    public float zoomSpeed = 2f; // Tốc độ zoom orthographic
    public float minOrthoSize = 2.5f;
    public float maxOrthoSize = 25f;

    [Header("Giới hạn di chuyển (Pan Limits)")]
    public Vector2 panLimitX;
    public Vector2 panLimitZ;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    private void Start()
    {
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();

        if (autoFitToGrid)
        {
            FitCameraToGrid();
        }
    }

    [ContextMenu("Fit Camera To Grid")]
    public void FitCameraToGrid()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null || cam == null) return;

        float width = gridManager.width * gridManager.cellSize;
        float height = gridManager.height * gridManager.cellSize;

        // Tâm của Map trên mặt phẳng (X, Z)
        float centerX = (width - gridManager.cellSize) / 2f;
        float centerZ = (height - gridManager.cellSize) / 2f;

        // Đặt góc nhìn nghiêng 60 độ
        float pitchAngle = 60f;
        transform.rotation = Quaternion.Euler(pitchAngle, 0f, 0f);

        // Đặt vị trí Camera sao cho tâm tầm nhìn chiếu thẳng vào tâm Map
        float camDistance = 30f;
        float camY = camDistance * Mathf.Sin(pitchAngle * Mathf.Deg2Rad);
        float camZOffset = camDistance * Mathf.Cos(pitchAngle * Mathf.Deg2Rad);

        transform.position = new Vector3(centerX, camY, centerZ - camZOffset);

        // NẾU CAMERA LÀ ORTHOGRAPHIC: Thay đổi orthographicSize để bao quát trọn vẹn Map
        if (cam.orthographic)
        {
            // Trong isometric/orthographic nhìn nghiêng 60 độ:
            // Chiều ngang cần size >= width / (2 * aspectRatio)
            // Chiều dọc chiếu nghiêng cần size >= height / 2 + lề
            float screenAspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            if (screenAspect <= 0.1f) screenAspect = 16f / 9f;

            float requiredVerticalSize = (height * 0.75f);
            float requiredHorizontalSize = (width / (2f * screenAspect)) * 1.25f;

            float fitSize = Mathf.Max(requiredVerticalSize, requiredHorizontalSize);
            cam.orthographicSize = Mathf.Max(5f, fitSize);

            minOrthoSize = 3f;
            maxOrthoSize = cam.orthographicSize * 1.8f;
        }
        else
        {
            // NẾU LÀ PERSPECTIVE CAMERA
            float camHeight = Mathf.Max(width, height) * 1.35f;
            transform.position = new Vector3(centerX, camHeight, centerZ - (camHeight * 0.45f));
        }

        // Thiết lập giới hạn di chuyển bao quanh Map
        panLimitX = new Vector2(-width * 0.3f, width * 1.3f);
        panLimitZ = new Vector2(-height * 0.3f, height * 1.3f);
    }

    private void Update()
    {
        HandleKeyboardPan();
        HandleMouseDragPan();
        HandleZoom();
    }

    private void HandleKeyboardPan()
    {
        Vector3 pos = transform.position;
        float horizontal = 0f;
        float vertical = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
        }
#else
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
#endif

        if (horizontal != 0f || vertical != 0f)
        {
            // Di chuyển song song với mặt đất
            pos.x += horizontal * panSpeed * Time.deltaTime;
            pos.z += vertical * panSpeed * Time.deltaTime;

            pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
            pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

            transform.position = pos;
        }
    }

    private void HandleMouseDragPan()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
            {
                dragOrigin = Mouse.current.position.ReadValue();
                isDragging = true;
            }
            if (Mouse.current.rightButton.wasReleasedThisFrame || Mouse.current.middleButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 currentMousePos = Mouse.current.position.ReadValue();
                Vector3 diff = (dragOrigin - currentMousePos) * 0.02f;
                dragOrigin = currentMousePos;

                Vector3 pos = transform.position;
                pos.x += diff.x * panSpeed * 0.5f;
                pos.z += diff.y * panSpeed * 0.5f;

                pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
                pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

                transform.position = pos;
            }
        }
#endif
    }

    private void HandleZoom()
    {
        float scrollDelta = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            float rawScroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(rawScroll) > 0.1f)
            {
                scrollDelta = Mathf.Sign(rawScroll);
            }
        }
#else
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            scrollDelta = Mathf.Sign(scroll);
        }
#endif

        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            if (cam != null && cam.orthographic)
            {
                // THU PHÓNG BẰNG ORTHOGRAPHIC SIZE (Cuộn lên = zoom gần = size giảm)
                cam.orthographicSize -= scrollDelta * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);
            }
            else
            {
                // THU PHÓNG BẰNG VECTOR VỊ TRÍ (Perspective)
                Vector3 pos = transform.position;
                pos += transform.forward * (scrollDelta * zoomSpeed * 3f);
                transform.position = pos;
            }
        }
    }
}