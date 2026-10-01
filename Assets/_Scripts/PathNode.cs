using UnityEngine;

public class PathNode
{
    // Tọa độ trên lưới mảng 2D
    public int gridX;
    public int gridY;

    // Vị trí thực tế trong không gian 3D
    public Vector3 worldPosition;

    // Đánh dấu ô này quái vật có đi qua được không (false nếu là vị trí đặt tháp hoặc vật cản)
    public bool isWalkable;

    // --- CÁC CHỈ SỐ CHO THUẬT TOÁN A* ---

    // G Cost: Chi phí khoảng cách từ điểm Bắt đầu (Start) đến Node hiện tại
    public int gCost;

    // H Cost (Heuristic): Chi phí ước tính từ Node hiện tại đến điểm Đích (End)
    public int hCost;

    // Node cha: Dùng để truy vết ngược lại đường đi ngắn nhất sau khi tìm thấy Đích
    public PathNode parent;

    // F Cost: Tổng chi phí (A* sẽ luôn chọn Node có fCost nhỏ nhất để đi tiếp)
    public int fCost
    {
        get { return gCost + hCost; }
    }

    // Hàm khởi tạo (Constructor)
    public PathNode(bool _isWalkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}