using UnityEngine;

public class ItemHoverRotate : MonoBehaviour
{
    [Header("Hover Settings")]
    public float hoverHeight = 0.25f;
    public float hoverSpeed = 2f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Hover();
        Rotate();
    }

    private void Hover()
    {
        float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = new Vector3(
            startPos.x,
            startPos.y + yOffset,
            startPos.z
        );
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
