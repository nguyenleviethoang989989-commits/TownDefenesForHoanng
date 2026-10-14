using UnityEngine;

public class TowerMissile : BaseTower
{
    public Transform firePoint;
    public GameObject missilePrefab; // Kéo Prefab Ammo_Missile vào đây

    protected override void Shoot()
    {
        // GỌI HÀM CỦA LỚP CHA ĐỂ KÍCH HOẠT ĐẾM NGƯỢC (RẤT QUAN TRỌNG)
        base.Shoot();
        if (target == null || firePoint == null) return;

        GameObject ammo = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        MissileProjectile proj = ammo.GetComponent<MissileProjectile>();
        if (proj != null)
        {
            proj.Seek(target);
        }
    }
}