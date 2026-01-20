using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button

public class NavigatingtoScene : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The exact name of the scene to load (check Build Settings)")]
    [SerializeField] private string sceneToLoad = "SampleScene";

    private Button myButton;

    private void Start()
    {
        // 1. Try to find a Button on this same GameObject
        myButton = GetComponent<Button>();

        if (myButton != null)
        {
            // 2. Automatically add the click listener
            myButton.onClick.AddListener(StartGame);
            Debug.Log("[NavigatingtoScene] Auto-linked Button to StartGame!");
        }
        else
        {
            Debug.LogWarning("[NavigatingtoScene] No Button component found on this object. Please attach to a Button or link OnClick manually.");
        }
    }

    private void OnDestroy()
    {
        if (myButton != null)
            myButton.onClick.RemoveListener(StartGame);
    }

    public void StartGame()
    {
        Debug.Log($"[NavigatingtoScene] Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}
