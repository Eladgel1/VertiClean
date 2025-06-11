using UnityEngine;
using TMPro;

public class UIInteractionPrompt : MonoBehaviour {
    public static UIInteractionPrompt Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays a message with default color (white), does not auto-hide.
    /// </summary>
    public void ShowMessage(string message) {
        ShowMessage(message, Color.white);
    }

    /// <summary>
    /// Displays a message with specified color, does not auto-hide.
    /// </summary>
    public void ShowMessage(string message, Color color) {
        if (promptText == null) {
            Debug.LogWarning("UIInteractionPrompt: promptText is not assigned.");
            return;
        }

        CancelInvoke(nameof(HideMessage));
        promptText.text = message;
        promptText.color = color;
        promptText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Displays a message with specified color and hides it after a given delay (in seconds).
    /// </summary>
    public void ShowMessage(string message, Color color, float duration) {
        ShowMessage(message, color);
        Invoke(nameof(HideMessage), duration);
    }

    /// <summary>
    /// Hides the currently displayed message.
    /// </summary>
    public void HideMessage() {
        if (promptText == null) return;

        promptText.text = "";
        promptText.gameObject.SetActive(false);
    }
}

