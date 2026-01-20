using UnityEngine;
using TMPro;

/// <summary>
/// Displays drop instructions to the player
/// Shows "Click on item to drop" or "Press Q to drop [ItemName]"
/// </summary>
public class DropHintUI : MonoBehaviour
{
    public static DropHintUI Instance;

    [Header("UI References")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Text Messages")]
    [SerializeField] private string defaultMessage = "Click on item to drop";
    [SerializeField] private string dropMessage = "Press Q to drop {0}";

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Auto-find references if needed
        if (!hintText)
            hintText = GetComponentInChildren<TextMeshProUGUI>();

        if (!hintPanel)
            hintPanel = gameObject;

        // Start hidden
        Hide();
    }

    /// <summary>
    /// Shows the default message: "Click on item to drop"
    /// </summary>
    public void ShowDefault()
    {
        if (hintPanel) hintPanel.SetActive(true);
        
        if (hintText)
        {
            hintText.text = defaultMessage;
            hintText.color = defaultColor;
        }
    }

    /// <summary>
    /// Shows drop instruction with item name: "Press Q to drop [ItemName]"
    /// </summary>
    public void ShowDropInstruction(string itemName)
    {
        if (hintPanel) hintPanel.SetActive(true);
        
        if (hintText)
        {
            hintText.text = string.Format(dropMessage, itemName);
            hintText.color = selectedColor;
        }
    }

    /// <summary>
    /// Hides the hint UI
    /// </summary>
    public void Hide()
    {
        if (hintPanel) hintPanel.SetActive(false);
    }
}