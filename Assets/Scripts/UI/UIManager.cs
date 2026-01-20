using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    public GameObject inventoryPanel;
    public InventorySystem inventorySystem;

    public bool IsInventoryOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        CloseInventory();
    }

    // Call this method from your UI button's OnClick event
    public void ToggleInventory()
    {
        if (IsInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        inventorySystem.SetOpen(true);
        IsInventoryOpen = true;
        
        bool hasItems = inventorySystem != null && (inventorySystem.UniqueItemCount > 0 || inventorySystem.StackableItemTypeCount > 0);
        
        if (hasItems)
            HintUIManager.Instance?.ShowInventory();
        else
            HintUIManager.Instance?.Hide();
        
        Debug.Log("✅ Inventory OPENED - IsInventoryOpen set to TRUE");
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        inventorySystem.SetOpen(false);
        IsInventoryOpen = false;
        
        HintUIManager.Instance?.Hide();
        
        Debug.Log("❌ Inventory CLOSED - IsInventoryOpen set to FALSE");
    }

    // Alternative: Automatically sync state based on panel active state
    private void Update()
    {
        // Auto-sync IsInventoryOpen with the actual panel state
        // This ensures the flag always matches reality
        bool panelActive = inventoryPanel != null && inventoryPanel.activeSelf;
        
        if (panelActive != IsInventoryOpen)
        {
            IsInventoryOpen = panelActive;
            if (inventorySystem != null)
                inventorySystem.SetOpen(panelActive);
            
            Debug.Log($"🔄 Auto-synced inventory state: {IsInventoryOpen}");
        }
    }
}