/*using UnityEngine;

public class SpongeTool : CleaningToolBase {
    [SerializeField] private LayerMask dirtLayer;

    private CleaningTarget currentTarget;
    private float movementThreshold = 0.01f;

    private void Update() {
        if (!IsHeld()) return;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float mouseMovement = mouseDelta.magnitude;

        if (Input.GetMouseButton(0)) {
            if (mouseMovement < movementThreshold) {
                if (currentTarget != null)
                    currentTarget.StopScrubEffect();

                CleaningProgressUI.Instance.HideProgressBar();
                SoundManager.Instance?.SetSponging(false, IsMop());
                return;
            }

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
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    SoundManager.Instance?.SetSponging(true, IsMop());

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        ShowRandomPraise();
                        currentTarget = null;
                    }

                    return;
                }
            }
        }

        if (currentTarget != null)
            currentTarget.StopScrubEffect();

        currentTarget = null;
        CleaningProgressUI.Instance.HideProgressBar();
        SoundManager.Instance?.SetSponging(false, IsMop());
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
}*/

/*using UnityEngine;
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
                        HapticManager.Instance.StopHaptic(XRNode.RightHand);
                        HapticManager.Instance.StopHaptic(XRNode.LeftHand);
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    SoundManager.Instance?.SetSponging(true, IsMop());

                    // Start Haptic During Scrubbing
                    float vibrationStrength = IsMop() ? 0.9f : 0.55f;
                    HapticManager.Instance.StartHaptic(XRNode.RightHand, vibrationStrength);
                    HapticManager.Instance.StartHaptic(XRNode.LeftHand, vibrationStrength);

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        ShowRandomPraise();
                        currentTarget = null;
                    }

                    return;
                }
            }
        }

        // Stop everything if not scrubbing
        if (currentTarget != null)
            currentTarget.StopScrubEffect();

        currentTarget = null;
        CleaningProgressUI.Instance.HideProgressBar();
        SoundManager.Instance?.SetSponging(false, IsMop());
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
}*/


/*using UnityEngine;
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

                // Haptic should occur even before we check spray
                float vibrationStrength = IsMop() ? 0.9f : 0.55f;
                HapticManager.Instance.StartHaptic(XRNode.RightHand, vibrationStrength);
                Debug.Log("[SpongeTool] Triggering Haptic - IsMop: " + IsMop());
                HapticManager.Instance.StartHaptic(XRNode.LeftHand, vibrationStrength);

                if (target != null) {
                    if (!target.WasSprayed()) {
                        CleaningProgressUI.Instance.ShowFeedback("Spray first!", Color.red);
                        target.StopScrubEffect();
                        SoundManager.Instance?.SetSponging(false, IsMop());
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    SoundManager.Instance?.SetSponging(true, IsMop());

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        ShowRandomPraise();
                        currentTarget = null;
                    }

                    return;
                }
            }
        }

        // Stop everything if not scrubbing
        if (currentTarget != null)
            currentTarget.StopScrubEffect();

        currentTarget = null;
        CleaningProgressUI.Instance.HideProgressBar();
        SoundManager.Instance?.SetSponging(false, IsMop());
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
}*/

/*using UnityEngine;
using UnityEngine.XR;

public class SpongeTool : CleaningToolBase {
    [SerializeField] private LayerMask dirtLayer;

    private CleaningTarget currentTarget;
    private float movementThreshold = 0.01f;
    private bool isActivelyCleaning = false;

    private void Update() {
        if (!IsHeld()) return;

        bool isPressing = VRInputManager.Instance.GetScrubButtonHeld();
        Vector2 scrubMovement = VRInputManager.Instance.GetScrubVector();
        bool isMoving = scrubMovement.magnitude > movementThreshold;
        bool wantsToScrub = isPressing && isMoving;

        bool isActuallyScrubbing = false;

        if (wantsToScrub) {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, dirtLayer)) {
                var target = hit.collider.GetComponent<CleaningTarget>()
                    ?? hit.collider.GetComponentInParent<CleaningTarget>()
                    ?? hit.collider.GetComponentInChildren<CleaningTarget>();

                if (target != null) {
                    if (!target.WasSprayed()) {
                        CleaningProgressUI.Instance.ShowFeedback("Spray first!", Color.red);
                        target.StopScrubEffect();
                        SoundManager.Instance?.SetSponging(false, IsMop());
                        StopHapticsIfNeeded();
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    SoundManager.Instance?.SetSponging(true, IsMop());

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        ShowRandomPraise();
                        currentTarget = null;
                    }

                    isActuallyScrubbing = true;
                }
            }
        }

        // Handle Haptics Only When Cleaning a Real Target
        if (isActuallyScrubbing) {
            float vibrationStrength = IsMop() ? 0.9f : 0.55f;

            var left = HapticManager.Instance.GetLeftController();
            var right = HapticManager.Instance.GetRightController();

            if (left.isValid)
                left.SendHapticImpulse(0u, vibrationStrength, Time.deltaTime);
            if (right.isValid)
                right.SendHapticImpulse(0u, vibrationStrength, Time.deltaTime);

            if (!isActivelyCleaning) {
                Debug.Log("[SpongeTool] Haptic Started");
                isActivelyCleaning = true;
            }
        }
        else {
            StopHapticsIfNeeded();
        }

        // Reset feedback if not scrubbing or no target
        if (currentTarget != null && !isActuallyScrubbing)
            currentTarget.StopScrubEffect();

        if (!isActuallyScrubbing) {
            currentTarget = null;
            CleaningProgressUI.Instance.HideProgressBar();
            SoundManager.Instance?.SetSponging(false, IsMop());
        }
    }

    private void StopHapticsIfNeeded() {
        if (isActivelyCleaning) {
            HapticManager.Instance.StopHaptic(XRNode.RightHand);
            HapticManager.Instance.StopHaptic(XRNode.LeftHand);
            Debug.Log("[SpongeTool] Haptic Stopped");
            isActivelyCleaning = false;
        }
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
}*/


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

