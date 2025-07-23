using UnityEngine;
using UnityEngine.XR;

public class SpongeTool : CleaningToolBase {
    [SerializeField] private LayerMask dirtLayer;

    private CleaningTarget currentTarget;
    private float movementThreshold = 0.01f;

    private void Update() {
        if (!IsHeld()) return;

        bool isPressing = VRInputManager.Instance.GetScrubButtonHeld();
        Vector2 scrubMovement = VRInputManager.Instance.GetScrubVector();
        bool isMoving = scrubMovement.magnitude > movementThreshold;
        bool isScrubbing = isPressing && isMoving;

        if (isScrubbing) {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, dirtLayer)) {
                var direct = hit.collider.GetComponent<CleaningTarget>();
                var inParent = hit.collider.GetComponentInParent<CleaningTarget>();
                var inChild = hit.collider.GetComponentInChildren<CleaningTarget>();
                var target = direct ?? inParent ?? inChild;

                if (target != null) {
                    if (!target.WasSprayed()) {
                        CleaningProgressUI.Instance.ShowFeedback("Spray first!", Color.red);
                        target.StopScrubEffect();
                        SoundManager.Instance?.SetSponging(false, IsMop());
                        StopHaptics();
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    SoundManager.Instance?.SetSponging(true, IsMop());

                    // Trigger continuous haptic while cleaning is active
                    float vibrationStrength = IsMop() ? 0.9f : 0.55f;
                    var left = HapticManager.Instance.GetLeftController();
                    var right = HapticManager.Instance.GetRightController();

                    if (left.isValid)
                        left.SendHapticImpulse(0u, vibrationStrength, Time.deltaTime);
                    if (right.isValid)
                        right.SendHapticImpulse(0u, vibrationStrength, Time.deltaTime);

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        ShowRandomPraise();
                        currentTarget = null;
                    }

                    return;
                }
            }
        }

        // Stop everything if not scrubbing a valid target
        if (currentTarget != null)
            currentTarget.StopScrubEffect();

        currentTarget = null;
        CleaningProgressUI.Instance.HideProgressBar();
        SoundManager.Instance?.SetSponging(false, IsMop());
        StopHaptics();
    }

    private void StopHaptics() {
        HapticManager.Instance.StopHaptic(XRNode.RightHand);
        HapticManager.Instance.StopHaptic(XRNode.LeftHand);
    }

    private bool IsMop() {
        var data = GetToolData();
        return data.toolType.ToString().ToLower().Contains("mop");
    }

    private void ShowRandomPraise() {
        string[] messages = { "Great!", "Awesome!", "Amazing!", "Fantastic!", "Incredible!" };
        int index = Random.Range(0, messages.Length);
        CleaningProgressUI.Instance.ShowFeedback(messages[index], Color.cyan);
    }

    public override void UseTool(RaycastHit hit) {
        // Not used directly — logic handled in Update
    }
}

