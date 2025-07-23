using UnityEngine;
using UnityEngine.InputSystem;

public class VRInputManager : MonoBehaviour {
    public static VRInputManager Instance { get; private set; }

    public XRControls input;
    private Vector2 lastNavVector;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        input = new XRControls();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    public bool GetSprayHeld() =>
        input.Gameplay.Spray.IsPressed() || Mouse.current.leftButton.isPressed;

    public bool GetScrubButtonHeld() =>
        input.Gameplay.Spray.IsPressed() || Mouse.current.leftButton.isPressed;

    public bool GetPickupPressed() =>
        input.Gameplay.Pickup.WasPerformedThisFrame() || Keyboard.current.eKey.wasPressedThisFrame;

    public bool GetDropPressed() =>
        input.Gameplay.Drop.WasPerformedThisFrame() || Keyboard.current.fKey.wasPressedThisFrame;

    public bool GetLiftUp() =>
        input.Gameplay.LiftUp.IsPressed() || Keyboard.current.pageUpKey.isPressed;

    public bool GetLiftDown() =>
        input.Gameplay.LiftDown.IsPressed() || Keyboard.current.pageDownKey.isPressed;

    public bool GetUIBack() =>
        input.Gameplay.UI_Back.WasPerformedThisFrame() || Keyboard.current.escapeKey.wasPressedThisFrame;

    public bool GetUIClick() =>
        input.Gameplay.UI_Click.WasPerformedThisFrame() || Keyboard.current.enterKey.wasPressedThisFrame;

    public bool GetOpenMenu() =>
        input.Gameplay.OpenMenu.WasPerformedThisFrame() || Keyboard.current.escapeKey.wasPressedThisFrame;

    public bool GetContinuePressed() =>
        input.Gameplay.Continue.WasPerformedThisFrame() || Keyboard.current.enterKey.wasPressedThisFrame;

    public Vector2 GetUINavigationDelta() {
        Vector2 current = input.Gameplay.UI_Navigate.ReadValue<Vector2>();
        Vector2 delta = Vector2.zero;

        if (current != Vector2.zero && lastNavVector == Vector2.zero)
            delta = current;

        lastNavVector = current;
        return delta;
    }

    public Vector2 GetMoveVector() {
        Vector2 move = input.Gameplay.Move.ReadValue<Vector2>();

        if (Keyboard.current != null) {
            if (Keyboard.current.wKey.isPressed) move.y += 1;
            if (Keyboard.current.sKey.isPressed) move.y -= 1;
            if (Keyboard.current.aKey.isPressed) move.x -= 1;
            if (Keyboard.current.dKey.isPressed) move.x += 1;
        }

        return move;
    }

    public Vector2 GetTurnVector() {
        return input.Gameplay.Turn.ReadValue<Vector2>();
    }

    public Vector2 GetScrubVector() {
        return input.Gameplay.ScrubStain.ReadValue<Vector2>();
    }

    public Vector2 GetUINavigation() {
        return input.Gameplay.UI_Navigate.ReadValue<Vector2>();
    }
}
