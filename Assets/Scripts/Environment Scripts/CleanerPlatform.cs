using UnityEngine;
using UnityEngine.XR;

public class CleanerPlatform : MonoBehaviour {
    public static CleanerPlatform Instance { get; private set; }

    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float groundLevel = 8.8f;

    private float maxHeight;
    private bool isMoving = false;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() {
        UpdateMaxHeight();
    }

    private void Update() {
        float vertical = 0f;
        bool liftingUp = VRInputManager.Instance.GetLiftUp() && transform.position.y < maxHeight;
        bool liftingDown = VRInputManager.Instance.GetLiftDown() && transform.position.y > groundLevel;

        if (liftingUp) vertical = 1f;
        if (liftingDown) vertical = -1f;

        Vector3 move = new Vector3(0f, vertical * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(move);

        // Movement and rattle detection
        bool isCurrentlyMoving = vertical != 0f;
        if (isMoving != isCurrentlyMoving) {
            isMoving = isCurrentlyMoving;
            SoundManager.Instance?.SetPlatformMoving(isMoving);
        }

        // Haptic Feedback
        if (isCurrentlyMoving) {
            HapticManager.Instance.StartHaptic(XRNode.LeftHand, 0.4f); // Medium-Light Vibration
            HapticManager.Instance.StartHaptic(XRNode.RightHand, 0.4f);
        }
        else {
            HapticManager.Instance.StopHaptic(XRNode.LeftHand);
            HapticManager.Instance.StopHaptic(XRNode.RightHand);
        }
    }

    private float GetMaxHeightForStage(int stage) {
        switch (stage) {
            case 1: return 17.3f;
            case 2: return 27.15f;
            case 3: return 37.8f;
            case 4: return 50.5f;
            default: return 100f;
        }
    }

    public void UpdateMaxHeight() {
        maxHeight = GetMaxHeightForStage(StageManager.Instance.GetCurrentStage());
    }
}


