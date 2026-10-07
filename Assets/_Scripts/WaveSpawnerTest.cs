using UnityEngine;

public class WaveSpawnerTest : MonoBehaviour
{
    public Transform spawnPoint; // Điểm xuất phát

    // Danh sách các tag phải khớp chính xác với Tag đã đặt trong ObjectPoolManager
    private string[] enemyTags = { "EnemyNormal", "EnemyFast", "EnemyTanker", "EnemyBoss" };

    [ContextMenu("Test Spawn Random Enemy")]
    public void SpawnRandomEnemy()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Chưa gán điểm Spawn Point!");
            return;
        }

        // Chọn ngẫu nhiên 1 trong 4 loại quái
        int randomIndex = Random.Range(0, enemyTags.Length);
        string selectedTag = enemyTags[randomIndex];

        // Lấy quái từ Pool ra
        GameObject enemy = ObjectPool.Instance.SpawnFromPool(selectedTag, spawnPoint.position, spawnPoint.rotation);

        if (enemy != null)
        {
            // BẮT BUỘC: Gọi hàm Setup để quái làm mới lại HP và tìm đường A*
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SpawnSetup();
            }

            Debug.Log("Đã gọi thành công: " + selectedTag);
        }
    }
}