using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputController : MonoBehaviour
{
    [Header("Input Values")]
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool SprintInput { get; private set; }

    
    public event Action OnPickupPressed;
    public event Action OnDropPressed;
    public event Action OnFirePressed;
    public event Action OnReloadPressed;
    public event Action OnInventoryToggle;
    public event Action OnJumpPressed;
    public event Action OnHelpToggle;

    [Header("Settings")]
    [SerializeField] private bool enableDebugLogs = false;

    // MOVEMENT INPUTS
    
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        
        if (enableDebugLogs)
            Debug.Log($"[Input] Move: {MoveInput}");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpInput = true;
            OnJumpPressed?.Invoke();
            
            if (enableDebugLogs)
                Debug.Log("[Input] Jump pressed");
        }
        else if (context.canceled)
        {
            JumpInput = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            SprintInput = true;
        else if (context.canceled)
            SprintInput = false;
    }

    // INTERACTION INPUTS

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Debug.Log("PICKUP INPUT RECEIVED (E PRESSED)");
        OnPickupPressed?.Invoke();
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Debug.Log("DROP INPUT RECEIVED (Q PRESSED)");
        OnDropPressed?.Invoke();
    }

    // COMBAT INPUTS

    public void OnFire(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        if (enableDebugLogs)
            Debug.Log("FIRE INPUT RECEIVED (Left Click)");
            
        OnFirePressed?.Invoke();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Debug.Log("RELOAD INPUT RECEIVED (R PRESSED)");
        OnReloadPressed?.Invoke();
    }

    // UI INPUTS

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Debug.Log("INVENTORY TOGGLE INPUT RECEIVED (Tab/I PRESSED)");
        OnInventoryToggle?.Invoke();
    }

    public void OnHelp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Debug.Log("HELP TOGGLE INPUT RECEIVED (H PRESSED)");
        OnHelpToggle?.Invoke();
    }

    // UTILITY METHODS

    public void ResetJumpInput()
    {
        JumpInput = false;
    }

    public void EnableDebugLogs(bool enable)
    {
        enableDebugLogs = enable;
    }

    // Called when script is disabled
    private void OnDisable()
    {
        // Reset all inputs
        MoveInput = Vector2.zero;
        LookInput = Vector2.zero;
        JumpInput = false;
        SprintInput = false;
    }
}