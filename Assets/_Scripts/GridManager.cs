using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public GameObject nodePrefab;

    [Header("Cài đặt kích thước A*")]
    public float nodeRadius = 0.5f;
    public int gridSizeX = 10;
    public int gridSizeY = 10;

    // Layer chứa các khối Đường đi (PathMap) hoặc vật cản
    public LayerMask unwalkableMask;

    // Mảng 2D chứa dữ liệu thuật toán
    public PathNode[,] grid;

    private void Awake()
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

    // Hàm quét và nạp dữ liệu cho mảng 2D
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

    // Hàm chuyển đổi vị trí thực tế trên Scene (Vector3) sang Node gần nhất trên Grid
    public PathNode GetNodeFromWorldPosition(Vector3 worldPos)
    {
        if (grid == null) return null;

        PathNode closestNode = null;
        float minDistance = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PathNode node = grid[x, y];
                if (node != null)
                {
                    Vector3 nodePosFlat = new Vector3(node.worldPosition.x, 0, node.worldPosition.z);
                    Vector3 targetPosFlat = new Vector3(worldPos.x, 0, worldPos.z);

                    float distance = Vector3.Distance(nodePosFlat, targetPosFlat);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestNode = node;
                    }
                }
            }
        }

        return closestNode;
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