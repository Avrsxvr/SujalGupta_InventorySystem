using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // For Restart
using TMPro;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    [Header("UI Settings")]
    [Tooltip("Assign a TextMeshProUGUI element to show progress (e.g. '1/10')")]
    public TextMeshProUGUI progressUI;
    
    [Header("Win Settings")]
    public GameObject winPanel; // Panel to show on win
    [TextArea] public string winMessage = "MISSION COMPLETE!\nALL ITEMS COLLECTED";
    [Tooltip("If true, Ammo items are NOT counted towards the win condition.")]
    public bool excludeAmmo = true; 

    private int totalItemsToCollect;
    private int currentCollectedCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeItemCount();
        UpdateProgressUI();
        if (winPanel != null) winPanel.SetActive(false);
    }

    private void InitializeItemCount()
    {
        totalItemsToCollect = 0;
        currentCollectedCount = 0;
        bool weaponCounted = false;

        // Find all items currently in the world
        WorldItem[] items = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);

        foreach (var item in items)
        {
            if (item.itemData == null) continue;

            // Logic: Player can only hold ONE weapon. 
            // So if there are multiple weapons in the scene, we only count 1 towards the goal.
            if (item.itemData is WeaponItem)
            {
                if (!weaponCounted)
                {
                    totalItemsToCollect++;
                    weaponCounted = true;
                }
            }
            else if (ShouldCountItem(item.itemData))
            {
                totalItemsToCollect++;
            }
        }

        Debug.Log($"[GameProgress] Total items to collect: {totalItemsToCollect}");
        UpdateProgressUI();
    }

    private bool ShouldCountItem(BaseItem data)
    {
        if (data == null) return false;
        if (excludeAmmo && data is AmmoItem) return false;
        return true;
    }

    // Called when an item is picked up
    public void OnItemPickedUp(BaseItem item)
    {
        // Special check: If we picked up a weapon, and we haven't counted a weapon yet? 
        // Actually, since we only count 1 weapon in Total, any weapon pickup counts as +1. 
        // If we swap weapons, Drop() defaults to -1, Pickup() defaults to +1, so it balances out.
        
        // Note: Check ShouldCountItem for non-weapons. For weapons, we always count (since 1 is required).
        bool isWeapon = item is WeaponItem;
        if (!isWeapon && !ShouldCountItem(item)) return;

        currentCollectedCount++;
        Debug.Log($"[GameProgress] Progress: {currentCollectedCount}/{totalItemsToCollect}");

        UpdateProgressUI();
        CheckWinCondition();
    }

    // Called when an item is dropped (put back in world)
    public void OnItemDropped(BaseItem item)
    {
        bool isWeapon = item is WeaponItem;
        if (!isWeapon && !ShouldCountItem(item)) return;

        currentCollectedCount--;
        if (currentCollectedCount < 0) currentCollectedCount = 0;
        
        Debug.Log($"[GameProgress] Item Dropped. Progress: {currentCollectedCount}/{totalItemsToCollect}");
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        if (progressUI != null)
        {
            progressUI.text = $"{currentCollectedCount} / {totalItemsToCollect}";
        }
    }

    private void CheckWinCondition()
    {
        if (currentCollectedCount >= totalItemsToCollect)
        {
            Debug.Log("[GameProgress] WIN CONDITION MET!");
            
            // Show Win UI
            if (winPanel != null) winPanel.SetActive(true);
            
            // Also show Hint
            HintUIManager.Instance?.ShowWin(winMessage);
        }
    }

    public void RestartGame()
    {
        Debug.Log("[GameProgress] Restarting Game...");
        SceneManager.LoadScene("StartScene");
    }
    public string GetProgressString()
    {
         return $"({currentCollectedCount}/{totalItemsToCollect})";
    }
}


