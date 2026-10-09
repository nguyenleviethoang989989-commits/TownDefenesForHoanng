using UnityEngine;

public class TowerIce : BaseTower
{
    [Header("Cài đặt Đạn Băng")]
    public Transform firePoint;
    // Tạm thời kéo Prefab vào đây, sau này sẽ dùng ObjectPool.SpawnFromPool("AmmoIce")
    public GameObject iceProjectilePrefab;

    protected override void Shoot()
    {
        if (target == null || firePoint == null) return;

        // Cần có fileRate (Ví dụ 1.5) để tháp không bắn liên tục như súng máy

        Debug.Log(gameObject.name + " bắn Đạn Băng vào: " + target.name);

        // Sinh viên đạn tại FirePoint (Tạm dùng Instantiate để test, bài sau sẽ ráp Object Pool)
        GameObject ammo = Instantiate(iceProjectilePrefab, firePoint.position, firePoint.rotation);

        // Truyền mục tiêu cho viên đạn đuổi theo
        IceProjectile iceProj = ammo.GetComponent<IceProjectile>();
        if (iceProj != null)
        {
            iceProj.Seek(target);
        }
    }
}