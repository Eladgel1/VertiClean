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
                        if (CleaningProgressUI.Instance != null)
                            CleaningProgressUI.Instance.ShowFeedback("Spray first!", Color.red);
                        return;
                    }

                    if (target != currentTarget)
                        currentTarget = target;

                    float preProgress = target.GetProgress();
                    target.TryClean(GetToolData().toolType, Time.deltaTime);
                    float postProgress = target.GetProgress();

                    float fill = Mathf.Clamp01(postProgress);
                    if (CleaningProgressUI.Instance != null)
                        CleaningProgressUI.Instance.ShowProgress(fill);

                    if (preProgress < 0.99f && postProgress >= 0.99f) {
                        currentTarget = null;
                        if (CleaningProgressUI.Instance != null)
                            CleaningProgressUI.Instance.HideProgressBar();
                    }

                    return;
                }
            }
        }

        if (currentTarget != null)
            currentTarget.StopScrubEffect();

        currentTarget = null;
        if (CleaningProgressUI.Instance != null)
            CleaningProgressUI.Instance.HideProgressBar();
    }

    public override void UseTool(RaycastHit hit) {
        // Not used directly
    }
}

