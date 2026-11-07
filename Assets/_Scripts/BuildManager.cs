using UnityEngine;

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

    private Node selectedNode;              // Lưu trữ Node đang được chọn
    private BaseTower selectedTowerToUpgrade; // Lưu trữ tháp trên Node đang được chọn

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (buildUI != null) buildUI.SetActive(false);
        if (upgradeUI != null) upgradeUI.SetActive(false);
    }

    // Hàm này được gọi khi click vào 1 ô đất (Node)
    public void SelectNode(Node node)
    {
        selectedNode = node;

        // Nếu ô đất đã có tháp -> Mở UI Nâng cấp/Bán, ẩn UI Xây
        if (node.currentTower != null)
        {
            selectedTowerToUpgrade = node.currentTower.GetComponent<BaseTower>();

            if (upgradeUI != null) upgradeUI.SetActive(true);
            if (buildUI != null) buildUI.SetActive(false);
        }
        else // Ngược lại nếu ô đất còn trống -> Mở UI Xây, ẩn UI Nâng cấp/Bán
        {
            selectedTowerToUpgrade = null;

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