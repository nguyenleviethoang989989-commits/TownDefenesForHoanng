using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    // Bổ sung tham chiếu đến GridManager để lấy dữ liệu bản đồ
    public GridManager gridManager;
    [Header("Debug")]
    public LineRenderer pathRenderer; // Kéo PathDebugger từ Hierarchy vào đây
    // Hàm Heuristic: Tính chi phí khoảng cách H Cost giữa 2 Node
    public int GetHeuristicDistance(PathNode nodeA, PathNode nodeB)
    {
        // Tính độ lệch tuyệt đối (không lấy số âm) trên trục X và Y
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // Trả về tổng khoảng cách. (Nhân với 10 để tránh số thập phân, chuẩn mực chung của A*)
        return 10 * (distanceX + distanceY);
    }

    // --- PHẦN 2: THUẬT TOÁN A* TÌM ĐƯỜNG ---

    public List<PathNode> FindPath(PathNode startNode, PathNode targetNode)
    {
        // Open List: Chứa các Node đang chờ được đánh giá (Dùng List để linh hoạt tìm F Cost thấp nhất)
        List<PathNode> openList = new List<PathNode>();

        // Closed List: Chứa các Node đã đánh giá xong, không quay lại nữa (Dùng HashSet để tối ưu tốc độ kiểm tra)
        HashSet<PathNode> closedList = new HashSet<PathNode>();

        // Bước 1: Đưa điểm xuất phát vào Open List
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Bước 2: Tìm Node có F Cost thấp nhất trong Open List
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                // Nếu F Cost bằng nhau, ưu tiên Node có H Cost (gần đích hơn) thấp hơn
                if (openList[i].fCost < currentNode.fCost ||
                   (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            // Bước 3: Chuyển Node hiện tại từ Open List sang Closed List
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Bước 4: Kiểm tra xem đã đến đích chưa
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode); // Trả về danh sách đường đi
            }

            // Bước 5: Duyệt qua các Node hàng xóm xung quanh Node hiện tại (Lên, Xuống, Trái, Phải)
            foreach (PathNode neighbor in gridManager.GetNeighbors(currentNode))
            {
                // Bỏ qua nếu hàng xóm là vật cản (đã xây tháp) hoặc đã nằm trong Closed List
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                // Tính chi phí G Cost mới nếu đi qua Node hiện tại
                int newMovementCostToNeighbor = currentNode.gCost + GetHeuristicDistance(currentNode, neighbor);

                // Nếu đường đi này ngắn hơn, HOẶC hàng xóm chưa từng được đưa vào Open List
                if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                {
                    // Cập nhật lại các chỉ số
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetHeuristicDistance(neighbor, targetNode);

                    // Lưu lại Node cha để lát nữa truy vết ngược lại đường đi
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // Vòng lặp kết thúc mà không return nghĩa là mọi đường đều bị chặn
        Debug.LogWarning("Không tìm thấy đường đi tới đích!");
        return null;
    }

    // Hàm truy vết đường đi từ Đích ngược về Bắt đầu
    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        // Lần ngược từ đích theo dấu vết 'parent' cho đến khi về tới Start Node
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Đảo ngược danh sách để có đường đi đúng thứ tự: Start -> End
        path.Reverse();
        // Gọi hàm vẽ đường đi để Debug
        DrawPath(path);
        return path;
    }

    // Thêm hàm vẽ đường đi này vào cuối class
    public void DrawPath(List<PathNode> path)
    {
        if (pathRenderer == null)
        {
            Debug.LogWarning("Chưa gán LineRenderer vào script Pathfinding!");
            return;
        }

        if (path == null || path.Count == 0)
        {
            // Nếu không có đường đi, xóa đường vẽ cũ
            pathRenderer.positionCount = 0;
            return;
        }

        // Thiết lập số lượng điểm neo của LineRenderer bằng với số lượng Node tìm được
        pathRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            // Lấy tọa độ 3D của Node
            Vector3 nodePosition = path[i].worldPosition;

            // Nâng trục Y lên một chút (ví dụ: +0.5f) để đường line không bị chìm dưới mặt đất
            nodePosition.y += 0.5f;

            // Gán tọa độ vào điểm neo thứ i của LineRenderer
            pathRenderer.SetPosition(i, nodePosition);
        }
    }
}