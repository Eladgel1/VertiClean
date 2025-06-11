using UnityEngine;
using UnityEngine.XR;
using TMPro;
using System;

public class IntroManager : MonoBehaviour {
    public static IntroManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private GameObject introPanel;
    [SerializeField, TextArea(3, 10)] private string[] introMessages;

    private int currentIndex = 0;
    private bool isIntroRunning = true;
    private Action onContinueCallback = null;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        XRSettings.eyeTextureResolutionScale = 1.5f;
    }

    private void Start() {
        isIntroRunning = true;
        currentIndex = 0;
        ShowCurrentMessage();
        introPanel.SetActive(true);
        LockPlayer();
    }

    private void Update() {
        if (!isIntroRunning) return;

        if (VRInputManager.Instance.GetContinuePressed()) {
            if (onContinueCallback != null) {
                EndCustomMessage();
                return;
            }

            currentIndex++;
            if (currentIndex < introMessages.Length) {
                ShowCurrentMessage();
            }
            else {
                EndIntro();
            }
        }
    }

    public void ShowNewStageMessage(string message) {
        introText.text = message;
        introPanel.SetActive(true);
        isIntroRunning = true;
        onContinueCallback = null;
    }

    public void ShowNewStageMessage(string message, Action onContinue) {
        introText.text = message;
        introPanel.SetActive(true);
        isIntroRunning = true;
        onContinueCallback = onContinue;
        LockPlayer();
    }

    private void ShowCurrentMessage() {
        introText.text = introMessages[currentIndex];
        introPanel.SetActive(true);
    }

    public void EndIntro() {
        introPanel.SetActive(false);
        introText.text = "";
        isIntroRunning = false;
        UnlockPlayer();
    }

    private void EndCustomMessage() {
        introPanel.SetActive(false);
        introText.text = "";
        isIntroRunning = false;
        onContinueCallback?.Invoke();
        onContinueCallback = null;
    }

    private void LockPlayer() {
        if (Player.Instance != null)
            Player.Instance.DisableMovement();
    }

    private void UnlockPlayer() {
        if (Player.Instance != null)
            Player.Instance.EnableMovement();
    }
}

