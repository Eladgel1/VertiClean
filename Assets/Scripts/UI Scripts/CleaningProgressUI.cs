using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CleaningProgressUI : MonoBehaviour {
    public static CleaningProgressUI Instance { get; private set; }

    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }

        if (progressBar != null) {
            HideProgressBar();
        }
        else {
            Debug.LogWarning("CleaningProgressUI: progressBar is null in Awake()");
        }

        if (feedbackText != null) {
            HideFeedback();
        }
        else {
            Debug.LogWarning("CleaningProgressUI: feedbackText is null in Awake()");
        }
    }

    public void ShowProgress(float progress) {
        if (progressBar == null) {
            Debug.LogWarning("CleaningProgressUI: ShowProgress called but progressBar is null");
            return;
        }

        progressBar.gameObject.SetActive(true);
        progressBar.value = Mathf.Clamp01(progress);
    }

    public void HideProgressBar() {
        if (progressBar != null) {
            progressBar.gameObject.SetActive(false);
        }
    }

    public void ShowFeedback(string message, Color color) {
        if (feedbackText == null) {
            Debug.LogWarning("CleaningProgressUI: ShowFeedback called but feedbackText is null");
            return;
        }

        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 3f);
    }

    private void HideFeedback() {
        if (feedbackText != null) {
            feedbackText.gameObject.SetActive(false);
        }
    }
}

