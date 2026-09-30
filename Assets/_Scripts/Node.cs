using UnityEngine;
using UnityEngine.EventSystems; // Bắt buộc phải khai báo thư viện này

// Thêm các Interface: IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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

    // Thay thế OnMouseEnter
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rend == null) return;

        if (!isBuildable || currentTower != null)
        {
            rend.material.color = errorColor;
            return;
        }
        rend.material.color = hoverColor;
    }

    // Thay thế OnMouseExit
    public void OnPointerExit(PointerEventData eventData)
    {
        if (rend == null) return;
        rend.material.color = defaultColor;
    }

    // Thay thế OnMouseDown
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isBuildable)
        {
            Debug.Log("Khu vực này không được phép xây dựng!");
            return;
        }

        if (currentTower != null)
        {
            Debug.Log("Ô này đã có tháp rồi, hãy chọn Nâng cấp thay vì xây mới!");
            return;
        }

        Debug.Log("Bạn vừa click vào Node: " + gameObject.name + " - Chuẩn bị xây tháp!");
    }
}