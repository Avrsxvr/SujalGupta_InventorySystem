using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Item Data")]
    public BaseItem itemData;

    [Header("Animation Settings")]
    [SerializeField] private bool enableAnimation = true;
    [SerializeField] private float hoverHeight = 0.25f;
    [SerializeField] private float hoverSpeed = 2f;
    [SerializeField] private float rotationSpeed = 50f;

    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (enableAnimation)
        {
            // Hover
            float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);

            // Rotate
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void SetItemData(BaseItem item)
    {
        itemData = item;
        Debug.Log($"[WorldItem] ItemData set to: {item?.ItemName ?? "NULL"}");
    }

    public void SetWorldActive(bool active)
    {
        gameObject.SetActive(active);
        // If we want to reset startPos when manually setting active via this method (redundant but safe)
        if (active) startPos = transform.position;
        Debug.Log($"[WorldItem] {itemData?.ItemName ?? "Unknown"} GameObject set active: {active}");
    }

    public string GetItemName()
    {
        return itemData ? itemData.ItemName : "Unknown Item";
    }

    public BaseItem GetItemData()
    {
        return itemData;
    }
}