using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("References")]
    public InventorySystem inventory;
    public InventorySlot slotPrefab;
    public Transform slotParent;

    [Header("UI Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button inventoryMenuButton; // New Button Logic

    [Header("Settings")]
    [SerializeField] private int maxVisibleSlots = 20;

    private readonly List<InventorySlot> slots = new();
    private readonly Dictionary<int, InventorySlot> itemSlotMap = new();

    private InventorySlot selectedSlot;
    private PlayerShooting playerShooting;
    private InputController inputController;

    // INIT 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (!inventory)
            inventory = FindFirstObjectByType<InventorySystem>();

        playerShooting = FindFirstObjectByType<PlayerShooting>();
        inputController = FindFirstObjectByType<InputController>();

        Debug.Log("[InventoryUI] Awake complete");
    }

    private void Start()
    {
        if (inventory != null)
        {
            inventory.OnItemAdded += OnItemAdded;
            inventory.OnItemRemoved += OnItemRemoved;
        }

        if (inputController != null)
        {
            inputController.OnInventoryToggle += HandleInventoryToggle;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseInventory);
        }

        // 🔥 NEW BUTTON LOGIC
        if (inventoryMenuButton != null)
        {
            inventoryMenuButton.onClick.AddListener(OnInventoryMenuClicked);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnItemAdded -= OnItemAdded;
            inventory.OnItemRemoved -= OnItemRemoved;
        }

        if (inputController != null)
        {
            inputController.OnInventoryToggle -= HandleInventoryToggle;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseInventory);
        }

        if (inventoryMenuButton != null)
        {
            inventoryMenuButton.onClick.RemoveListener(OnInventoryMenuClicked);
        }
    }

    private void HandleInventoryToggle()
    {
        if (inventory == null) return;

        if (inventory.IsOpen)
        {
            // If open, simulate closing
            if (closeButton != null) 
                closeButton.onClick.Invoke();
        }
        else
        {
            // If closed, simulate opening (Menu Click)
            if (inventoryMenuButton != null)
                inventoryMenuButton.onClick.Invoke();
        }
    }

    // EVENTS
    private void OnItemAdded(BaseItem item)
    {
        Refresh();
    }

    private void OnItemRemoved(BaseItem item)
    {
        Refresh();
    }

    // REFRESH
    public void Refresh()
    {
        ClearSlots();
        int slotCount = 0;

        // Unique items
        foreach (var pair in inventory.GetUniqueItems())
        {
            if (slotCount++ >= maxVisibleSlots) break;
            CreateSlot(pair.Value, 1);
        }

        // Stackable items
        foreach (var pair in inventory.GetStackableItems())
        {
            if (slotCount++ >= maxVisibleSlots) break;

            BaseItem item = inventory.GetStackableItemReference(pair.Key);
            CreateSlot(item, pair.Value);
        }
    }

    // SLOT CREATION
    private void CreateSlot(BaseItem item, int count)
    {
        InventorySlot slot = Instantiate(slotPrefab, slotParent);

        // Standard display count from inventory
        slot.Set(item, count);
        slots.Add(slot);
        itemSlotMap[item.ItemId] = slot;
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slots.Clear();
        itemSlotMap.Clear();
        selectedSlot = null;
    }

    // SELECTION LOGIC
    public void SelectSlot(InventorySlot slot)
    {
        // 🔁 Clicking same slot again deselects
        if (selectedSlot == slot)
        {
            ClearSelection();
            return;
        }

        if (selectedSlot != null)
            selectedSlot.SetSelected(false);

        selectedSlot = slot;
        selectedSlot.SetSelected(true);

        HintUIManager.Instance?.ShowDrop(slot.GetItem().ItemName);
    }

    public bool HasSelection()
    {
        return selectedSlot != null && !selectedSlot.IsEmpty();
    }

    public BaseItem GetSelectedItem()
    {
        return selectedSlot != null ? selectedSlot.GetItem() : null;
    }

    public void ClearSelection()
    {
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
            selectedSlot = null;
        }

        // Fix: If inventory is still open, show the "Select item" hint instead of hiding everything
        if (inventory != null && inventory.IsOpen)
        {
            // Only show hint if we have items
            bool hasItems = (inventory.UniqueItemCount > 0 || inventory.StackableItemTypeCount > 0);

            if (hasItems)
                HintUIManager.Instance?.ShowInventory();
            else
                HintUIManager.Instance?.Hide();
        }
        else
        {
             HintUIManager.Instance?.Hide();
        }
    }

    public bool IsSelected(InventorySlot slot)
    {
        return selectedSlot == slot;
    }

    // AMMO SLOT ACCESS (FIX)
    public InventorySlot GetAmmoSlot()
    {
        foreach (var pair in itemSlotMap)
        {
            if (pair.Value != null && pair.Value.GetItem() is AmmoItem)
                return pair.Value;
        }
        return null;
    }

    // CLOSE BUTTON
    private void OnCloseInventory()
    {
        Debug.Log("[InventoryUI] Close button clicked → clearing selection");
        ClearSelection();
        HintUIManager.Instance?.Hide(); // Force hide hints when specifically closing via button
    }

    private void OnInventoryMenuClicked()
    {
        Debug.Log("[InventoryUI] Inventory Menu button clicked");
        
        bool hasItems = inventory != null && (inventory.UniqueItemCount > 0 || inventory.StackableItemTypeCount > 0);
        
        if (hasItems)
            HintUIManager.Instance?.ShowInventory();
        else
            HintUIManager.Instance?.Hide();
    }
}
