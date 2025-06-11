using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class CleaningToolBase : MonoBehaviour {
    [SerializeField] protected CleaningToolSO toolData;

    protected bool isHeld;
    protected Vector3 originalWorldPosition;

    protected Transform originalParent;
    protected Vector3 originalLocalPosition;
    protected Quaternion originalRotation;

    /// <summary>
    /// Caches initial position and parent of the tool.
    /// </summary>
    protected virtual void Awake() {
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
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
    /// Returns the original world-space position of the tool.
    /// </summary>
    public virtual Vector3 GetOriginalWorldPosition() => originalWorldPosition;

    /// <summary>
    /// Abstract method that all cleaning tools must implement for custom tool usage.
    /// </summary>
    public abstract void UseTool(RaycastHit hit);
}