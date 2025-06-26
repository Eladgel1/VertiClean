using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System.Collections.Generic;

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
    private bool isWalkingNow = false;

    private PlayerTunnelingVignetteProvider vignetteProvider;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("[Player] Duplicate Player instance detected. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start() {
        vignetteProvider = FindFirstObjectByType<PlayerTunnelingVignetteProvider>();

        if (XRSettings.isDeviceActive) {
            XRSettings.eyeTextureResolutionScale = 1.25f;
        }

    }

    private void Update() {
        if (!movementEnabled) return;

        HandleMovement();
        HandleLook();

        if (ShouldStopVignette()) {
            vignetteProvider?.StopVignette();
        }
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

        bool moving = (input.x != 0 || input.y != 0);
        if (moving != isWalkingNow) {
            isWalkingNow = moving;
            SoundManager.Instance?.SetWalking(isWalkingNow);
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Trigger vignette only if using VR
        if (IsVRActive())
            vignetteProvider?.StartVignette();
    }

    private void HandleLook() {
        Vector2 lookInput = VRInputManager.Instance.GetTurnVector();

        float sensitivity = mouseSensitivity;

        // Scale thumbstick input so it behaves like mouse movement
        bool isThumbstick = Mouse.current == null || Mouse.current.delta.ReadValue().magnitude <= 0.01f;
        if (isThumbstick)
            lookInput *= 55f;

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Trigger vignette only if using VR
        if (IsVRActive())
            vignetteProvider?.StartVignette();
    }

    private bool ShouldStopVignette() {
        Vector2 move = VRInputManager.Instance.GetMoveVector();
        Vector2 turn = VRInputManager.Instance.GetTurnVector();
        return move == Vector2.zero && Mathf.Abs(turn.x) < 0.01f;
    }

    private bool IsVRActive() {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        foreach (var subsystem in subsystems) {
            if (subsystem.running)
                return true;
        }
        return false;
    }

    public void DisableMovement() => movementEnabled = false;
    public void EnableMovement() => movementEnabled = true;

    public void ApplyGameData() {
        Debug.Log($"[Player] Applying saved position: {GameData.Position}");
        controller = GetComponent<CharacterController>();
        if (controller != null) {
            controller.enabled = false;
            transform.position = GameData.Position;
            controller.enabled = true;
            Debug.Log("[Player] Position applied successfully.");
        }
        else {
            Debug.LogError("[Player] CharacterController not found!");
        }
    }

    public void ExportToGameData() {
        GameData.Position = transform.position;
        Debug.Log($"[Player] Exported position to GameData: {GameData.Position}");
    }
}



