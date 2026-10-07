using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Định nghĩa cấu trúc của một Wave
[System.Serializable]
public struct Wave
{
    public string waveName; // Tên của đợt (ví dụ: "Wave 1 - Khởi động")
    public List<WaveElement> elements; // Các nhóm quái trong đợt này
}

// Định nghĩa từng nhóm quái vật bên trong một Wave
[System.Serializable]
public struct WaveElement
{
    public string enemyTag; // Tag của quái trong Object Pool (VD: "EnemyNormal", "EnemyTanker")
    public int count;       // Số lượng quái vật loại này
    public float spawnRate; // Tốc độ sinh quái (Ví dụ: 0.5 giây / 1 con)
}

public class WaveManager : MonoBehaviour
{
    [Header("Cài đặt Wave")]
    public Transform spawnPoint; // Điểm quái vật xuất hiện
    public List<Wave> waves; // Danh sách các Wave trong level

    [Header("Trạng thái hiện tại")]
    public float timeBetweenWaves = 5f; // Thời gian nghỉ giữa các đợt (Wait for Wave)
    private float countdown; // Đếm ngược thời gian chờ đợt tiếp theo

    private int currentWaveIndex = 0; // Đợt hiện tại
    private bool isSpawning = false;  // Đang trong quá trình sinh quái hay không

    private void Start()
    {
        countdown = timeBetweenWaves; // Khởi tạo đếm ngược
    }

    private void Update()
    {
        // Vòng lặp cốt lõi: Chờ Wave -> Bắt đầu Wave
        if (currentWaveIndex >= waves.Count)
        {
            // Đã hết các Wave, có thể kiểm tra điều kiện thắng ở đây
            return;
        }

        if (isSpawning)
        {
            return; // Đang sinh quái thì không đếm ngược
        }

        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            // Hết thời gian chờ, bắt đầu gọi Wave
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves; // Reset đếm ngược cho Wave sau
        }
    }

    // Coroutine để xử lý việc sinh quái có độ trễ (delay)
    IEnumerator SpawnWave()
    {
        isSpawning = true;
        Wave currentWave = waves[currentWaveIndex];

        Debug.Log("Bắt đầu " + currentWave.waveName);

        // Duyệt qua từng nhóm quái (WaveElement) trong Wave
        foreach (WaveElement element in currentWave.elements)
        {
            // Sinh từng con quái theo số lượng (count)
            for (int i = 0; i < element.count; i++)
            {
                SpawnEnemy(element.enemyTag);

                // Tạm dừng 1 khoảng thời gian (spawnRate) trước khi sinh con tiếp theo
                yield return new WaitForSeconds(1f / element.spawnRate);
            }
        }

        currentWaveIndex++;
        isSpawning = false; // Kết thúc sinh quái cho đợt này
    }

    private void SpawnEnemy(string tag)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Chưa gán điểm Spawn Point trong WaveManager!");
            return;
        }

        // Gọi quái vật ra từ Object Pool đã thiết lập trước đó
        GameObject enemy = ObjectPool.Instance.SpawnFromPool(tag, spawnPoint.position, spawnPoint.rotation);

        if (enemy != null)
        {
            // Khởi tạo trạng thái và tìm đường A* cho quái
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SpawnSetup();
            }
        }
    }
}