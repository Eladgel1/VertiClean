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

        XRSettings.eyeTextureResolutionScale = 1.25f;
    }

    private void Start() {
        ShowStageIntro(GameData.Level);
        Debug.Log($"[IntroManager] Init check — Level: {GameData.Level}, ReplayStage: {GameData.ReplayStage}, Replay: {GameData.StartedFromReplay}, FromSave: {GameData.LoadedFromSave}, ShowIntro: {GameData.ShowIntro}");
    }

    private void Update() {
        if (!isIntroRunning) return;

        if (VRInputManager.Instance.GetContinuePressed()) {
            if (onContinueCallback != null) {
                EndCustomMessage();
                return;
            }

            currentIndex++;
            if (currentIndex < introMessages.Length && GameData.Level == 1 && GameData.ShowIntro) {
                ShowCurrentMessage();
            }
            else {
                if (GameData.Level == 1) StatisticsManager.Instance.BeginSession(1);
                EndIntro();
            }
        }
    }

    public void ShowStageIntro(int stageNumber) {
        if (stageNumber > 1 || !GameData.ShowIntro || GameData.LoadedFromSave || GameData.StartedFromReplay) {
            introPanel.SetActive(false);
            introText.text = "";
            isIntroRunning = false;
            UnlockPlayer();
            if (stageNumber == 1 && GameData.LoadedFromSave)
                StatisticsManager.Instance.BeginSession(1);
            Debug.Log("[IntroManager] Intro skipped by ShowStageIntro conditions.");
            return;
        }

        isIntroRunning = true;
        currentIndex = stageNumber - 1;
        ShowCurrentMessage();
        introPanel.SetActive(true);
        LockPlayer();
        Debug.Log("[IntroManager] Showing Stage Intro.");
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
        GameData.ShowIntro = false;
        UnlockPlayer();
        Debug.Log("[IntroManager] Intro ended and ShowIntro set to false.");
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
