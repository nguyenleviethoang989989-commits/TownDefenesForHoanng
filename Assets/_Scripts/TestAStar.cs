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
        PathNode startNode = GetNodeFromWorldPosition(spawnPoint.position);
        PathNode endNode = GetNodeFromWorldPosition(basePoint.position);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Điểm Start hoặc End nằm ngoài khu vực Grid!");
            return;
        }

        // 3. Gọi hàm tìm đường
        pathfinding.FindPath(startNode, endNode);

        Debug.Log($"Đã chạy A* từ ({startNode.gridX},{startNode.gridY}) đến ({endNode.gridX},{endNode.gridY})");
    }

    // Hàm chuyển đổi vị trí thực tế trên Scene sang ô (Node) trên Grid
    // Hàm chuyển đổi vị trí thực tế trên Scene sang ô (Node) trên Grid - PHIÊN BẢN CẢI TIẾN
    private PathNode GetNodeFromWorldPosition(Vector3 worldPos)
    {
        // 1. Kiểm tra an toàn
        if (gridManager == null || gridManager.grid == null) return null;

        // 2. Tìm Node có khoảng cách gần nhất với điểm mốc (worldPos)
        PathNode closestNode = null;
        float minDistance = float.MaxValue;

        // Quét toàn bộ Grid để tìm Node gần điểm mốc nhất
        for (int x = 0; x < gridManager.gridSizeX; x++)
        {
            for (int y = 0; y < gridManager.gridSizeY; y++) // Chú ý: dùng gridSizeY (trục dọc của mảng 2D)
            {
                PathNode node = gridManager.grid[x, y];
                if (node != null)
                {
                    // Tính khoảng cách từ Node đến điểm mốc (bỏ qua trục Y độ cao để tránh sai số)
                    Vector3 nodePosFlat = new Vector3(node.worldPosition.x, 0, node.worldPosition.z);
                    Vector3 targetPosFlat = new Vector3(worldPos.x, 0, worldPos.z);

                    float distance = Vector3.Distance(nodePosFlat, targetPosFlat);

                    // Nếu khoảng cách này nhỏ hơn khoảng cách nhỏ nhất từng ghi nhận
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
}