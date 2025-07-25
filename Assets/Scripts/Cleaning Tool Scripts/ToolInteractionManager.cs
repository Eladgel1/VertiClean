using UnityEngine;

public class ToolInteractionManager : MonoBehaviour {
    public static ToolInteractionManager Instance { get; private set; }

    [SerializeField] private Transform playerHand;
    [SerializeField] private float pickupDistance = 3.2f;
    [SerializeField] private float placementDistance = 3f;
    [SerializeField] private LayerMask toolLayer;
    [SerializeField] private UIInteractionPrompt uiPrompt;

    private CleaningToolBase currentTool;
    private CleaningToolBase nearbyTool;
    private bool isTempMessageActive = false;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update() {
        if (!isTempMessageActive)
            DetectNearbyTool();

        HandleInput();
    }

    private void DetectNearbyTool() {
        nearbyTool = null;

        if (currentTool == null) {
            Collider[] hits = Physics.OverlapSphere(playerHand.position, pickupDistance, toolLayer);
            foreach (var hit in hits) {
                var tool = hit.GetComponent<CleaningToolBase>();
                if (tool != null && !tool.IsHeld()) {
                    nearbyTool = tool;
                    uiPrompt.ShowMessage($"Press [A] button to pick up a {tool.GetToolData().toolName}");
                    return;
                }
            }
        }

        if (currentTool != null) {
            float distanceToReturn = Vector3.Distance(playerHand.position, currentTool.GetOriginalWorldPosition());
            if (distanceToReturn < placementDistance) {
                uiPrompt.ShowMessage($"Press [B] button to drop the {currentTool.GetToolData().toolName}");
                return;
            }
        }

        uiPrompt.HideMessage();
    }

    private void HandleInput() {
        if (VRInputManager.Instance.GetPickupPressed() && currentTool == null) {
            if (nearbyTool != null) {
                PickUpTool(nearbyTool);
            }
            else {
                ShowTemporaryMessage("Move closer to pick up a tool.", 2f);
            }
        }

        if (VRInputManager.Instance.GetDropPressed() && currentTool != null) {
            float distanceToReturn = Vector3.Distance(playerHand.position, currentTool.GetOriginalWorldPosition());
            if (distanceToReturn < placementDistance) {
                DropCurrentTool();
            }
            else {
                ShowTemporaryMessage("Move closer to drop the tool.", 2f);
            }
        }
    }

    private void PickUpTool(CleaningToolBase tool) {
        currentTool = tool;
        tool.PickUp(playerHand);
        uiPrompt.HideMessage();
    }

    private void DropCurrentTool() {
        currentTool.Drop();
        currentTool = null;
        uiPrompt.HideMessage();
    }

    private void ShowTemporaryMessage(string message, float duration) {
        isTempMessageActive = true;
        uiPrompt.ShowMessage(message);
        CancelInvoke(nameof(ClearTemporaryMessage));
        Invoke(nameof(ClearTemporaryMessage), duration);
    }

    private void ClearTemporaryMessage() {
        isTempMessageActive = false;
        uiPrompt.HideMessage();
    }

    public bool HasActiveTool() => currentTool != null;
}

