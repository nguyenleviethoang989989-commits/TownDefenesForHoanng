using UnityEngine;

public class TestAStar : MonoBehaviour
{
    public Pathfinding pathfinding;
    public GridManager gridManager;

    public int startX, startY;
    public int endX, endY;

    // Đánh dấu ContextMenu giúp bạn gọi hàm này bằng tay từ giao diện Unity
    [ContextMenu("Test Find Path")]
    public void TestFindPath()
    {
        // Kiểm tra xem đã kết nối biến chưa
        if (pathfinding == null || gridManager == null || gridManager.grid == null)
        {
            Debug.LogError("Chưa kết nối Pathfinding, GridManager hoặc Grid chưa được khởi tạo!");
            return;
        }

        // Lấy Node dựa theo tọa độ nhập
        PathNode startNode = gridManager.grid[startX, startY];
        PathNode endNode = gridManager.grid[endX, endY];

        // Gọi hàm tìm đường
        pathfinding.FindPath(startNode, endNode);

        Debug.Log($"Đã chạy A* từ ({startX},{startY}) đến ({endX},{endY})");
    }
}