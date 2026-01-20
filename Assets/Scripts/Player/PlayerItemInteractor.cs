using UnityEngine;

public class PlayerItemInteractor : MonoBehaviour
{
    [Header("References")]
    public InputController inputController;
    public InventorySystem inventory;
    public WeaponController weaponController;
    // Removed PickupUI reference
    public InventoryUI inventoryUI;
    public PlayerShooting playerShooting;

    [Header("Drop Settings")]
    public Transform dropPoint;
    public float dropDistance = 1.5f;

    private WorldItem currentItem;

    private void Awake()
    {
        inputController ??= FindFirstObjectByType<InputController>();
        inventory ??= FindFirstObjectByType<InventorySystem>();
        weaponController ??= FindFirstObjectByType<WeaponController>();
        inventoryUI ??= FindFirstObjectByType<InventoryUI>();
        playerShooting ??= FindFirstObjectByType<PlayerShooting>();
        dropPoint ??= transform;
    }

    private void OnEnable()
    {
        inputController.OnPickupPressed += TryPickup;
        inputController.OnDropPressed += TryDropSelectedItem;
    }

    private void OnDisable()
    {
        inputController.OnPickupPressed -= TryPickup;
        inputController.OnDropPressed -= TryDropSelectedItem;
    }

    // WORLD DETECTION
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out WorldItem item)) return;
        if (item.itemData == null) return;

        currentItem = item;
        HintUIManager.Instance?.ShowPickup(item.itemData.ItemName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out WorldItem item) && item == currentItem)
        {
            currentItem = null;
            HintUIManager.Instance?.Hide();
        }
    }

    // PICKUP
    private void TryPickup()
    {
        if (!currentItem) return;

        BaseItem item = currentItem.itemData;
        GameObject worldObj = currentItem.gameObject;

        // AMMO
        if (item is AmmoItem ammo)
        {
            // Add the specific amount of bullets
            if (!inventory.TryAddItem(ammo, ammo.bulletsPerPack))
            {
                HintUIManager.Instance?.ShowPickupSuccess("Inventory full!"); // Error logic might need review to fit format but keeping simple for now
                return;
            }

            Destroy(worldObj);
            
            // Notify Progress
            GameProgress.Instance?.OnItemPickedUp(ammo);

            // Pass just the quantity string so it fits into "Picked up {0}" -> "Picked up 50 bullets"
            HintUIManager.Instance?.ShowPickupSuccess($"{ammo.bulletsPerPack} bullets");

            // Evaluate hints based on new inventory state
            // Delay hint update by 3 seconds so "Picked up..." message can be seen
            playerShooting.EvaluateGunAmmoHints(3f);
            currentItem = null;
            return;
        }

        // WEAPON
        if (item is WeaponItem weapon)
        {
            if (weaponController.HasWeapon)
            {
                HintUIManager.Instance?.ShowWeaponAlreadyHeld();
                // Removed Destroy(worldObj) so the item stays in the world
                return;
            }

            weaponController.EquipWeapon(weapon, worldObj);

            if (!inventory.HasItem(weapon))
                inventory.TryAddItem(weapon);

            // Notify Progress
            GameProgress.Instance?.OnItemPickedUp(weapon);
            
            // Reverted: No longer showing progress in hint
            HintUIManager.Instance?.ShowPickupSuccess(weapon.ItemName);

            // Delay hint update by 3 seconds
            playerShooting.OnWeaponEquipped(3f);
            currentItem = null;
            return;
        }

        // OTHER ITEMS
        if (!inventory.TryAddItem(item))
        {
            HintUIManager.Instance?.ShowPickupSuccess("Inventory full!");
            return;
        }

        Destroy(worldObj);
        
        // Notify Progress
        GameProgress.Instance?.OnItemPickedUp(item);
        
        HintUIManager.Instance?.ShowPickupSuccess(item.ItemName);
        currentItem = null;
    }

    // DROP
    private void TryDropSelectedItem()
    {
        if (!inventoryUI.HasSelection())
        {
            HintUIManager.Instance?.ShowInventory();
            return;
        }

        BaseItem item = inventoryUI.GetSelectedItem();

        if (item is AmmoItem)
        {
            HintUIManager.Instance?.ShowAmmoBlocked();
            return;
        }

        if (!inventory.TryRemoveItem(item))
            return;

        // Notify Progress (Decrement)
        GameProgress.Instance?.OnItemDropped(item);

        GameObject obj;

        if (item is WeaponItem weapon &&
            weaponController.HasWeapon &&
            weaponController.CurrentWeapon.ItemId == weapon.ItemId)
        {
            obj = weaponController.DropWeapon();
            playerShooting.OnWeaponDropped();
        }
        else
        {
            obj = Instantiate(item.worldPrefab);
        }

        SpawnWorldItem(obj, item);
        inventoryUI.ClearSelection();
    }

    // WORLD SPAWN
    private void SpawnWorldItem(GameObject obj, BaseItem item)
    {
        // 1. Position the object first
        obj.transform.position = dropPoint.position + dropPoint.forward * dropDistance;
        obj.transform.rotation = Quaternion.identity;
        
        // 2. Get or Add WorldItem component
        WorldItem wi = obj.GetComponent<WorldItem>() ?? obj.AddComponent<WorldItem>();
        wi.SetItemData(item);

        // 3. Activate and Reset Hover Start Position
        wi.SetWorldActive(true);

        Collider col = obj.GetComponent<Collider>() ?? obj.AddComponent<BoxCollider>();
        col.enabled = true;
        col.isTrigger = true;

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
