using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Gun Hold Point")]
    [SerializeField] private Transform gunHoldPoint;

    public WeaponItem CurrentWeapon { get; private set; }
    public bool HasWeapon => CurrentWeapon != null;

    private GameObject gunObject;

    // EQUIP
    public void EquipWeapon(WeaponItem weapon, GameObject weaponInstance)
    {
        if (weapon == null || weaponInstance == null)
        {
            Debug.LogError("EquipWeapon failed - weapon or instance is NULL");
            return;
        }

        if (HasWeapon)
        {
            Debug.Log("Already holding a weapon");
            return;
        }

        CurrentWeapon = weapon;
        gunObject = weaponInstance;

        // Attach to gun hold point
        gunObject.transform.SetParent(gunHoldPoint);
        gunObject.transform.localPosition = Vector3.zero;
        gunObject.transform.localRotation = Quaternion.identity;
        gunObject.transform.localScale = Vector3.one;

        // Disable world-only behaviours
        if (gunObject.TryGetComponent(out WorldItem worldItem))
        {
            worldItem.enabled = false;
        }

        // Disable collider while held
        Collider col = gunObject.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Disable physics while held
        Rigidbody rb = gunObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Optional UI cleanup
        HintUIManager.Instance?.Hide();

        Debug.Log($"Equipped weapon - {weapon.ItemName}");
    }

    // DROP
    public GameObject DropWeapon()
    {
        if (!HasWeapon || gunObject == null)
        {
            Debug.LogWarning("DropWeapon called but no weapon equipped");
            return null;
        }

        GameObject dropped = gunObject;

        // Detach from player
        dropped.transform.SetParent(null);

        // Re-enable world interaction
        WorldItem worldItem = dropped.GetComponent<WorldItem>();
        if (worldItem != null)
        {
            worldItem.enabled = true;
        }

        // Re-enable collider as trigger
        Collider col = dropped.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = true;
        }

        // Keep weapon floating (not falling)
        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Restore hover rotation
        // Handled automatically by WorldItem.OnEnable
        
        // Clear references
        gunObject = null;
        CurrentWeapon = null;

        Debug.Log("Weapon dropped to world");

        return dropped;
    }

    // GETTERS
    public GameObject GetGunObject()
    {
        return gunObject;
    }
}
