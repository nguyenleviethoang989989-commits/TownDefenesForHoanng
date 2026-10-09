using UnityEngine;

public class TowerCannon : BaseTower
{
    public Transform firePoint; // Điểm bắn trên nòng súng

    protected override void Shoot()
    {
        if (target == null || firePoint == null) return;

        Debug.Log(gameObject.name + " bắn đạn Cannon vào: " + target.name);
        // Sau này: ObjectPool.Instance.SpawnFromPool("Đạn Cannon", firePoint.position, firePoint.rotation);
    }
}