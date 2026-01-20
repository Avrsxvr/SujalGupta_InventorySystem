using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4.5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    public InputController inputController;
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private int speedHash;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (!inputController)
            inputController = FindFirstObjectByType<InputController>();

        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;

        if (!animator)
            animator = GetComponentInChildren<Animator>();

        speedHash = Animator.StringToHash("Speed");
    }

    private void Update()
    {
        Move();
        ApplyGravity();
        UpdateAnimation();
    }

    private void Move()
    {
        Vector2 input = inputController.MoveInput;
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        Vector3 horizontalVelocity = new Vector3(
            controller.velocity.x, 0, controller.velocity.z);

        animator.SetFloat(speedHash, horizontalVelocity.magnitude);
    }
}