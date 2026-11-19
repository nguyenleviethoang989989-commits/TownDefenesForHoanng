using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Định nghĩa cấu trúc của một Wave
[System.Serializable]
public struct Wave
{
    public string waveName; // Tên của đợt (ví dụ: "Đợt 1 - Khởi động")
    public List<WaveElement> elements; // Các nhóm quái trong đợt này
}

// Định nghĩa từng nhóm quái vật bên trong một Wave
[System.Serializable]
public struct WaveElement
{
    public string enemyTag; // Tag của quái trong Object Pool ("EnemyNormal", "EnemyFast", "EnemyTanker", "EnemyBoss")
    public int count;       // Số lượng quái vật loại này
    public float spawnRate; // Tốc độ sinh quái (Ví dụ: 2 => 0.5s / con)
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Cài đặt Wave")]
    public Transform spawnPoint;  // Điểm quái vật xuất hiện (spawnstart)
    public Transform targetPoint; // Điểm đích đến của quái vật (spawnend / Base)
    public List<Wave> waves;      // Danh sách các Wave trong level

    [Header("Trạng thái hiện tại")]
    public float timeBetweenWaves = 5f; // Thời gian nghỉ giữa các đợt
    private float countdown;            // Đếm ngược thời gian chờ đợt tiếp theo

    public int currentWaveIndex = 0;   // Đợt hiện tại
    private int loopCycleCount = 1;     // Số vòng lặp đã qua
    private bool isSpawning = false;    // Đang trong quá trình sinh quái hay không

    [Header("Chế độ Lặp Vô Tận (Endless Waves)")]
    public bool endlessWaves = true;    // Tự động lặp lại đợt quái nếu căn cứ chưa hết máu

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (targetPoint == null)
        {
            GameObject endObj = GameObject.Find("spawnend");
            if (endObj != null) targetPoint = endObj.transform;
        }

        if (spawnPoint == null)
        {
            GameObject startObj = GameObject.Find("spawnstart");
            if (startObj != null) spawnPoint = startObj.transform;
        }
    }

    private void Start()
    {
        countdown = timeBetweenWaves;

        // Tự động hoán đổi loại quái tùy theo từng Map (giữ nguyên Count & Spawn Rate)
        ConfigureWavesForCurrentMap();
    }

    // Tự động thay đổi phong cách enemy theo từng Map
    private void ConfigureWavesForCurrentMap()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Xác định loại quái theo từng map:
        // Đợt 1: 1 loại quái
        // Đợt 2: 2 loại quái còn lại
        // Đợt 3: Boss (giữ nguyên ở tất cả các map)
        string wave1Enemy = "EnemyNormal";
        string wave2EnemyA = "EnemyFast";
        string wave2EnemyB = "EnemyTanker";

        if (sceneName == "Map2")
        {
            // Map 2: Đợt 1 là EnemyFast, Đợt 2 là Normal + Tanker
            wave1Enemy = "EnemyFast";
            wave2EnemyA = "EnemyNormal";
            wave2EnemyB = "EnemyTanker";
        }
        else if (sceneName == "Map3")
        {
            // Map 3: Đợt 1 là EnemyTanker (trâu bò ở mê cung), Đợt 2 là Fast + Normal
            wave1Enemy = "EnemyTanker";
            wave2EnemyA = "EnemyFast";
            wave2EnemyB = "EnemyNormal";
        }
        else if (sceneName == "Map4")
        {
            // Map 4: Đợt 1 là EnemyFast (chạy đường dài), Đợt 2 là Tanker + Normal
            wave1Enemy = "EnemyFast";
            wave2EnemyA = "EnemyTanker";
            wave2EnemyB = "EnemyNormal";
        }
        else if (sceneName == "Map5")
        {
            // Map 5: Đợt 1 là EnemyTanker, Đợt 2 là Fast + Tanker
            wave1Enemy = "EnemyTanker";
            wave2EnemyA = "EnemyFast";
            wave2EnemyB = "EnemyTanker";
        }

        // Cập nhật vào danh sách waves nếu có đủ 3 đợt
        if (waves != null && waves.Count >= 2)
        {
            // Cập nhật Đợt 1
            Wave w1 = waves[0];
            if (w1.elements != null && w1.elements.Count > 0)
            {
                WaveElement e1 = w1.elements[0];
                e1.enemyTag = wave1Enemy;
                w1.elements[0] = e1;
            }
            w1.waveName = $"Đợt 1 - Khởi động ({wave1Enemy})";
            waves[0] = w1;

            // Cập nhật Đợt 2
            Wave w2 = waves[1];
            if (w2.elements != null)
            {
                if (w2.elements.Count > 0)
                {
                    WaveElement e2A = w2.elements[0];
                    e2A.enemyTag = wave2EnemyA;
                    w2.elements[0] = e2A;
                }
                if (w2.elements.Count > 1)
                {
                    WaveElement e2B = w2.elements[1];
                    e2B.enemyTag = wave2EnemyB;
                    w2.elements[1] = e2B;
                }
            }
            w2.waveName = $"Đợt 2 - Tăng cường ({wave2EnemyA} & {wave2EnemyB})";
            waves[1] = w2;

            // Đợt 3 - Boss: Giữ nguyên EnemyBoss
            if (waves.Count >= 3)
            {
                Wave w3 = waves[2];
                w3.waveName = "Đợt 3 - Boss (EnemyBoss)";
                waves[2] = w3;
            }
        }
    }

    private void Update()
    {
        // Nếu Game Over (căn cứ hết máu) thì ngừng hẳn sinh quái
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            return;
        }

        // Đang sinh quái thì không đếm ngược
        if (isSpawning)
        {
            return;
        }

        // KIỂM TRA HẾT 3 ĐỢT WAVES
        if (currentWaveIndex >= waves.Count)
        {
            if (endlessWaves)
            {
                // Kiểm tra điều kiện căn cứ còn máu
                if (GameManager.Instance != null && GameManager.Instance.currentBaseHP > 0)
                {
                    // Kiểm tra xem quái vật của đợt trước đã chết hoặc chạm base hết chưa
                    // Nếu còn quái trên map thì đợi quái dọn xong mới bắt đầu vòng mới
                    if (HasAliveEnemies())
                    {
                        return;
                    }

                    // TỰ ĐỘNG LẶP LẠI VÒNG WAVES MỚI
                    loopCycleCount++;
                    currentWaveIndex = 0;
                    countdown = timeBetweenWaves;
                    Debug.Log($"<color=yellow><b>[WaveManager]</b> Hoàn thành xuất sắc vòng trước! Căn cứ còn {GameManager.Instance.currentBaseHP} máu. Bắt đầu Vòng Wave tiếp theo #{loopCycleCount}!</color>");
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
    }

    private bool HasAliveEnemies()
    {
        // Tìm xem còn quái vật nào đang hoạt động trên màn chơi không
        EnemyMovement[] aliveEnemies = Object.FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        foreach (var enemy in aliveEnemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    private int totalWavesSpawned = 0; // Đếm tổng số đợt quái đã xuất hiện liên tục

    IEnumerator SpawnWave()
    {
        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex];
        totalWavesSpawned++;

        string loopPrefix = loopCycleCount > 1 ? $"[Vòng {loopCycleCount}] " : "";
        Debug.Log("Bắt đầu " + loopPrefix + currentWave.waveName);

        // Kiểm tra xem đợt này có chứa Boss hay không
        bool hasBoss = false;
        if (currentWave.elements != null)
        {
            foreach (var el in currentWave.elements)
            {
                if (el.enemyTag == "EnemyBoss" || el.enemyTag.Contains("Boss"))
                {
                    hasBoss = true;
                    break;
                }
            }
        }
        if (currentWave.waveName != null && currentWave.waveName.ToLower().Contains("boss"))
        {
            hasBoss = true;
        }

        // Thông báo UI Đợt Quái:
        // Đợt thường: Màu vàng cam sáng (Color(1f, 0.85f, 0.2f))
        // Đợt Boss: Màu đỏ cam rực rỡ / tím hồng rực lửa (Color(1f, 0.25f, 0.1f) hoặc viền vàng rực rỡ)
        if (UIManager.Instance != null)
        {
            if (hasBoss)
            {
                // Thông báo Boss nổi bật rực rỡ
                UIManager.Instance.ShowEventNotification($"⚠ BẮT ĐẦU ĐỢT {totalWavesSpawned} - BOSS XUẤT HIỆN! ⚠", new Color(1f, 0.22f, 0.1f, 1f), 4.5f);
            }
            else
            {
                // Thông báo đợt quái thường
                UIManager.Instance.ShowEventNotification($"Bắt đầu đợt {totalWavesSpawned}", new Color(1f, 0.85f, 0.2f, 1f), 3f);
            }
        }

        foreach (WaveElement element in currentWave.elements)
        {
            for (int i = 0; i < element.count; i++)
            {
                // Nếu đang sinh mà căn cứ hết máu thì dừng lại
                if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                {
                    isSpawning = false;
                    yield break;
                }

                SpawnEnemy(element.enemyTag);
                yield return new WaitForSeconds(1f / element.spawnRate);
            }
        }

        currentWaveIndex++;
        isSpawning = false;
    }

    private void SpawnEnemy(string tag)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Chưa gán điểm Spawn Point trong WaveManager!");
            return;
        }

        GameObject enemy = ObjectPool.Instance.SpawnFromPool(tag, spawnPoint.position, spawnPoint.rotation);

        if (enemy != null)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SpawnSetup(spawnPoint, targetPoint);
            }
        }
    }
}