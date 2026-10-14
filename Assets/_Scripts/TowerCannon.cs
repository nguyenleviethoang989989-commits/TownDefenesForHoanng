using UnityEngine;

public class TowerCannon : BaseTower
{
    public Transform firePoint;
    public GameObject cannonPrefab; // Kéo Prefab Ammo_Cannon vào đây

    protected override void Shoot()
    {
        // GỌI HÀM CỦA LỚP CHA ĐỂ KÍCH HOẠT ĐẾM NGƯỢC (RẤT QUAN TRỌNG)
        base.Shoot();
        if (target == null || firePoint == null) return;

        GameObject ammo = Instantiate(cannonPrefab, firePoint.position, firePoint.rotation);
        CannonProjectile proj = ammo.GetComponent<CannonProjectile>();
        if (proj != null)
        {
            proj.Seek(target);
        }
    }
}