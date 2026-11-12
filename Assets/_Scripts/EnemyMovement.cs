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

    [Header("Điểm mốc di chuyển (Tùy chọn)")]
    public Transform customStartPoint;
    public Transform customTargetPoint;

    private List<PathNode> pathNodes;
    private int currentNodeIndex;

    // Biến để lưu kết nối tránh phải tìm lại nhiều lần
    private Pathfinding pathfinding;
    private GridManager gridManager;

    [Header("Âm thanh")]
    private AudioSource spawnAudio; // Biến âm thanh

    // 1. KHAI BÁO SỰ KIỆN OBSERVER TOÀN CỤC
    public static event Action<GameObject> OnEnemyRemoved;

    private void Awake()
    {
        // Tìm 2 hệ thống này ngay khi game bắt đầu
        pathfinding = FindFirstObjectByType<Pathfinding>();
        gridManager = FindFirstObjectByType<GridManager>();
        spawnAudio = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        currentHP = maxHP;
    }

    private void Start()
    {
        // Dành cho việc test quái đặt sẵn trên Scene bằng tay
        if (pathNodes == null || pathNodes.Count == 0)
        {
            GetPathFromAStar(customStartPoint, customTargetPoint);
        }
    }

    // Object Pool hoặc WaveManager sẽ gọi hàm này sau khi lấy quái từ kho ra
    public void SpawnSetup(Transform start = null, Transform target = null)
    {
        currentHP = maxHP;

        // Nếu truyền tham số vào thì dùng, không thì dùng custom/tự tìm
        Transform s = start != null ? start : customStartPoint;
        Transform t = target != null ? target : customTargetPoint;

        GetPathFromAStar(s, t);

        // Kích hoạt tiếng bước chân/spawn
        if (spawnAudio != null)
        {
            spawnAudio.Play();
        }
    }

    public void GetPathFromAStar(Transform startPoint = null, Transform targetPoint = null)
    {
        if (pathfinding == null) pathfinding = FindFirstObjectByType<Pathfinding>();
        if (gridManager == null) gridManager = FindFirstObjectByType<GridManager>();

        if (pathfinding != null && gridManager != null && gridManager.grid != null)
        {
            // 1. Xác định vị trí xuất phát
            Vector3 startPos = startPoint != null ? startPoint.position : transform.position;

            // 2. Xác định vị trí đích đến
            Vector3 endPos;
            if (targetPoint != null)
            {
                endPos = targetPoint.position;
            }
            else if (WaveManager.Instance != null && WaveManager.Instance.targetPoint != null)
            {
                endPos = WaveManager.Instance.targetPoint.position;
            }
            else
            {
                // Thử tìm object spawnend trong Scene
                GameObject endObj = GameObject.Find("spawnend");
                if (endObj != null)
                {
                    endPos = endObj.transform.position;
                }
                else
                {
                    // Dự phòng nếu không tìm thấy
                    endPos = gridManager.grid[8, 1].worldPosition;
                }
            }

            // 3. Quy đổi sang PathNode trên Grid động theo vị trí thực tế
            PathNode startNode = gridManager.GetNodeFromWorldPosition(startPos);
            PathNode endNode = gridManager.GetNodeFromWorldPosition(endPos);

            if (startNode != null && endNode != null)
            {
                pathNodes = pathfinding.FindPath(startNode, endNode);
                currentNodeIndex = 0;

                if (pathNodes != null && pathNodes.Count > 0)
                {
                    Vector3 initPos = pathNodes[0].worldPosition;
                    initPos.y += yOffset;
                    transform.position = initPos;
                }
            }
            else
            {
                Debug.LogError(gameObject.name + ": Điểm Start hoặc End nằm ngoài phạm vi Grid!");
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage(damageToBase);
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager trong Scene!");
        }

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
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.AddMoney(rewardMoney);
        }
        OnEnemyRemoved?.Invoke(gameObject);
        gameObject.SetActive(false);
    }
}