using UnityEngine;

public class TowerCannon : BaseTower
{
    public Transform firePoint;
    public GameObject cannonPrefab;

    [Header("Âm thanh súng")]
    private AudioSource shootAudio;

    private void Awake()
    {
        // Lấy component Audio Source đã gắn trên tháp
        shootAudio = GetComponent<AudioSource>();
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (target == null || firePoint == null) return;

        // Phát tiếng nổ Cannon
        if (shootAudio != null)
        {
            shootAudio.Play();
        }

        GameObject ammo = Instantiate(cannonPrefab, firePoint.position, firePoint.rotation);
        CannonProjectile proj = ammo.GetComponent<CannonProjectile>();
        if (proj != null)
        {
            proj.Seek(target);
        }
    }
}