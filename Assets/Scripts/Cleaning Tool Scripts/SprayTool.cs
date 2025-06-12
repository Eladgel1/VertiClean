using UnityEngine;

public class SprayTool : CleaningToolBase {
    [SerializeField] private ParticleSystem sprayEffect;
    [SerializeField] private float requiredSprayTime = 2f;
    [SerializeField] private LayerMask dirtLayer;
    [SerializeField] private Transform effectOrigin;

    private float currentSprayTime = 0f;
    private CleaningTarget currentTarget;
    private bool isSprayingNow = false;

    private void Update() {
        if (!IsHeld()) {
            StopSpraySound();
            return;
        }

        if (VRInputManager.Instance.GetSprayHeld()) {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, dirtLayer)) {
                var target = hit.collider.GetComponent<CleaningTarget>();
                if (target != null && !target.WasSprayed()) {
                    if (target != currentTarget) {
                        currentTarget = target;
                        currentSprayTime = 0f;
                    }

                    StartSpraySound();

                    currentSprayTime += Time.deltaTime;

                    // Update visual spray effect
                    if (sprayEffect != null) {
                        if (effectOrigin != null) {
                            sprayEffect.transform.position = effectOrigin.position;
                            sprayEffect.transform.rotation = effectOrigin.rotation;
                        }
                        else {
                            sprayEffect.transform.position = transform.position;
                            sprayEffect.transform.rotation = transform.rotation;
                        }

                        if (!sprayEffect.isPlaying) {
                            sprayEffect.Play();
                        }
                    }

                    float progress = currentSprayTime / requiredSprayTime;
                    CleaningProgressUI.Instance.ShowProgress(progress);

                    if (progress >= 1f) {
                        target.MarkSprayed();
                        currentTarget = null;
                        CleaningProgressUI.Instance.HideProgressBar();
                        if (sprayEffect != null && sprayEffect.isPlaying)
                            sprayEffect.Stop();
                    }

                    return;
                }
            }
        }

        // Stop visual and audio when not hitting or mouse released
        if (sprayEffect != null && sprayEffect.isPlaying)
            sprayEffect.Stop();

        StopSpraySound();
        CleaningProgressUI.Instance.HideProgressBar();
    }

    private void StartSpraySound() {
        if (!isSprayingNow) {
            SoundManager.Instance?.SetSpraying(true);
            isSprayingNow = true;
        }
    }

    private void StopSpraySound() {
        if (isSprayingNow) {
            SoundManager.Instance?.SetSpraying(false);
            isSprayingNow = false;
        }
    }

    public override void UseTool(RaycastHit hit) {
        // Not used directly
    }
}


