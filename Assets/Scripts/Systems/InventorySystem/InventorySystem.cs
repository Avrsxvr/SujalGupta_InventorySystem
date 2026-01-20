using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxUniqueItems = 10;
    [SerializeField] private bool enableDebugLogs = true;

    
    public event System.Action OnInventoryChanged;
    public event System.Action<BaseItem> OnItemAdded;
    public event System.Action<BaseItem> OnItemRemoved;

    
    // INVENTORY DATA
    
    // Unique items (weapons, quest items) - can only have ONE of each
    private Dictionary<int, BaseItem> uniqueItems = new Dictionary<int, BaseItem>();
    
    // Stackable items (ammo, consumables) - can have multiple
    private Dictionary<int, int> stackableItems = new Dictionary<int, int>();
    
    // Reference to stackable item ScriptableObjects
    private Dictionary<int, BaseItem> stackableItemReferences = new Dictionary<int, BaseItem>();

    
    // PROPERTIES
    
    public bool IsOpen { get; private set; }
    
    public int UniqueItemCount => uniqueItems.Count;
    public int StackableItemTypeCount => stackableItems.Count;
    public bool IsFull => uniqueItems.Count >= maxUniqueItems;

    
    // INITIALIZATION
    
    private void Awake()
    {
        IsOpen = false;
        
        if (enableDebugLogs)
            Debug.Log("[InventorySystem] Initialized");
    }

    
    // INVENTORY STATE
    
    public void SetOpen(bool open)
    {
        IsOpen = open;
        
        if (enableDebugLogs)
            Debug.Log($"[InventorySystem] Inventory IsOpen set to: {IsOpen}");
    }

    public void ToggleOpen()
    {
        SetOpen(!IsOpen);
    }

    
    // ADD ITEMS
    
    public bool TryAddItem(BaseItem item, int amount = 1)
    {
        if (!item)
        {
            Debug.LogWarning("[InventorySystem] Trying to add NULL item!");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("[InventorySystem]  Amount must be greater than 0");
            return false;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[InventorySystem] ===== ADDING ITEM =====");
            Debug.Log($"[InventorySystem] Item: {item.ItemName}");
            Debug.Log($"[InventorySystem] ItemId: {item.ItemId}");
            Debug.Log($"[InventorySystem] IsStackable: {item.IsStackable}");
            Debug.Log($"[InventorySystem] Amount: {amount}");
        }

        bool success = false;

        
        // UNIQUE ITEMS (Weapons, etc.)
        
        if (!item.IsStackable)
        {
            // Unique items can only be added 1 at a time in this logic pattern, 
            // or we need to loop. For now, assuming amount 1 for unique or loop externally.
            // But usually unique items are picked up one by one.
            if (amount > 1) 
                Debug.LogWarning("[InventorySystem]  Adding multiple UNIQUE items at once is unusual. Only adding first one.");

            // Check if already exists
            if (uniqueItems.ContainsKey(item.ItemId))
            {
                Debug.LogWarning($"[InventorySystem]  Unique item {item.ItemName} (ID: {item.ItemId}) already exists!");
                return false;
            }

            // Check if inventory is full
            if (IsFull)
            {
                Debug.LogWarning($"[InventorySystem]  Inventory full! Cannot add {item.ItemName}");
                return false;
            }

            // Add to unique items
            uniqueItems.Add(item.ItemId, item);
            success = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[InventorySystem]  Added to UNIQUE items");
                Debug.Log($"[InventorySystem] Total unique items: {uniqueItems.Count}");
            }
        }
        
        // STACKABLE ITEMS (Ammo, consumables, etc.)
        
        else
        {
            // Store reference to the item if we don't have it yet
            if (!stackableItemReferences.ContainsKey(item.ItemId))
            {
                stackableItemReferences[item.ItemId] = item;
                
                if (enableDebugLogs)
                    Debug.Log($"[InventorySystem] Stored stackable item reference for ID: {item.ItemId}");
            }

            // Initialize count if needed
            if (!stackableItems.ContainsKey(item.ItemId))
            {
                stackableItems[item.ItemId] = 0;
            }

            // Increment count
            stackableItems[item.ItemId] += amount;
            success = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[InventorySystem]  Added to STACKABLE items");
                Debug.Log($"[InventorySystem] Item: {item.ItemName}, New Count: {stackableItems[item.ItemId]}");
                Debug.Log($"[InventorySystem] Total stackable item types: {stackableItems.Count}");
            }
        }

        if (success)
        {
            // Invoke events
            OnItemAdded?.Invoke(item);
            OnInventoryChanged?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[InventorySystem] =======================");
        }

        return success;
    }

    
    // REMOVE ITEMS
    
    public bool TryRemoveItem(BaseItem item, int amount = 1)
    {
        if (!item)
        {
            Debug.LogWarning("[InventorySystem]  Trying to remove NULL item!");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("[InventorySystem]  Amount must be greater than 0!");
            return false;
        }

        bool success = false;

        
        // UNIQUE ITEMS
        
        if (!item.IsStackable)
        {
            if (!uniqueItems.ContainsKey(item.ItemId))
            {
                if (enableDebugLogs)
                    Debug.Log($"[InventorySystem]  Unique item {item.ItemName} not found in inventory");
                return false;
            }

            uniqueItems.Remove(item.ItemId);
            success = true;

            if (enableDebugLogs)
                Debug.Log($"[InventorySystem]  Removed unique item: {item.ItemName}");
        }
        
        // STACKABLE ITEMS
        
        else
        {
            if (!stackableItems.ContainsKey(item.ItemId))
            {
                if (enableDebugLogs)
                    Debug.Log($"[InventorySystem] Stackable item {item.ItemName} not found in inventory");
                return false;
            }

            int currentCount = stackableItems[item.ItemId];

            // Check if we have enough
            if (currentCount < amount)
            {
                Debug.LogWarning($"[InventorySystem]  Not enough {item.ItemName}! Have: {currentCount}, Need: {amount}");
                return false;
            }

            // Decrease count
            stackableItems[item.ItemId] -= amount;
            success = true;

            if (enableDebugLogs)
                Debug.Log($"[InventorySystem] Removed {amount} of {item.ItemName}. Remaining: {stackableItems[item.ItemId]}");

            // If count reaches 0, remove completely
            if (stackableItems[item.ItemId] <= 0)
            {
                stackableItems.Remove(item.ItemId);
                stackableItemReferences.Remove(item.ItemId);

                if (enableDebugLogs)
                    Debug.Log($"[InventorySystem] Stackable item removed completely: {item.ItemName}");
            }
        }

        if (success)
        {
            // Invoke events
            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
        }

        return success;
    }

    
    // QUERY ITEMS
    
    public bool HasItem(BaseItem item)
    {
        if (!item) return false;

        if (!item.IsStackable)
            return uniqueItems.ContainsKey(item.ItemId);
        else
            return stackableItems.ContainsKey(item.ItemId);
    }

    public bool HasItem(int itemId)
    {
        return uniqueItems.ContainsKey(itemId) || stackableItems.ContainsKey(itemId);
    }

    public int GetItemCount(BaseItem item)
    {
        if (!item) return 0;

        if (!item.IsStackable)
            return uniqueItems.ContainsKey(item.ItemId) ? 1 : 0;
        else
            return stackableItems.TryGetValue(item.ItemId, out int count) ? count : 0;
    }

    public BaseItem GetItemById(int itemId)
    {
        // Check unique items first
        if (uniqueItems.TryGetValue(itemId, out BaseItem uniqueItem))
            return uniqueItem;

        // Then check stackable items
        if (stackableItemReferences.TryGetValue(itemId, out BaseItem stackableItem))
            return stackableItem;

        return null;
    }

    
    // GET INVENTORY DATA
    
    public IReadOnlyDictionary<int, BaseItem> GetUniqueItems() => uniqueItems;
    
    public IReadOnlyDictionary<int, int> GetStackableItems() => stackableItems;
    
    public BaseItem GetStackableItemReference(int itemId)
    {
        return stackableItemReferences.TryGetValue(itemId, out BaseItem item) ? item : null;
    }

    public List<BaseItem> GetAllItems()
    {
        List<BaseItem> allItems = new List<BaseItem>();

        // Add unique items
        foreach (var item in uniqueItems.Values)
            allItems.Add(item);

        // Add stackable items
        foreach (var item in stackableItemReferences.Values)
            allItems.Add(item);

        return allItems;
    }

    
    // INVENTORY MANAGEMENT
    
    public void ClearInventory()
    {
        uniqueItems.Clear();
        stackableItems.Clear();
        stackableItemReferences.Clear();

        Debug.Log("[InventorySystem]  Inventory cleared!");
        
        OnInventoryChanged?.Invoke();
    }

    public void RemoveAllItemsOfType(BaseItem item)
    {
        if (!item) return;

        if (!item.IsStackable)
        {
            TryRemoveItem(item);
        }
        else
        {
            if (stackableItems.TryGetValue(item.ItemId, out int count))
            {
                TryRemoveItem(item, count);
            }
        }
    }

    
    // DEBUG / UTILITY
    
    public void DebugInventory()
    {
        Debug.Log("========== INVENTORY STATE ==========");
        Debug.Log($"Is Open: {IsOpen}");
        Debug.Log($"Unique Items: {uniqueItems.Count}/{maxUniqueItems}");
        
        foreach (var pair in uniqueItems)
        {
            Debug.Log($"  - ID: {pair.Key}, Item: {pair.Value.ItemName}");
        }
        
        Debug.Log($"Stackable Items: {stackableItems.Count} types");
        
        foreach (var pair in stackableItems)
        {
            BaseItem item = GetStackableItemReference(pair.Key);
            Debug.Log($"  - ID: {pair.Key}, Count: {pair.Value}, Item: {item?.ItemName ?? "NULL"}");
        }
        
        Debug.Log("=====================================");
    }

    public void EnableDebugLogs(bool enable)
    {
        enableDebugLogs = enable;
    }

    
    // SAVE/LOAD SUPPORT (Optional - for future use)
    
    [System.Serializable]
    public class InventorySaveData
    {
        public List<int> uniqueItemIds = new List<int>();
        public List<int> stackableItemIds = new List<int>();
        public List<int> stackableItemCounts = new List<int>();
    }

    public InventorySaveData GetSaveData()
    {
        InventorySaveData data = new InventorySaveData();

        // Save unique items
        foreach (var itemId in uniqueItems.Keys)
            data.uniqueItemIds.Add(itemId);

        // Save stackable items
        foreach (var pair in stackableItems)
        {
            data.stackableItemIds.Add(pair.Key);
            data.stackableItemCounts.Add(pair.Value);
        }

        return data;
    }

    
}