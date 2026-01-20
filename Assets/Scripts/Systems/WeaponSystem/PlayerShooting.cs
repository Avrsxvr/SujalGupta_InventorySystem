using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private InventorySystem inventorySystem; // Add reference
    [SerializeField] private Transform shootPoint;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float bulletLife = 3f;

    private InputController input;
    private bool hasFiredOnce = false; // Track if user has fired

    private void Awake()
    {
        input = FindFirstObjectByType<InputController>();
        weaponController ??= FindFirstObjectByType<WeaponController>();
        inventorySystem ??= FindFirstObjectByType<InventorySystem>(); // Find inventory
    }

    private void OnEnable()
    {
        input.OnFirePressed += Shoot;
    }

    private void OnDisable()
    {
        input.OnFirePressed -= Shoot;
    }

    // AMMO & WEAPON HELPERS
    
    public int GetCurrentBullets()
    {
        if (!weaponController || !weaponController.HasWeapon) return 0;
        
        // Get ammo type from current weapon
        BaseItem ammoType = weaponController.CurrentWeapon.ammoType;
        if (ammoType == null) return 0;

        // Check inventory
        return inventorySystem ? inventorySystem.GetItemCount(ammoType) : 0;
    }

    // WEAPON EVENTS
    public void OnWeaponEquipped(float delay = 0f)
    {
        EvaluateGunAmmoHints(delay);
    }

    public void OnWeaponDropped()
    {
        EvaluateGunAmmoHints();
    }

    // SHOOT
    private void Shoot()
    {
        if (!weaponController.HasWeapon)
        {
            EvaluateGunAmmoHints();
            return;
        }

        WeaponItem weapon = weaponController.CurrentWeapon;
        if (weapon.ammoType == null)
        {
            Debug.LogError("Weapon has no AmmoType assigned!");
            return;
        }

        // Check if we have enough ammo in inventory
        if (!inventorySystem.HasItem(weapon.ammoType))
        {
            EvaluateGunAmmoHints();
            // TODO: Play "Click" empty sound here
            return;
        }

        // Consume 1 bullet from inventory
        if (inventorySystem.TryRemoveItem(weapon.ammoType, 1))
        {
            hasFiredOnce = true; // Mark that user has learned to fire
            SpawnBullet();
            EvaluateGunAmmoHints();
        }
    }

    private void SpawnBullet()
    {
        if (!bulletPrefab || !shootPoint) return;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        if (bullet.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = shootPoint.forward * bulletSpeed;

        Destroy(bullet, bulletLife);
    }

    // HINT LOGIC
    public void EvaluateGunAmmoHints(float delay = 0f)
    {
        CancelInvoke(nameof(EvaluateGunAmmoHintsInternal));

        if (delay > 0f)
        {
            Invoke(nameof(EvaluateGunAmmoHintsInternal), delay);
        }
        else
        {
            EvaluateGunAmmoHintsInternal();
        }
    }

    private void EvaluateGunAmmoHintsInternal()
    {
        if (HintUIManager.Instance == null) return;

        bool hasGun = weaponController != null && weaponController.HasWeapon;
        int ammoCount = GetCurrentBullets();
        bool hasAmmo = ammoCount > 0;

        if (hasGun && !hasAmmo)
        {
            HintUIManager.Instance.ShowPickAmmoToUseGun();
        }
        else if (!hasGun && hasAmmo)
        {
            HintUIManager.Instance.ShowPickGunToUseAmmo();
        }
        else if (hasGun && hasAmmo)
        {
            // Only show hint if they haven't fired yet
            if (!hasFiredOnce)
            {
                HintUIManager.Instance.ShowPressFire();
            }
            else
            {
                HintUIManager.Instance.Hide();
            }
        }
        else
        {
            HintUIManager.Instance.Hide();
        }
    }
}
