using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CleaningProgressUI : MonoBehaviour {
    public static CleaningProgressUI Instance { get; private set; }

    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI feedbackText;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        HideProgressBar();
        HideFeedback();
    }

    public void ShowProgress(float progress) {
        if (progressBar == null) return; 

        progressBar.gameObject.SetActive(true);
        progressBar.value = Mathf.Clamp01(progress);
    }

    public void HideProgressBar() {
        progressBar.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message, Color color) {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 3f);
    }

    private void HideFeedback() {
        feedbackText.gameObject.SetActive(false);
    }
}
