using UnityEngine;

/// <summary>
/// Represents a cleanable stain in the scene.
/// Includes spray logic, cleaning logic, and unique ID for save/load tracking.
/// </summary>
public class CleaningTarget : MonoBehaviour {
    [Header("Cleaning Settings")]
    [SerializeField] private ToolType requiredTool;
    [SerializeField] private int cleanHitsRequired = 15;
    [SerializeField] private SpriteRenderer dirtRenderer;
    [SerializeField] private ParticleSystem scrubEffect;

    private float currentCleanProgress = 0f;
    private bool isClean = false;
    private bool sprayed = false;

    private void Start() {
        if (dirtRenderer == null)
            dirtRenderer = GetComponent<SpriteRenderer>();

        if (dirtRenderer == null)
            Debug.LogWarning($"CleaningTarget '{name}' is missing a SpriteRenderer.");
    }

    public void TryClean(ToolType toolUsed, float progressTime) {
        if (isClean || !sprayed || toolUsed != requiredTool) return;

        float secondsToClean = 10f;
        float scrubSpeed = cleanHitsRequired / secondsToClean;
        currentCleanProgress += progressTime * scrubSpeed;

        float progress = Mathf.Clamp01(currentCleanProgress / cleanHitsRequired);
        UpdateTransparency(1f - progress);
        CleaningProgressUI.Instance?.ShowProgress(progress);

        if (scrubEffect != null && !scrubEffect.isPlaying) {
            scrubEffect.transform.position = transform.position;
            scrubEffect.Play();
        }

        if (progress >= 0.99f) {
            CompleteCleaning();
        }
    }

    private void UpdateTransparency(float alpha) {
        if (dirtRenderer == null) return;

        Color color = dirtRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        dirtRenderer.color = color;
    }

    public float GetProgress() => Mathf.Clamp01(currentCleanProgress / cleanHitsRequired);
    public void MarkSprayed() => sprayed = true;
    public bool WasSprayed() => sprayed;
    public ToolType GetRequiredTool() => requiredTool;

    public void StopScrubEffect() {
        if (scrubEffect != null && scrubEffect.isPlaying)
            scrubEffect.Stop();
    }

    public void CompleteCleaning() {
        isClean = true;

        StopScrubEffect();
        CleaningProgressUI.Instance?.HideProgressBar();
        Destroy(gameObject);
    }
}
