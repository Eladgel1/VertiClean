using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyboard : MonoBehaviour {
    private TMP_InputField inputField;

    [Header("The panel GameObject with all save slot buttons + input field")]
    [SerializeField] private GameObject panelToDisable;

    [Header("Visual input field shown above the virtual keyboard")]
    [SerializeField] private TMP_InputField visualKeyboardInputField;

    [Header("The GameObject root of the visual keyboard input")]
    [SerializeField] private GameObject visualInputRoot;

    private bool keyboardOpen = false;

    void Start() {
        inputField = GetComponent<TMP_InputField>();

#if UNITY_ANDROID && !UNITY_EDITOR
        inputField.onSelect.AddListener((text) => OpenKeyboard());
#endif

        if (NonNativeKeyboard.Instance != null) {
            NonNativeKeyboard.Instance.OnClosed += OnKeyboardClosed;
            NonNativeKeyboard.Instance.OnTextUpdated += OnKeyboardTextUpdated;
        }

        if (visualInputRoot != null) {
            visualInputRoot.SetActive(false);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        if (VRInputManager.Instance != null) {
            VRInputManager.Instance.input.Gameplay.UI_Back.performed += ctx => {
                if (keyboardOpen) {
                    NonNativeKeyboard.Instance.Close();
                }
            };
        }
#endif
    }

    private void OnDestroy() {
        if (NonNativeKeyboard.Instance != null) {
            NonNativeKeyboard.Instance.OnClosed -= OnKeyboardClosed;
            NonNativeKeyboard.Instance.OnTextUpdated -= OnKeyboardTextUpdated;
        }
    }

    public void OpenKeyboard() {
        if (keyboardOpen) return;
        keyboardOpen = true;

        if (NonNativeKeyboard.Instance == null) {
            Debug.LogError("NonNativeKeyboard instance is null.");
            return;
        }

        if (visualKeyboardInputField != null) {
            visualKeyboardInputField.text = inputField.text;
            visualKeyboardInputField.ActivateInputField();
        }

        if (visualInputRoot != null) {
            visualInputRoot.SetActive(true);
        }

        NonNativeKeyboard.Instance.gameObject.SetActive(true);
        NonNativeKeyboard.Instance.InputField = visualKeyboardInputField;
        NonNativeKeyboard.Instance.PresentKeyboard(visualKeyboardInputField.text);

        if (panelToDisable != null) {
            panelToDisable.SetActive(false);
        }
    }

    private void OnKeyboardTextUpdated(string newText) {
        if (visualKeyboardInputField != null) {
            visualKeyboardInputField.text = newText;
        }
    }

    private void OnKeyboardClosed(object sender, System.EventArgs e) {
        CloseKeyboard();
    }

    private void CloseKeyboard() {
        keyboardOpen = false;

        if (visualKeyboardInputField != null && inputField != null) {
            inputField.text = visualKeyboardInputField.text;
        }

        if (visualInputRoot != null) {
            visualInputRoot.SetActive(false);
        }

        if (panelToDisable != null) {
            panelToDisable.SetActive(true);
        }
    }
}



