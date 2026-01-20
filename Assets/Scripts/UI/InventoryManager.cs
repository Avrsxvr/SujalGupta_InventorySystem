using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("References")]
    public GameObject inventoryPanel; // Your inventory UI panel
    public InventorySystem inventorySystem;
    public InputController inputController;

    private void Awake()
    {
        if (!inventorySystem)
            inventorySystem = FindFirstObjectByType<InventorySystem>();
        
        if (!inputController)
            inputController = FindFirstObjectByType<InputController>();
    }

    private void OnEnable()
    {
        // Logic handled by InventoryUI for specific key binds roughly
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        // Start with inventory closed
        if (inventoryPanel)
            inventoryPanel.SetActive(false);
        
        inventorySystem?.SetOpen(false);
    }

    private void ToggleInventory()
    {
        if (!inventoryPanel) return;

        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);
        inventorySystem?.SetOpen(newState);

        // Optional: Pause game, lock cursor, etc.
        Time.timeScale = newState ? 0f : 1f; // Pause when inventory open
        Cursor.lockState = newState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = newState;

        if (newState)
        {
            // Only show hint if there are items
            bool hasItems = inventorySystem != null && (inventorySystem.UniqueItemCount > 0 || inventorySystem.StackableItemTypeCount > 0);
            
            if (hasItems)
                HintUIManager.Instance?.ShowInventory();
            else
                HintUIManager.Instance?.Hide();
        }
        else
        {
            HintUIManager.Instance?.Hide();
        }

        Debug.Log($" Inventory {(newState ? "OPENED" : "CLOSED")}");
    }
}