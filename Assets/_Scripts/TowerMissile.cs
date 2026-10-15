using UnityEngine;

public class TowerMissile : BaseTower
{
    public Transform firePoint;
    public GameObject missilePrefab; // Kéo Prefab Ammo_Missile vào đây
    [Header("Âm thanh súng")]
    private AudioSource shootAudio;

    private void Awake()
    {
        shootAudio = GetComponent<AudioSource>();
    }
    protected override void Shoot()
    {
        // GỌI HÀM CỦA LỚP CHA ĐỂ KÍCH HOẠT ĐẾM NGƯỢC (RẤT QUAN TRỌNG)
        base.Shoot();
        if (target == null || firePoint == null) return;
        // Phát tiếng tên lửa phóng
        if (shootAudio != null)
        {
            shootAudio.Play();
        }
        GameObject ammo = Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
        MissileProjectile proj = ammo.GetComponent<MissileProjectile>();
        if (proj != null)
        {
            proj.Seek(target);
        }
    }
}