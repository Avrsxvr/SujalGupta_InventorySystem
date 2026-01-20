using UnityEngine;
using TMPro;

public class HintUIManager : MonoBehaviour
{
    public static HintUIManager Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Positions")]
    [SerializeField] private RectTransform nearItemPosition;
    [SerializeField] private RectTransform inventoryPosition;
    [SerializeField] private RectTransform warningPosition;
    [SerializeField] private RectTransform gameplayPosition;

    [Header("General Messages")]
    [TextArea] public string pickupHint = "Press E to pick up {0}";
    [TextArea] public string inventoryHint = "Select item to drop from inventory";
    [TextArea] public string dropHint = "Press Q to drop {0}";
    [TextArea] public string ammoDropBlocked = "Ammo cannot be dropped";
    [TextArea] public string weaponAlreadyHeld = "You already have a weapon";

    [TextArea] public string pickAmmoToUseGun = "Pick ammo to use the gun";
    [TextArea] public string pickGunToUseAmmo = "Pick a gun to use ammo";
    [TextArea] public string pressFireToShoot = "Press TAB to fire";
    
    [Header("Success Messages")]
    [TextArea] public string pickupSuccessMessage = "Picked up {0}";

    [Header("Auto Hide")]
    [SerializeField] private float autoHideDelay = 5f;

    private RectTransform rect;
    private float timer;
    private bool timerActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        rect = hintText.GetComponent<RectTransform>();
        Hide();
    }

    private void Update()
    {
        if (!timerActive || autoHideDelay <= 0f) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Hide();
    }

    // ---------- PUBLIC API ----------
    public void ShowPickup(string itemName)
    {
        // Safe check: if Inspector value doesn't have {0}, append it
        string fmt = pickupHint.Contains("{0}") ? pickupHint : pickupHint + " {0}";
        Show(string.Format(fmt, itemName), nearItemPosition, false);
    }

    public void ShowInventory() => Show(inventoryHint, inventoryPosition, false);

    public void ShowDrop(string itemName)
    {
        // Safe check: if Inspector value doesn't have {0}, append it
        string fmt = dropHint.Contains("{0}") ? dropHint : dropHint + " {0}";
        Show(string.Format(fmt, itemName), inventoryPosition, false);
    }
    
    public void ShowAmmoBlocked() => Show(ammoDropBlocked, warningPosition, true); // Toast
    public void ShowWeaponAlreadyHeld() => Show(weaponAlreadyHeld, warningPosition, true); // Toast

    // NEW API
    public void ShowPickAmmoToUseGun() => Show(pickAmmoToUseGun, gameplayPosition, false);
    public void ShowPickGunToUseAmmo() => Show(pickGunToUseAmmo, gameplayPosition, false);
    public void ShowPressFire() => Show(pressFireToShoot, gameplayPosition, false);

    // Centralized Pickup Success Message
    public void ShowPickupSuccess(string itemName)
    {
        // Safe check: if Inspector value doesn't have {0}, append it
        string fmt = pickupSuccessMessage.Contains("{0}") ? pickupSuccessMessage : pickupSuccessMessage + " {0}";
        Show(string.Format(fmt, itemName), nearItemPosition, true);
    }

    // WIN MESSAGE
    public void ShowWin(string message)
    {
        Show(message, gameplayPosition, false); // Persistent
    }

    public void Hide()
    {
        timerActive = false;
        hintText.text = "";
        hintText.gameObject.SetActive(false);
    }

    private void Show(string msg, RectTransform target, bool autoHide)
    {
        hintText.text = msg;
        hintText.gameObject.SetActive(true);

        if (target != null)
            rect.position = target.position;

        timerActive = autoHide;
        if (autoHide && autoHideDelay > 0f)
        {
            timer = autoHideDelay;
        }
    }
}
