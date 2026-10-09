using UnityEngine;

public class TowerMissile : BaseTower
{
    public Transform firePoint;
    public float explosionRadius = 2f; // Bán kính nổ

    protected override void Shoot()
    {
        if (target == null || firePoint == null) return;

        Debug.Log(gameObject.name + " phóng Tên lửa! (Sẽ nổ diện rộng " + explosionRadius + "m)");
        // Sau này: Gọi tên lửa từ Pool, tên lửa chạm đất sẽ dùng Physics.OverlapSphere để trừ máu diện rộng
    }
}