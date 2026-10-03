using UnityEngine;

public class WaveSpawnerTest : MonoBehaviour
{
    public Transform spawnPoint; // Kéo điểm Node(0,0) hoặc điểm xuất phát vào đây

    // Bỏ hàm Update chứa lệnh Input lỗi đi
    // Thay bằng ContextMenu để gọi bằng cách click chuột phải
    [ContextMenu("Test Spawn Enemy")]
    public void SpawnTestEnemy()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Bạn chưa gắn Spawn Point vào script WaveSpawnerTest!");
            return;
        }

        // Gọi quái ra từ Object Pool
        GameObject enemy = ObjectPool.Instance.SpawnFromPool("EnemyTest", spawnPoint.position, spawnPoint.rotation);

        if (enemy != null)
        {
            Debug.Log("Đã gọi thành công 1 quái vật ra từ Object Pool!");
        }
    }
}