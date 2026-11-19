using UnityEngine;
using TMPro;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Danh sách Tháp")]
    public GameObject cannonPrefab;
    public GameObject missilePrefab;
    public GameObject icePrefab;

    [Header("Giá tiền xây Tháp")]
    public int cannonCost = 20;
    public int missileCost = 30;
    public int iceCost = 50;

    [Header("Giao diện UI")]
    public GameObject buildUI;     // Bảng chọn loại tháp để xây
    public GameObject upgradeUI;   // Bảng Nâng cấp / Bán tháp
    public TextMeshProUGUI upgradeButtonText; // Text trên nút Nâng cấp
    public TextMeshProUGUI sellButtonText;    // Text trên nút Bán
    public TextMeshProUGUI cannonButtonText;  // Text trên nút Tháp Cannon
    public TextMeshProUGUI missileButtonText; // Text trên nút Tháp Missile
    public TextMeshProUGUI iceButtonText;     // Text trên nút Tháp Ice

    private Node selectedNode;              // Lưu trữ Node đang được chọn
    private BaseTower selectedTowerToUpgrade; // Lưu trữ tháp trên Node đang được chọn

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        FindUpgradeUITexts();
        FindBuildUITexts();
    }

    private void FindBuildUITexts()
    {
        if (buildUI != null)
        {
            if (cannonButtonText == null)
            {
                Transform btn = buildUI.transform.Find("Tháp Cannon");
                if (btn != null) cannonButtonText = btn.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (missileButtonText == null)
            {
                Transform btn = buildUI.transform.Find("Tháp Missile");
                if (btn != null) missileButtonText = btn.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (iceButtonText == null)
            {
                Transform btn = buildUI.transform.Find("Tháp Ice");
                if (btn != null) iceButtonText = btn.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    private void UpdateBuildUIInfo()
    {
        FindBuildUITexts();

        if (cannonButtonText != null)
        {
            cannonButtonText.fontSize = 14;
            cannonButtonText.text = $"Tháp Cannon ({cannonCost}G)";
        }

        if (missileButtonText != null)
        {
            missileButtonText.fontSize = 14;
            missileButtonText.text = $"Tháp Missile ({missileCost}G)";
        }

        if (iceButtonText != null)
        {
            iceButtonText.fontSize = 14;
            iceButtonText.text = $"Tháp Ice ({iceCost}G)";
        }
    }

    private void FindUpgradeUITexts()
    {
        if (upgradeUI != null)
        {
            if (upgradeButtonText == null)
            {
                Transform upBtn = upgradeUI.transform.Find("Upgrade_Button");
                if (upBtn != null) upgradeButtonText = upBtn.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (sellButtonText == null)
            {
                Transform sellBtn = upgradeUI.transform.Find("Sell_Button");
                if (sellBtn != null) sellButtonText = sellBtn.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    private void Start()
    {
        if (buildUI != null) buildUI.SetActive(false);
        if (upgradeUI != null) upgradeUI.SetActive(false);
    }

    // Hàm cập nhật chữ và giá trên các nút Upgrade & Sell của tháp đang chọn
    public void UpdateUpgradeUIInfo()
    {
        FindUpgradeUITexts();

        if (selectedTowerToUpgrade == null) return;

        // Cập nhật nút Nâng Cấp
        if (upgradeButtonText != null)
        {
            if (selectedTowerToUpgrade.towerLevel >= 2)
            {
                upgradeButtonText.text = "TỐI ĐA";
            }
            else
            {
                upgradeButtonText.text = $"NÂNG CẤP\n<size=75%>{selectedTowerToUpgrade.upgradeCost} Vàng</size>";
            }
        }

        // Cập nhật nút Bán
        if (sellButtonText != null)
        {
            sellButtonText.text = $"BÁN\n<size=75%>+{selectedTowerToUpgrade.sellValue} Vàng</size>";
        }
    }

    // Hàm này được gọi khi click vào 1 ô đất (Node)
    public void SelectNode(Node node)
    {
        selectedNode = node;

        // Nếu ô đất đã có tháp -> Mở UI Nâng cấp/Bán, ẩn UI Xây
        if (node.currentTower != null)
        {
            selectedTowerToUpgrade = node.currentTower.GetComponent<BaseTower>();

            // Cập nhật giá tương ứng của từng tháp trước khi hiển thị panel
            UpdateUpgradeUIInfo();

            if (upgradeUI != null) upgradeUI.SetActive(true);
            if (buildUI != null) buildUI.SetActive(false);
        }
        else // Ngược lại nếu ô đất còn trống -> Mở UI Xây, ẩn UI Nâng cấp/Bán
        {
            selectedTowerToUpgrade = null;

            // Cập nhật giá của các loại tháp lên các nút
            UpdateBuildUIInfo();

            if (buildUI != null) buildUI.SetActive(true);
            if (upgradeUI != null) upgradeUI.SetActive(false);
        }
    }

    // Nâng cấp tháp đang chọn
    public void UpgradeSelectedTower()
    {
        if (selectedTowerToUpgrade == null) return;

        // Kiểm tra xem tháp đã đạt level tối đa chưa
        if (selectedTowerToUpgrade.towerLevel >= 2)
        {
            Debug.Log("Tháp đã đạt cấp tối đa!");
            CloseUI();
            return;
        }

        // Kiểm tra tiền
        if (CurrencySystem.Instance != null && !CurrencySystem.Instance.HasEnoughMoney(selectedTowerToUpgrade.upgradeCost))
        {
            Debug.Log("Không đủ tiền để nâng cấp tháp này!");
            CloseUI();
            return;
        }

        // Trừ tiền và gọi hàm nâng cấp của tháp
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.SpendMoney(selectedTowerToUpgrade.upgradeCost);
        }

        selectedTowerToUpgrade.UpgradeTower();

        // Đóng UI sau khi nâng cấp
        CloseUI();
    }

    // Bán tháp đang chọn
    public void SellSelectedTower()
    {
        if (selectedTowerToUpgrade == null) return;

        // Gọi hàm bán tháp (đã tự cộng tiền và Destroy)
        selectedTowerToUpgrade.SellTower();

        // Xóa liên kết tháp trên Node
        if (selectedNode != null)
        {
            selectedNode.currentTower = null;
        }

        // Đóng UI sau khi bán
        CloseUI();
    }

    // Các hàm xây tháp gắn vào nút trên Build UI
    public void BuildCannonTower()
    {
        TryBuildTower(cannonPrefab, cannonCost);
    }

    public void BuildMissileTower()
    {
        TryBuildTower(missilePrefab, missileCost);
    }

    public void BuildIceTower()
    {
        TryBuildTower(icePrefab, iceCost);
    }

    // Logic xây tháp thực sự
    private void TryBuildTower(GameObject towerPrefab, int cost)
    {
        if (selectedNode == null) return;

        // KIỂM TRA ĐIỀU KIỆN Ô ĐƯỢC PHÉP XÂY DỰNG
        if (!selectedNode.isBuildable || selectedNode.currentTower != null)
        {
            Debug.Log("Không thể xây tháp tại vị trí này (nằm trên đường A* hoặc đã có tháp)!");
            CloseUI();
            return;
        }

        // KIỂM TRA TIỀN TRƯỚC KHI XÂY
        if (CurrencySystem.Instance != null && !CurrencySystem.Instance.HasEnoughMoney(cost))
        {
            Debug.Log("Bạn không đủ tiền để xây tháp này!");
            CloseUI();
            return;
        }

        // TRỪ TIỀN
        if (CurrencySystem.Instance != null)
        {
            CurrencySystem.Instance.SpendMoney(cost);
        }

        // Sinh ra tháp tại vị trí của Node, nâng y lên 1 chút để tháp không bị lún
        Vector3 buildPos = selectedNode.transform.position;
        buildPos.y += 0.5f;

        GameObject towerObj = Instantiate(towerPrefab, buildPos, Quaternion.identity);

        // Lưu tháp vừa tạo vào Node để phục vụ việc chọn nâng cấp / bán sau này
        selectedNode.currentTower = towerObj;

        // Đóng UI sau khi xây
        CloseUI();
    }

    // Hàm đóng tất cả UI liên quan đến chọn tháp / nâng cấp
    public void CloseUI()
    {
        if (buildUI != null) buildUI.SetActive(false);
        if (upgradeUI != null) upgradeUI.SetActive(false);
        selectedNode = null;
        selectedTowerToUpgrade = null;
    }
}