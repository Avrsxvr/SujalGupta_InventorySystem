using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Base Item")]
public class BaseItem : ScriptableObject
{
    [Header("Basic Info")]
    public int ItemId;
    public string ItemName;
    public Sprite Icon;

    [TextArea]
    public string Description;

    public bool IsStackable = true;

    [Header("World Prefab")]
    public GameObject worldPrefab; // ADD THIS LINE
}