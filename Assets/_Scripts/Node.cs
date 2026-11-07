using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Màu sắc tương tác")]
    public Color hoverColor;
    public Color errorColor;

    private Color defaultColor;
    private Renderer rend;

    [Header("Trạng thái Node")]
    public GameObject currentTower;
    public bool isBuildable = true;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            defaultColor = rend.material.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rend == null) return;

        if (!isBuildable)
        {
            rend.material.color = errorColor;
            return;
        }
        rend.material.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rend == null) return;
        rend.material.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuildable)
        {
            Debug.Log("Khu vực này không được phép xây dựng!");
            return;
        }

        Debug.Log("Bạn vừa click vào Node: " + gameObject.name);

        // Gọi BuildManager để xử lý (xây mới nếu ô trống, hoặc hiện bảng Nâng cấp/Bán nếu đã có tháp)
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.SelectNode(this);
        }
    }
}