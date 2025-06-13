using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

public class HapticManager : MonoBehaviour {
    public static HapticManager Instance { get; private set; }

    private InputDevice leftController;
    private InputDevice rightController;

    private bool leftHapticActive = false;
    private bool rightHapticActive = false;
    private float leftAmplitude = 0f;
    private float rightAmplitude = 0f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        StartCoroutine(EnsureControllersInitialized());
    }

    private IEnumerator EnsureControllersInitialized() {
        int retries = 0;
        const int maxRetries = 20;

        while (retries < maxRetries) {
            InitializeControllers();

            if (leftController.isValid && rightController.isValid) {
                Debug.Log("[HapticManager] Initialized controllers: Left - works, Right - works");
                yield break;
            }

            retries++;
            yield return new WaitForSeconds(0.25f);
        }

        Debug.LogWarning("[HapticManager] Failed to initialize both controllers after multiple retries.");
    }

    private void Update() {
        if (leftHapticActive && leftController.isValid)
            leftController.SendHapticImpulse(0u, leftAmplitude, Time.deltaTime);

        if (rightHapticActive && rightController.isValid)
            rightController.SendHapticImpulse(0u, rightAmplitude, Time.deltaTime);
    }

    private void InitializeControllers() {
        var leftDevices = new List<InputDevice>();
        var rightDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightDevices);

        if (leftDevices.Count > 0) {
            leftController = leftDevices[0];
            Debug.Log("[HapticManager] Left controller initialized.");
        }

        if (rightDevices.Count > 0) {
            rightController = rightDevices[0];
            Debug.Log("[HapticManager] Right controller initialized.");
        }
    }

    public void StartHaptic(XRNode hand, float intensity) {
        intensity = Mathf.Clamp01(intensity);

        if (hand == XRNode.LeftHand && leftController.isValid) {
            leftHapticActive = true;
            leftAmplitude = intensity;
        }
        else if (hand == XRNode.RightHand && rightController.isValid) {
            rightHapticActive = true;
            rightAmplitude = intensity;
        }
    }

    public void StopHaptic(XRNode hand) {
        if (hand == XRNode.LeftHand) {
            leftHapticActive = false;
            if (leftController.isValid) leftController.StopHaptics();
        }
        else if (hand == XRNode.RightHand) {
            rightHapticActive = false;
            if (rightController.isValid) rightController.StopHaptics();
        }
    }

    public InputDevice GetRightController() => rightController;

    public InputDevice GetLeftController() => leftController;

    public void StopAllHaptics() {
        StopHaptic(XRNode.LeftHand);
        StopHaptic(XRNode.RightHand);
    }
}
