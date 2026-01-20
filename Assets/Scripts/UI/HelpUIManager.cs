using UnityEngine;
using UnityEngine.UI;

public class HelpUIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The actual Help Panel GameObject that opens/closes")]
    [SerializeField] private GameObject helpPanel;
    
    [Tooltip("The button used to open the help menu")]
    [SerializeField] private Button helpMenuButton;

    // Optional: If you have a specific close button inside the Help Panel
    // [SerializeField] private Button closeButton; 

    private InputController inputController;
    private bool isHelpOpen = false;

    private void Awake()
    {
        inputController = FindFirstObjectByType<InputController>();
        
        // Ensure panel starts closed
        if (helpPanel != null) 
        {
            isHelpOpen = helpPanel.activeSelf;
            helpPanel.SetActive(isHelpOpen);
        }
    }

    private void Start()
    {
        // Listen for Keyboard Input
        if (inputController != null)
        {
            inputController.OnHelpToggle += HandleHelpToggle;
        }

        // Listen for UI Button Click
        if (helpMenuButton != null)
        {
            helpMenuButton.onClick.AddListener(OnHelpButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (inputController != null)
            inputController.OnHelpToggle -= HandleHelpToggle;

        if (helpMenuButton != null)
            helpMenuButton.onClick.RemoveListener(OnHelpButtonClicked);
    }

    // Called when 'H' is pressed
    private void HandleHelpToggle()
    {
        // Simulate a button click if available, otherwise toggle directly
        if (helpMenuButton != null)
        {
            helpMenuButton.onClick.Invoke();
        }
        else
        {
            ToggleHelpState();
        }
    }

    // Called when the UI Button is clicked (either by mouse or simulated by 'H' key)
    private void OnHelpButtonClicked()
    {
        ToggleHelpState();
    }

    private void ToggleHelpState()
    {
        isHelpOpen = !isHelpOpen;

        if (helpPanel != null)
        {
            helpPanel.SetActive(isHelpOpen);
        }
        
        // Optional: Pause game only if inventory isn't already handling it?
        // inventoryManager usually handles pause, so maybe we just show overlay here.
        Debug.Log($"Help Menu {(isHelpOpen ? "OPENED" : "CLOSED")}");
    }
}
