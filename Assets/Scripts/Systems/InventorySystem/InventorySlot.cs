using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject selectionHighlight;

    private BaseItem item;
    private int count;

    private void Awake()
    {
        // 🔒 CRITICAL: selection highlight must NOT block clicks
        if (selectionHighlight != null)
        {
            Image img = selectionHighlight.GetComponent<Image>();
            if (img != null)
                img.raycastTarget = false;

            selectionHighlight.SetActive(false);
        }

        // ✅ FIX: icon must not block clicks
        if (icon != null)
            icon.raycastTarget = false;

        // ✅ FIX: count text must not block clicks
        if (countText != null)
            countText.raycastTarget = false;
    }

    
    // SET SLOT DATA
    
    public void Set(BaseItem newItem, int newCount)
    {
        item = newItem;
        count = newCount;

        icon.sprite = item.Icon;
        icon.enabled = true;

        countText.text = count.ToString();
        countText.gameObject.SetActive(true);

        SetSelected(false);

        Debug.Log($"[InventorySlot] SET {item.ItemName} x{count}");
    }

    
    // COUNT (SAFE UPDATE)
    
    public void UpdateCount(int newCount)
    {
        count = Mathf.Max(0, newCount); // ✅ FIX: never go negative

        if (countText == null)
        {
            Debug.LogError("[InventorySlot] ❌ CountText missing");
            return;
        }

        countText.text = count.ToString();
        countText.gameObject.SetActive(true);

        Debug.Log($"[InventorySlot] 🔢 Count updated → {item?.ItemName}: {count}");
    }

    
    // CLEAR
    
    public void Clear()
    {
        Debug.Log($"[InventorySlot] CLEAR → {item?.ItemName}");

        item = null;
        count = 0;

        icon.sprite = null;
        icon.enabled = false;

        countText.text = "";
        countText.gameObject.SetActive(false);

        SetSelected(false);
    }

    
    // SELECTION
    
    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(selected);
            Debug.Log($"[InventorySlot] Selection {(selected ? "ON" : "OFF")} → {item?.ItemName}");
        }
    }

    
    // CLICK
    
    public void OnPointerClick(PointerEventData eventData)
{
    Debug.Log($"[InventorySlot] 🖱 CLICKED → {item?.ItemName}");

    if (item == null)
        return;

    if (InventoryUI.Instance == null)
        return;

    // 🔁 TOGGLE SELECTION
    if (InventoryUI.Instance.IsSelected(this))
    {
        InventoryUI.Instance.ClearSelection();
        return;
    }

    InventoryUI.Instance.SelectSlot(this);
}


    
    // GETTERS (UNCHANGED)
    
    public bool IsEmpty() => item == null;
    public BaseItem GetItem() => item;
    public int GetCount() => count;
}
