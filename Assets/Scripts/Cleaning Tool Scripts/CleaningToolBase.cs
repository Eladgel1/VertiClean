using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CleaningToolBase : MonoBehaviour {
    [SerializeField] protected CleaningToolSO toolData;

    protected bool isHeld;
    protected Vector3 originalWorldPosition;

    protected Transform originalParent;
    protected Vector3 originalLocalPosition;
    protected Quaternion originalRotation;

    private Transform platformTransform;
    private Vector3 localPositionRelativeToPlatform;

    /// <summary>
    /// Caches initial position and parent of the tool.
    /// </summary>
    protected virtual void Awake() {
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        platformTransform = CleanerPlatform.Instance?.transform;
        if (platformTransform != null)
            localPositionRelativeToPlatform = platformTransform.InverseTransformPoint(transform.position);
        else
            originalWorldPosition = transform.position;
    }

    /// <summary>
    /// Returns the tool's data asset.
    /// </summary>
    public CleaningToolSO GetToolData() => toolData;

    /// <summary>
    /// Picks up the tool and attaches it to the player's hand.
    /// </summary>
    public virtual void PickUp(Transform playerHand) {
        isHeld = true;
        transform.SetParent(playerHand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (platformTransform == null)
            platformTransform = CleanerPlatform.Instance?.transform;

        if (platformTransform != null)
            localPositionRelativeToPlatform = platformTransform.InverseTransformPoint(transform.position);

        originalWorldPosition = transform.position;
    }

    /// <summary>
    /// Drops the tool back to its original location.
    /// </summary>
    public virtual void Drop() {
        isHeld = false;
        transform.SetParent(originalParent);
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalRotation;
    }

    /// <summary>
    /// Returns whether the tool is currently being held by the player.
    /// </summary>
    public virtual bool IsHeld() => isHeld;

    /// <summary>
    /// Returns the adjusted world position where the tool was picked up, relative to the platform.
    /// </summary>
    public virtual Vector3 GetOriginalWorldPosition() {
        if (platformTransform != null)
            return platformTransform.TransformPoint(localPositionRelativeToPlatform);

        return originalWorldPosition;
    }

    /// <summary>
    /// Abstract method that all cleaning tools must implement for custom tool usage.
    /// </summary>
    public abstract void UseTool(RaycastHit hit);
}
