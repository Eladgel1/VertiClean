using UnityEngine;

public class SprayTool : CleaningToolBase {
    [SerializeField] private float requiredSprayTime = 2f;
    [SerializeField] private LayerMask dirtLayer;

    private float currentSprayTime = 0f;
    private CleaningTarget currentTarget;

    private void Update() {
        if (!IsHeld()) return;

        if (VRInputManager.Instance.GetSprayHeld()) {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, dirtLayer)) {
                var target = hit.collider.GetComponent<CleaningTarget>();
                if (target != null && !target.WasSprayed()) {
                    if (target != currentTarget) {
                        currentTarget = target;
                        currentSprayTime = 0f;
                    }

                    currentSprayTime += Time.deltaTime;

                    float progress = currentSprayTime / requiredSprayTime;
                    CleaningProgressUI.Instance.ShowProgress(progress);

                    if (progress >= 1f) {
                        target.MarkSprayed();
                        currentTarget = null;
                        CleaningProgressUI.Instance.HideProgressBar();
                    }

                    return;
                }
            }
        }

        currentTarget = null;
        CleaningProgressUI.Instance.HideProgressBar();
    }

    public override void UseTool(RaycastHit hit) {
        // Not used directly
    }
}
