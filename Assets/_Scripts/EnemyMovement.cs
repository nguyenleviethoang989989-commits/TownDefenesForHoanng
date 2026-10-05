using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số cơ bản")]
    public float moveSpeed = 3f; // Tốc độ di chuyển của quái

    // Danh sách các điểm cần đi qua (lấy từ A*)
    private List<PathNode> pathNodes;
    private int currentNodeIndex;

    private void Start()
    {
        // Khi quái xuất hiện, lấy đường đi từ hệ thống A*
        GetPathFromAStar();
    }

    private void GetPathFromAStar()
    {
        // Tìm Pathfinding trong Scene (giả sử nó nằm trong AStarSystem hoặc GameManager)
        Pathfinding pathfinding = FindFirstObjectByType<Pathfinding>();
        GridManager gridManager = FindFirstObjectByType<GridManager>();

        if (pathfinding != null && gridManager != null)
        {
            // Tạm thời hardcode tọa độ Start (0,0) và End (9,9) để test.
            // Sau này bạn sẽ lấy Start là điểm Spawn và End là Base theo thiết kế map.
            PathNode startNode = gridManager.grid[0, 3];
            PathNode endNode = gridManager.grid[8, 1];

            // Yêu cầu A* tính toán đường đi
            pathNodes = pathfinding.FindPath(startNode, endNode);
            currentNodeIndex = 0; // Bắt đầu di chuyển từ điểm đầu tiên trong danh sách

            // Đặt vị trí xuất phát cho quái ngay trên điểm Start
            if (pathNodes != null && pathNodes.Count > 0)
            {
                transform.position = pathNodes[0].worldPosition;
            }
        }
        else
        {
            Debug.LogError("Enemy không tìm thấy Pathfinding hoặc GridManager!");
        }
    }

    private void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        // Nếu không có đường đi hoặc đã đi đến điểm cuối cùng
        if (pathNodes == null || currentNodeIndex >= pathNodes.Count)
        {
            return;
        }

        // Lấy vị trí của điểm đến tiếp theo (Nâng trục Y lên một chút để quái không bị chìm dưới đất)
        Vector3 targetPosition = pathNodes[currentNodeIndex].worldPosition;
        targetPosition.y += 0.5f; // Chỉnh con số này tùy theo chiều cao của Model quái vật

        // Di chuyển dần dần từ vị trí hiện tại đến targetPosition
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // (Tùy chọn) Xoay mặt quái vật hướng về phía đang đi
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        // Nếu quái đã đi tới rất gần Node hiện tại (khoảng cách cực nhỏ), chuyển sang Node tiếp theo
        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            currentNodeIndex++;

            // Xử lý khi đi đến Đích (Base)
            if (currentNodeIndex >= pathNodes.Count)
            {
                ReachBase();
            }
        }
    }

    private void ReachBase()
    {
        Debug.Log("Kẻ địch đã lọt vào Căn cứ! Trừ máu Base!");
        // (Logic trừ máu Base và tắt quái sẽ code chi tiết sau)
        gameObject.SetActive(false); // Hoặc Destroy(gameObject) nếu chưa dùng ObjectPool cho Enemy
    }
}