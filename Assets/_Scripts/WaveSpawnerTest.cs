using UnityEngine;

public class WaveSpawnerTest : MonoBehaviour
{
    public Transform spawnPoint;  // Điểm xuất phát
    public Transform targetPoint; // Điểm kết thúc

    private string[] enemyTags = { "EnemyNormal", "EnemyFast", "EnemyTanker", "EnemyBoss" };

    [ContextMenu("Test Spawn Random Enemy")]
    public void SpawnRandomEnemy()
    {
        if (spawnPoint == null)
        {
            GameObject startObj = GameObject.Find("spawnstart");
            if (startObj != null) spawnPoint = startObj.transform;
            else
            {
                Debug.LogError("Chưa gán điểm Spawn Point!");
                return;
            }
        }

        if (targetPoint == null)
        {
            GameObject endObj = GameObject.Find("spawnend");
            if (endObj != null) targetPoint = endObj.transform;
        }

        int randomIndex = Random.Range(0, enemyTags.Length);
        string selectedTag = enemyTags[randomIndex];

        GameObject enemy = ObjectPool.Instance.SpawnFromPool(selectedTag, spawnPoint.position, spawnPoint.rotation);

        if (enemy != null)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SpawnSetup(spawnPoint, targetPoint);
            }

            Debug.Log("Đã gọi thành công: " + selectedTag);
        }
    }
}