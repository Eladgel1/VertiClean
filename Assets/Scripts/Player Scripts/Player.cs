using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {
    public static Player Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.25f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.12f;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private bool movementEnabled = true;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (!movementEnabled) return;

        HandleMovement();
        HandleLook();
    }

    private void HandleMovement() {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        Vector2 input = VRInputManager.Instance.GetMoveVector();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        float currentSpeed = moveSpeed;
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            currentSpeed *= sprintMultiplier;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook() {
        Vector2 lookInput = VRInputManager.Instance.GetTurnVector();
        float sensitivity = mouseSensitivity;

        bool isThumbstick = Mouse.current == null || Mouse.current.delta.ReadValue().magnitude <= 0.01f;
        if (isThumbstick)
            lookInput *= 55f;

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void DisableMovement() => movementEnabled = false;
    public void EnableMovement() => movementEnabled = true;
}
