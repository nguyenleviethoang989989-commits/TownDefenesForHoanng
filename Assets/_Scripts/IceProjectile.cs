using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    private Transform target;
    public float speed = 15f; // Tốc độ bay của đạn
    public float damage = 5f; // Sát thương nhỏ
    public float slowAmount = 0.5f; // Làm chậm 50%
    public float slowDuration = 2f; // Thời gian hiệu lực 2 giây

    // Tháp sẽ gọi hàm này để truyền mục tiêu cho đạn
    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            // Nếu quái chết trước khi đạn tới, đạn tự biến mất (trả về pool)
            gameObject.SetActive(false);
            return;
        }

        // Đạn bay đuổi theo mục tiêu
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget(); // Đạn chạm mục tiêu
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        EnemyMovement enemy = target.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // Trừ máu
            // (Tính năng làm chậm sẽ được thêm vào EnemyMovement ở bước sau)
            Debug.Log("Đạn Băng trúng " + target.name + " - Sẽ làm chậm trong " + slowDuration + "s!");
        }

        // Chạm xong thì nổ/tắt đi
        gameObject.SetActive(false); // Sẽ kết hợp với Object Pool sau
    }
}
