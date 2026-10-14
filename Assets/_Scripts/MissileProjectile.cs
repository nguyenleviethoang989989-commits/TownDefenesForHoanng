using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    private Transform target;
    public float speed = 10f; // Bay chậm hơn Cannon
    public float explosionRadius = 3f; // Bán kính nổ
    public float damage = 40f; // Sát thương lớn
    public LayerMask enemyMask; // Lọc layer Enemy

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Explode(); // Tên lửa mất mục tiêu giữa chừng thì vẫn nổ tại chỗ
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            Explode();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void Explode()
    {
        // Quét tất cả quái vật trong bán kính nổ
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);
        foreach (Collider col in colliders)
        {
            EnemyMovement enemy = col.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        gameObject.SetActive(false); // Trả về pool
    }

    // Vẽ vòng tròn đỏ trong Scene để dễ cân chỉnh bán kính nổ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}