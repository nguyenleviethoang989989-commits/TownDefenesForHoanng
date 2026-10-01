using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public GameObject nodePrefab;

    // Layer chứa các khối Đường đi (PathMap)
    public LayerMask unwalkableMask;

    // Mảng 2D chứa dữ liệu thuật toán
    public PathNode[,] grid;

    private void Start()
    {
        GenerateGrid();
        CreatePathNodeData();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject newNode = Instantiate(nodePrefab, position, Quaternion.identity);
                newNode.name = $"Node ({x}, {z})";
                newNode.transform.SetParent(transform);
            }
        }
    }

    // Hàm mới: Quét và nạp dữ liệu cho mảng 2D
    public void CreatePathNodeData()
    {
        grid = new PathNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPoint = new Vector3(x * cellSize, 0, y * cellSize);

                // Bắn tia Sphere để kiểm tra xem vị trí này có bị đè bởi Đường đi (unwalkableMask) hay không
                bool walkable = !Physics.CheckSphere(worldPoint, cellSize / 2, unwalkableMask);

                // Lưu vào mảng
                grid[x, y] = new PathNode(walkable, worldPoint, x, y);
            }
        }
    }

    // Hàm lấy 4 hàng xóm xung quanh một Node
    public List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int checkX = node.gridX + dx[i];
            int checkY = node.gridY + dy[i];

            // Đảm bảo hàng xóm nằm trong ranh giới bản đồ
            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }

        return neighbors;
    }
}