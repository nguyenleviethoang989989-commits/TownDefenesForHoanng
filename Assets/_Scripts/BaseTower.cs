using UnityEngine;

public class BaseTower : MonoBehaviour
{
    // Định nghĩa State Machine cơ bản theo yêu cầu GDD
    public enum TowerState { Idle, Tracking, Shooting }
    public TowerState currentState = TowerState.Idle;

    [Header("Cấp độ & Kinh tế")]
    public int towerLevel = 1;
    public int upgradeCost = 50;
    public int sellValue = 15;

    [Header("Thông số cơ bản")]
    public float range = 3f;      // Tầm bắn của tháp
    public float fireRate = 1f;   // Tốc độ bắn (số viên đạn/giây)
    private float fireCountdown = 0f; // Bộ đếm ngược thời gian bắn

    [Header("Chỉ số sau khi Nâng cấp")]
    public float upgradedRange = 4.5f;
    public float upgradedFireRate = 1.8f;

    [Header("Nhận diện mục tiêu")]
    public LayerMask enemyMask;   // Chọn Layer "Enemy" ở Inspector
    protected Transform target;     // Mục tiêu hiện tại đang khóa

    [Header("Trực quan (Tùy chọn)")]
    public Transform partToRotate; // Kéo khối nòng súng vào đây để nó xoay theo quái

    protected virtual void Update()
    {
        // Đếm ngược thời gian nạp đạn LIÊN TỤC mỗi frame
        if (fireCountdown > 0)
        {
            fireCountdown -= Time.deltaTime;
        }

        switch (currentState)
        {
            case TowerState.Idle:
                FindTarget();
                break;

            case TowerState.Tracking:
                TrackTarget();
                // CHỈ CHO PHÉP CHUYỂN SANG SHOOTING KHI ĐÃ ĐẾM NGƯỢC XONG
                if (target != null && fireCountdown <= 0f)
                {
                    currentState = TowerState.Shooting;
                }
                break;

            case TowerState.Shooting:
                Shoot(); // Khi bắn xong, hàm Shoot sẽ reset fireCountdown = 1f/fireRate
                currentState = TowerState.Tracking;
                break;
        }
    }

    // --- BẮT ĐẦU PHẦN OBSERVER PATTERN ---
    protected virtual void OnEnable()
    {
        // Khi Tháp được bật, đăng ký lắng nghe sự kiện quái chết
        EnemyMovement.OnEnemyRemoved += HandleEnemyRemoved;
    }

    protected virtual void OnDisable()
    {
        // Khi Tháp bị tắt/xóa, BẮT BUỘC phải hủy lắng nghe
        EnemyMovement.OnEnemyRemoved -= HandleEnemyRemoved;
    }

    // Hàm này sẽ tự động chạy mỗi khi có BẤT KỲ con quái nào chết hoặc lọt vào base
    private void HandleEnemyRemoved(GameObject removedEnemy)
    {
        // Kiểm tra xem con quái vừa chết có đúng là mục tiêu tháp đang ngắm không?
        if (target != null && target.gameObject == removedEnemy)
        {
            // Nếu đúng, lập tức reset mục tiêu và quay về trạng thái quét tìm mồi mới
            target = null;
            currentState = TowerState.Idle;
        }
    }
    // --- KẾT THÚC PHẦN OBSERVER PATTERN ---

    // Hàm quét mục tiêu bằng Physics.OverlapSphere
    private void FindTarget()
    {
        // Tạo 1 hình cầu tàng hình quanh tháp, thu thập mọi vật chạm vào thuộc Layer Enemy
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, enemyMask);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Lọc trong danh sách các quái lọt vào tầm, tìm con nào gần nhất
        foreach (Collider col in colliders)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, col.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = col.gameObject;
            }
        }

        // Nếu tìm thấy và quái đó thực sự trong tầm bắn
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            currentState = TowerState.Tracking; // Đổi trạng thái sang Bám sát
        }
        else
        {
            target = null;
        }
    }

    // Hàm bám theo mục tiêu (Xoay nòng súng)
    private void TrackTarget()
    {
        // Kiểm tra xem quái đã chết (bị cất vào pool), hoặc chạy ra khỏi tầm bắn chưa
        if (target == null || Vector3.Distance(transform.position, target.position) > range)
        {
            target = null;
            currentState = TowerState.Idle; // Mất dấu -> Trở về trạng thái nghỉ tìm mồi mới
            return;
        }

        // Nếu có khai báo partToRotate, xoay từ từ nòng súng về phía quái
        if (partToRotate != null)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            // Lerp giúp xoay mượt mà hơn
            Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRot, Time.deltaTime * 10f).eulerAngles;
            partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }

    // Hàm bắn (Sẽ được viết đè ở các loại tháp Cannon, Laser khác nhau)
    protected virtual void Shoot()
    {
        Debug.Log(gameObject.name + " bắn mục tiêu: " + target.name);
        fireCountdown = 1f / fireRate; // Nạp lại đạn (Ví dụ fireRate = 2 thì mất 0.5s để bắn viên tiếp theo)
    }

    // Nâng cấp tháp
    public virtual void UpgradeTower()
    {
        if (towerLevel < 2)
        {
            towerLevel++;
            range = upgradedRange;
            fireRate = upgradedFireRate;
            sellValue += upgradeCost / 2; // Tăng giá trị bán lại khi nâng cấp

            // Làm tháp to lên theo yêu cầu
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

            Debug.Log(gameObject.name + " đã được nâng cấp lên Level " + towerLevel + "!");
        }
        else
        {
            Debug.Log(gameObject.name + " đã đạt cấp độ tối đa!");
        }
    }

    // Bán tháp
    public virtual void SellTower()
    {
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.AddMoney(sellValue);
        }

        Debug.Log(gameObject.name + " đã được bán với giá " + sellValue + " Vàng!");
        Destroy(gameObject);
    }

    // Hàm này giúp vẽ một vòng tròn màu đỏ bao quanh tháp trong Scene để bạn dễ hình dung Tầm Bắn
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}