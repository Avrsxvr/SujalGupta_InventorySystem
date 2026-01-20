using UnityEngine;
using UnityEngine.InputSystem; // ← Add this

public class GunDebugHelper : MonoBehaviour
{
    [SerializeField] private WeaponController weaponController;

    private void Update()
    {
        // Use new Input System
        if (Keyboard.current.pKey.wasPressedThisFrame) // Press P to debug
        {
            DebugGunInfo();
        }
    }

    private void DebugGunInfo()
    {
        if (weaponController.HasWeapon)
        {
            GameObject gun = weaponController.GetGunObject();
            if (gun != null)
            {
                Debug.Log("=== GUN DEBUG INFO ===");
                Debug.Log($"Gun Name: {gun.name}");
                Debug.Log($"Gun Active: {gun.activeSelf}");
                Debug.Log($"Gun Position: {gun.transform.position}");
                Debug.Log($"Gun Local Position: {gun.transform.localPosition}");
                Debug.Log($"Gun Parent: {gun.transform.parent?.name ?? "NULL"}");
                Debug.Log($"Gun Layer: {LayerMask.LayerToName(gun.layer)}");
                
                // Check renderers
                Renderer[] renderers = gun.GetComponentsInChildren<Renderer>();
                Debug.Log($"Number of Renderers: {renderers.Length}");
                foreach (var r in renderers)
                {
                    Debug.Log($"  Renderer: {r.name}, Enabled: {r.enabled}");
                }
            }
            else
            {
                Debug.Log("❌ Gun object is NULL!");
            }
        }
        else
        {
            Debug.Log("❌ No weapon equipped!");
        }
    }
}