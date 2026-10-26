using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Danh sách Tháp")]
    public GameObject cannonPrefab;
    public GameObject missilePrefab;
    public GameObject icePrefab;

    [Header("UI Chọn Tháp")]
    public GameObject buildUI; // Kéo Panel UI chứa 3 nút vào đây

    private Node selectedNode; // Lưu trữ Node đang được chọn

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        buildUI.SetActive(false); // Ẩn UI khi mới vào game
    }

    // Hàm này được gọi khi bạn click vào 1 cục đất (Node)
    public void SelectNode(Node node)
    {
        // Nếu ô đất đã có tháp rồi thì không cho xây nữa
        // Giả sử PathNode có biến bool hasTower, nếu chưa có bạn có thể tự thêm vào PathNode.cs
        /* if (node.hasTower) return; */

        selectedNode = node;
        buildUI.SetActive(true); // Hiện bảng chọn tháp lên
    }

    // Các hàm này sẽ được gắn vào 3 nút bấm trên UI
    public void BuildCannonTower()
    {
        BuildTower(cannonPrefab);
    }

    public void BuildMissileTower()
    {
        BuildTower(missilePrefab);
    }

    public void BuildIceTower()
    {
        BuildTower(icePrefab);
    }

    // Logic xây tháp thực sự
    private void BuildTower(GameObject towerPrefab)
    {
        if (selectedNode == null) return;

        // Sinh ra tháp tại vị trí của Node, nâng y lên 1 chút để tháp không bị lún
        Vector3 buildPos = selectedNode.transform.position;
        buildPos.y += 0.5f;

        Instantiate(towerPrefab, buildPos, Quaternion.identity);

        // Đánh dấu Node này đã có tháp (nếu bạn có biến hasTower)
        /* selectedNode.hasTower = true; */

        // Ẩn UI đi sau khi xây xong và xóa lựa chọn Node
        buildUI.SetActive(false);
        selectedNode = null;
    }

    // Hàm để đóng UI nếu người chơi đổi ý không xây nữa
    public void CloseUI()
    {
        buildUI.SetActive(false);
        selectedNode = null;
    }
}