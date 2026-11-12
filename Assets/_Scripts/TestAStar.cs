using UnityEngine;

public class TestAStar : MonoBehaviour
{
    public Pathfinding pathfinding;
    public GridManager gridManager;

    [Header("Các điểm Mốc trên Map")]
    public Transform spawnPoint; // Điểm bắt đầu (Kéo thả từ Hierarchy vào đây)
    public Transform basePoint;  // Điểm kết thúc (Căn cứ)

    // Đánh dấu ContextMenu giúp bạn gọi hàm này bằng tay từ giao diện Unity
    [ContextMenu("Test Find Path")]
    public void TestFindPath()
    {
        // 1. Kiểm tra xem đã kết nối biến chưa
        if (pathfinding == null || gridManager == null || gridManager.grid == null)
        {
            Debug.LogError("Chưa kết nối Pathfinding, GridManager hoặc Grid chưa được khởi tạo!");
            return;
        }

        if (spawnPoint == null || basePoint == null)
        {
            Debug.LogError("Chưa gắn điểm Start hoặc End!");
            return;
        }

        // 2. Chuyển đổi tọa độ không gian (World Position) của 2 điểm mốc sang chỉ số Grid (X, Y)
        PathNode startNode = gridManager.GetNodeFromWorldPosition(spawnPoint.position);
        PathNode endNode = gridManager.GetNodeFromWorldPosition(basePoint.position);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Điểm Start hoặc End nằm ngoài khu vực Grid!");
            return;
        }

        // 3. Gọi hàm tìm đường
        pathfinding.FindPath(startNode, endNode);

        Debug.Log($"Đã chạy A* từ ({startNode.gridX},{startNode.gridY}) đến ({endNode.gridX},{endNode.gridY})");
    }
}