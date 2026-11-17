using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image fillImage;

    [Header("Billboarding")]
    public bool useBillboarding = true;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Hàm cập nhật tỷ lệ máu trên thanh máu
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (fillImage != null)
        {
            if (maxHealth <= 0f)
            {
                fillImage.fillAmount = 0f;
                return;
            }

            fillAmountSafe(Mathf.Clamp01(currentHealth / maxHealth));
        }
    }

    private void fillAmountSafe(float amount)
    {
        fillImage.fillAmount = amount;
    }

    // Billboarding: Giữ cho thanh máu luôn đối diện trực diện với Camera
    private void LateUpdate()
    {
        if (!useBillboarding) return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Với Camera Orthographic hoặc Perspective, cách xoay cùng hướng góc nhìn của Camera
        // sẽ ngăn thanh máu bị lật ngược hoặc méo góc
        transform.rotation = mainCamera.transform.rotation;
    }
}