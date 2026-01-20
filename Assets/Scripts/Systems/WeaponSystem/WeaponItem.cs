using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : BaseItem
{
    [Header("Weapon Settings")]
    public GameObject weaponPrefab;
    public BaseItem ammoType;
    
    // ADD THESE NEW FIELDS:
    [Header("Hold Settings")]
    public Vector3 holdOffset = Vector3.zero;
    public Vector3 holdRotation = Vector3.zero;
    
    private void OnValidate()
    {
        IsStackable = false;
    }
}