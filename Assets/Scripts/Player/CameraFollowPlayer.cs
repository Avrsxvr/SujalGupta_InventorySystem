using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float followSpeed = 10f;

    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    private float currentRotationY;
    private float currentRotationX = 15f;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentRotationY = angles.y;
        currentRotationX = angles.x;
    }

    private void LateUpdate()
    {
        // ⛔ Stop camera when inventory is open
        if (UIManager.Instance != null && UIManager.Instance.IsInventoryOpen)
            return;

        HandleCameraRotation();
        FollowPlayer();
    }

    private void HandleCameraRotation()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        currentRotationY += delta.x * mouseSensitivity * 0.02f;
        currentRotationX -= delta.y * mouseSensitivity * 0.02f;

        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
    }

    private void FollowPlayer()
    {
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        transform.position = Vector3.Lerp(transform.position, target.position + offset, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height);
    }
}
