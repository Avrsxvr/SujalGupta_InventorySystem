using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmoItem : BaseItem
{
    [Header("Ammo Settings")]
    public int bulletsPerPack = 50; // How many bullets in this ammo pack
    
    private void OnValidate()
    {
        // Ammo is STACKABLE (you can have multiple packs)
        IsStackable = true;
    }
}