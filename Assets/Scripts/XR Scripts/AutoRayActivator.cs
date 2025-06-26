using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoRayActivator : MonoBehaviour {
    [Header("XR Controller Objects")]
    [SerializeField] private GameObject leftHandObject;
    [SerializeField] private GameObject rightHandObject;

    [Header("Panels to Monitor")]
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject enterNamePanel;

    private XRInteractorLineVisual leftRay;
    private XRInteractorLineVisual rightRay;

    void Start() {
        leftRay = leftHandObject.GetComponent<XRInteractorLineVisual>();
        rightRay = rightHandObject.GetComponent<XRInteractorLineVisual>();
    }

    void Update() {
        bool shouldEnable = optionsMenuPanel.activeSelf || enterNamePanel.activeSelf;

        if (enterNamePanel.activeSelf && VRInputManager.Instance != null && VRInputManager.Instance.GetUIBack()) {
            enterNamePanel.SetActive(false);
            shouldEnable = false;
        }

        if (leftRay != null) leftRay.enabled = shouldEnable;
        if (rightRay != null) rightRay.enabled = shouldEnable;
    }
}
