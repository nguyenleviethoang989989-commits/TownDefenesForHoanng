using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Nhạc Nền")]
    public AudioClip menuMusic;     // Nhạc cho Menu (VD: Pirate 1)
    public AudioClip gameplayMusic; // Nhạc cho lúc chơi game (VD: Pirate 2)

    [Header("Cài đặt Âm thanh")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    private bool isMuted = false;

    private const string MUTE_PREF_KEY = "IsMusicMuted";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ nhạc chạy mượt mà qua các scene nếu có
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.GetComponent<AudioSource>();
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        // Đọc trạng thái Mute đã lưu từ trước
        isMuted = PlayerPrefs.GetInt(MUTE_PREF_KEY, 0) == 1;
        ApplyMuteState();
    }

    private void Start()
    {
        // Khi mới vào game (ở UI Menu), phát nhạc Menu
        if (menuMusic != null)
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (musicSource == null || menuMusic == null) return;
        if (musicSource.clip == menuMusic && musicSource.isPlaying) return;

        musicSource.clip = menuMusic;
        musicSource.volume = isMuted ? 0f : musicVolume;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        if (musicSource == null || gameplayMusic == null) return;
        if (musicSource.clip == gameplayMusic && musicSource.isPlaying) return;

        musicSource.clip = gameplayMusic;
        musicSource.volume = isMuted ? 0f : musicVolume;
        musicSource.Play();
    }

    // Hàm Bật/Tắt âm thanh (Toggle)
    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MUTE_PREF_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMuteState();
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    private void ApplyMuteState()
    {
        if (musicSource != null)
        {
            musicSource.volume = isMuted ? 0f : musicVolume;
        }

        // Cập nhật tất cả các nút SoundToggleButton trong Scene
        SoundToggleButton[] buttons = Object.FindObjectsByType<SoundToggleButton>(FindObjectsSortMode.None);
        foreach (var btn in buttons)
        {
            btn.UpdateVisual(isMuted);
        }
    }
}