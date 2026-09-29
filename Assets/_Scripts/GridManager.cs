using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f; // Kích thước của mỗi khối 3D Kenney thường là 1 unit

    [Header("Prefabs")]
    public GameObject nodePrefab; // Kéo Prefab ô đất vào đây

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Tính toán vị trí trong không gian 3D (Trục Y giữ nguyên là 0)
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);

                // Tạo ô đất
                GameObject newNode = Instantiate(nodePrefab, position, Quaternion.identity);
                newNode.name = $"Node ({x}, {z})";

                // Gom gọn các ô đất vào làm con của GridManager để Hierarchy không bị rối
                newNode.transform.SetParent(transform);
            }
        }
    }
}