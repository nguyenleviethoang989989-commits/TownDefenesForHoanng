using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Tham chiếu Hệ thống Lưới & Tìm đường")]
    public GridManager gridManager;
    public Pathfinding pathfinding;
    public Transform spawnPoint; // Điểm Start
    public Transform basePoint;  // Điểm Base (End)

    [Header("Danh sách Prefab Cảnh quan (RPGPP_LT)")]
    public List<GameObject> treePrefabs = new List<GameObject>();
    public List<GameObject> rockPrefabs = new List<GameObject>();
    public List<GameObject> grassPrefabs = new List<GameObject>();
    public List<GameObject> flowerPrefabs = new List<GameObject>();

    [Header("Tỉ lệ sinh cảnh quan (0 -> 1)")]
    [Range(0f, 1f)] public float treeSpawnRate = 0.15f;
    [Range(0f, 1f)] public float rockSpawnRate = 0.12f;
    [Range(0f, 1f)] public float grassSpawnRate = 0.25f;

    [Header("Kích thước tùy chỉnh theo từng loại (phù hợp với Enemy)")]
    public Vector2 treeScaleRange = new Vector2(0.25f, 0.35f);
    public Vector2 rockScaleRange = new Vector2(0.3f, 0.45f);
    public Vector2 grassScaleRange = new Vector2(0.35f, 0.5f);

    [Header("Cài đặt sinh ngẫu nhiên")]
    public float yOffset = 0f;
    public Transform environmentParent;

    [Header("Tự động sinh khi bắt đầu game")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            GenerateEnvironment();
        }
    }

    [ContextMenu("Tạo Cảnh Quan (Generate Environment)")]
    public void GenerateEnvironment()
    {
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();

        if (gridManager == null || gridManager.grid == null)
        {
            Debug.LogError("Chưa tìm thấy GridManager hoặc Grid chưa được tạo!");
            return;
        }

        if (spawnPoint == null)
        {
            GameObject startObj = GameObject.Find("spawnstart");
            if (startObj != null) spawnPoint = startObj.transform;
        }
        if (basePoint == null)
        {
            GameObject endObj = GameObject.Find("spawnend");
            if (endObj != null) basePoint = endObj.transform;
        }

        // 1. Tìm danh sách các ô thuộc đường đi A* từ Start đến Base
        HashSet<PathNode> pathNodeSet = new HashSet<PathNode>();
        if (pathfinding != null && spawnPoint != null && basePoint != null)
        {
            PathNode startNode = gridManager.GetNodeFromWorldPosition(spawnPoint.position);
            PathNode endNode = gridManager.GetNodeFromWorldPosition(basePoint.position);

            if (startNode != null && endNode != null)
            {
                List<PathNode> currentPath = pathfinding.FindPath(startNode, endNode);
                if (currentPath != null)
                {
                    foreach (PathNode node in currentPath)
                    {
                        pathNodeSet.Add(node);
                    }
                }
            }
        }

        // 2. Dọn dẹp cảnh quan cũ nếu có
        ClearEnvironment();

        if (environmentParent == null)
        {
            GameObject envObj = GameObject.Find("EnvironmentContainer");
            if (envObj == null) envObj = new GameObject("EnvironmentContainer");
            environmentParent = envObj.transform;
        }

        // 3. Quét từng ô trên bản đồ
        int spawnedCount = 0;
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                PathNode node = gridManager.grid[x, y];
                if (node == null) continue;

                // TUYỆT ĐỐI BỎ QUA nếu:
                // - Ô nằm trên lối đi A* đã tính
                // - Hoặc ô là unwalkable (được xếp khối Path Prefab bên trên)
                if (pathNodeSet.Contains(node) || !node.isWalkable)
                {
                    continue;
                }

                // Kiểm tra xem ô đất này đã bị xây tháp chưa
                Vector3 nodeWorldPos = node.worldPosition;
                Collider[] colliders = Physics.OverlapSphere(nodeWorldPos, gridManager.cellSize * 0.4f);
                bool hasObstacleOrTower = false;
                foreach (Collider col in colliders)
                {
                    if (col.GetComponent<BaseTower>() != null || col.GetComponentInParent<BaseTower>() != null)
                    {
                        hasObstacleOrTower = true;
                        break;
                    }
                }
                if (hasObstacleOrTower) continue;

                // Thử sinh Cây (Tree)
                float roll = Random.value;
                if (roll < treeSpawnRate && treePrefabs.Count > 0)
                {
                    SpawnDecor(GetRandomPrefab(treePrefabs), nodeWorldPos, treeScaleRange);
                    spawnedCount++;
                    continue;
                }
                roll -= treeSpawnRate;

                // Thử sinh Đá (Rock)
                if (roll < rockSpawnRate && rockPrefabs.Count > 0)
                {
                    SpawnDecor(GetRandomPrefab(rockPrefabs), nodeWorldPos, rockScaleRange);
                    spawnedCount++;
                    continue;
                }
                roll -= rockSpawnRate;

                // Thử sinh Cỏ / Hoa (Grass / Flowers)
                if (roll < grassSpawnRate && (grassPrefabs.Count > 0 || flowerPrefabs.Count > 0))
                {
                    List<GameObject> foliageList = grassPrefabs.Count > 0 ? grassPrefabs : flowerPrefabs;
                    SpawnDecor(GetRandomPrefab(foliageList), nodeWorldPos, grassScaleRange);
                    spawnedCount++;
                }
            }
        }

        Debug.Log($"<color=green>Đã sinh thành công {spawnedCount} đối tượng cảnh quan tự động xung quanh đường đi!</color>");
    }

    private void SpawnDecor(GameObject prefab, Vector3 basePos, Vector2 scaleMinMax)
    {
        if (prefab == null) return;

        // Thêm độ lệch ngẫu nhiên nhẹ bên trong ô
        float offsetX = Random.Range(-0.15f, 0.15f);
        float offsetZ = Random.Range(-0.15f, 0.15f);
        Vector3 spawnPos = new Vector3(basePos.x + offsetX, basePos.y + yOffset, basePos.z + offsetZ);

        // Xoay ngẫu nhiên quanh trục Y
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        GameObject instance = Instantiate(prefab, spawnPos, randomRotation, environmentParent);

        // Kích thước ngẫu nhiên hài hòa với Enemy
        float scale = Random.Range(scaleMinMax.x, scaleMinMax.y);
        instance.transform.localScale = Vector3.one * scale;
    }

    private GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    [ContextMenu("Xóa Cảnh Quan (Clear Environment)")]
    public void ClearEnvironment()
    {
        if (environmentParent != null)
        {
            for (int i = environmentParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(environmentParent.GetChild(i).gameObject);
            }
        }
        else
        {
            GameObject envObj = GameObject.Find("EnvironmentContainer");
            if (envObj != null)
            {
                DestroyImmediate(envObj);
            }
        }
    }
}