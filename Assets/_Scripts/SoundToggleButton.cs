using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Button button;           // Nút bấm
    public Image speakerIcon;       // Ảnh Speaker
    public GameObject crossLine;    // Đối tượng gạch chéo hiển thị khi tắt âm

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            UpdateVisual(AudioManager.Instance.IsMuted());
        }
    }

    private void OnButtonClicked()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ToggleMute();
        }
    }

    // Cập nhật hiển thị (khi tắt âm thì hiện gạch chéo, khi bật thì ẩn)
    public void UpdateVisual(bool isMuted)
    {
        if (crossLine != null)
        {
            crossLine.SetActive(isMuted);
        }

        // Tùy chọn làm mờ nhẹ icon khi mute
        if (speakerIcon != null)
        {
            speakerIcon.color = isMuted ? new Color(0.8f, 0.8f, 0.8f, 0.65f) : Color.white;
        }
    }
}