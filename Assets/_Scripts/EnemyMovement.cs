using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chỉ số cơ bản")]
    public float maxHP = 100f;
    public float currentHP;
    public float moveSpeed = 3f;
    public int damageToBase = 1;
    public int rewardMoney = 10;
    public float yOffset = 0.5f;


    private List<PathNode> pathNodes;
    private int currentNodeIndex;

    // Biến để lưu kết nối tránh phải tìm lại nhiều lần
    private Pathfinding pathfinding;
    private GridManager gridManager;


    [Header("Âm thanh")]
    private AudioSource spawnAudio; // Biến âm thanh
    // 1. KHAI BÁO SỰ KIỆN OBSERVER TOÀN CỤC
    // Sự kiện này sẽ truyền đi GameObject của con quái vừa biến mất
    public static event Action<GameObject> OnEnemyRemoved;
    private void Awake()
    {
        // Tìm 2 hệ thống này ngay khi game bắt đầu
        pathfinding = FindFirstObjectByType<Pathfinding>();
        gridManager = FindFirstObjectByType<GridManager>();
        // Lấy component AudioSource đã gắn trên con quái
        spawnAudio = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        currentHP = maxHP;

        // CHÚ Ý: KHÔNG gọi GetPathFromAStar() ở đây nữa nếu quái được đặt sẵn trên Scene
        // Object Pool sẽ tự gọi hàm SpawnSetup() sau khi bật quái lên.
    }

    private void Start()
    {
        // Hàm Start chạy sau Awake và OnEnable. Lúc này Grid chắc chắn đã vẽ xong.
        // Dành cho việc bạn test thả quái trực tiếp trên Scene bằng tay:
        GetPathFromAStar();
    }

    // Object Pool sẽ gọi hàm này sau khi lấy quái từ kho ra
    public void SpawnSetup()
    {
        currentHP = maxHP;
        GetPathFromAStar();

        // Kích hoạt tiếng bước chân/spawn
        if (spawnAudio != null)
        {
            spawnAudio.Play();
        }
    }

    private void GetPathFromAStar()
    {
        if (pathfinding != null && gridManager != null && gridManager.grid != null)
        {
            // Tọa độ Start (0,3) và End (8,1) theo hình ảnh map của bạn
            PathNode startNode = gridManager.grid[0, 3];
            PathNode endNode = gridManager.grid[8, 1];

            pathNodes = pathfinding.FindPath(startNode, endNode);
            currentNodeIndex = 0;

            if (pathNodes != null && pathNodes.Count > 0)
            {
                transform.position = pathNodes[0].worldPosition;
            }
        }
        else
        {
            Debug.LogError(gameObject.name + ": Không tìm thấy Hệ thống A* hoặc Grid chưa được tạo!");
        }
    }

    protected virtual void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (pathNodes == null || currentNodeIndex >= pathNodes.Count)
        {
            return;
        }

        Vector3 targetPosition = pathNodes[currentNodeIndex].worldPosition;
        targetPosition.y += yOffset;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            currentNodeIndex++;

            if (currentNodeIndex >= pathNodes.Count)
            {
                ReachBase();
            }
        }
    }

    protected virtual void ReachBase()
    {
        Debug.Log(gameObject.name + " đã lọt vào Căn cứ! Trừ máu Base!");
        // 2. PHÁT SỰ KIỆN TRƯỚC KHI TẮT QUÁI
        OnEnemyRemoved?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " đã bị tiêu diệt! Nhận " + rewardMoney + " tiền.");
        // 2. PHÁT SỰ KIỆN TRƯỚC KHI TẮT QUÁI
        OnEnemyRemoved?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}