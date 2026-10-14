using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    private Transform target;
    public float speed = 20f; // Bay nhanh
    public float damage = 20f;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false); // Trả về pool nếu quái chết trước khi đạn tới
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
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
            enemy.TakeDamage(damage);
        }

        gameObject.SetActive(false); // Trả về pool
    }
}