using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public enum SaveActionType { Save, Load, Delete }

public class SaveMenuUI : MonoBehaviour {
    [Header("UI References")]
    [SerializeField] private GameObject[] slotButtons;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject enterNamePanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmNameButton;

    private SaveActionType currentAction;
    private int pendingSlotIndex = -1;
    private int selectedSlotIndex = 0;
    private bool cameFromGame = false;
    private bool inputRecentlyUsed = false;

    private void Awake() {
        enterNamePanel?.SetActive(false);
    }

    public void Open(SaveActionType action) {
        currentAction = action;
        inputRecentlyUsed = true;
        Invoke(nameof(EnableInput), 0.25f);

        cameFromGame = SceneManager.GetActiveScene().name == "GameScene";

        titleText.text = action switch {
            SaveActionType.Save => "Save Menu",
            SaveActionType.Load => "Load Menu",
            SaveActionType.Delete => "Delete Menu",
            _ => "Save Menu"
        };

        enterNamePanel.SetActive(false);

        if (MenuManager.Instance != null)
            MenuManager.Instance.ShowOnlyPanel(gameObject);

        for (int i = 0; i < slotButtons.Length; i++) {
            GameObject buttonObj = slotButtons[i];
            if (buttonObj == null) continue;

            Button btn = buttonObj.GetComponent<Button>();
            TextMeshProUGUI label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btn == null || label == null) continue;

            int slotIndex = i;
            btn.onClick.RemoveAllListeners();

            if (SaveManager.Instance != null && SaveManager.Instance.SlotExists(slotIndex)) {
                SaveData header = SaveManager.Instance.LoadSlotHeader(slotIndex);
                label.text = header != null
                    ? $"{header.playerName} - {header.saveTime}"
                    : $"Slot {slotIndex} (Corrupted)";

                if (currentAction != SaveActionType.Save)
                    btn.onClick.AddListener(() => OnSlotClicked(slotIndex));
            }
            else {
                label.text = $"Empty Slot No. {slotIndex + 1}";
                if (currentAction == SaveActionType.Save)
                    btn.onClick.AddListener(() => OnSlotClicked(slotIndex));
            }
        }

        selectedSlotIndex = 0;
        HighlightSlot(selectedSlotIndex);
        gameObject.SetActive(true);
    }

    private void EnableInput() => inputRecentlyUsed = false;

    private void HighlightSlot(int index) {
        if (index >= 0 && index < slotButtons.Length) {
            Button btn = slotButtons[index]?.GetComponent<Button>();
            btn?.Select();
        }
    }

    public void NavigateSlots(int direction) {
        int total = slotButtons.Length;
        selectedSlotIndex = Mathf.Clamp(selectedSlotIndex + direction, 0, total - 1);
        HighlightSlot(selectedSlotIndex);
    }

    public void ConfirmSlotSelection() {
        if (!inputRecentlyUsed) {
            if (selectedSlotIndex >= 0 && selectedSlotIndex < slotButtons.Length) {
                Button btn = slotButtons[selectedSlotIndex]?.GetComponent<Button>();
                btn?.onClick.Invoke();
            }
        }
    }

    public void OnSlotClicked(int index) {
        if (currentAction == SaveActionType.Save) {
            pendingSlotIndex = index;
            nameInput.text = "";
            enterNamePanel.SetActive(true);

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(nameInput.gameObject);

            nameInput.ActivateInputField();

            if (confirmNameButton != null) {
                confirmNameButton.onClick.RemoveAllListeners();
                confirmNameButton.onClick.AddListener(ConfirmPlayerName);
            }
            else {
                Debug.LogError("SaveMenuUI: confirmNameButton is null during OnSlotClicked.");
            }
        }
        else if (currentAction == SaveActionType.Load) {
            if (SaveManager.Instance != null && SaveManager.Instance.SlotExists(index)) {
                LoadBuffer.pendingSlotIndex = index; // defer the load until GameScene is ready
                GameData.LoadedFromSave = true;
                SceneController.LoadGameScene();      // actual loading happens in GameManager
            }
        }
        else if (currentAction == SaveActionType.Delete) {
            if (SaveManager.Instance != null && SaveManager.Instance.SlotExists(index)) {
                SaveManager.Instance.DeleteSlot(index);
                Open(currentAction); // Refresh UI
            }
        }
    }

    public void ConfirmPlayerName() {
        if (string.IsNullOrWhiteSpace(nameInput.text)) {
            Debug.LogWarning("Player name cannot be empty.");
            return;
        }

        if (pendingSlotIndex < 0) {
            Debug.LogError("SaveMenuUI: No slot selected to save.");
            return;
        }

        if (SaveManager.Instance == null) {
            Debug.LogError("SaveMenuUI: SaveManager.Instance is null.");
            return;
        }

        GameData.PlayerName = nameInput.text;

        if (Player.Instance != null)
            GameData.Position = Player.Instance.transform.position;

        SaveManager.Instance.SaveToSlot(pendingSlotIndex);

        enterNamePanel.SetActive(false);
        Close();
    }

    public IEnumerator WaitUntilReady() {
        yield return new WaitUntil(() =>
            Player.Instance != null &&
            StageManager.Instance != null &&
            SaveManager.Instance != null &&
            GameRestorer.Instance != null &&
            StatisticsManager.Instance != null
        );
    }

    public GameObject[] GetSlotButtons() => slotButtons;
    public GameObject GetEnterNamePanel() => enterNamePanel;
    public void CancelNameInput() => enterNamePanel.SetActive(false);
    public void CloseMenu() => Close();

    public void Close() {
        gameObject.SetActive(false);

        if (MenuManager.Instance != null) {
            if (cameFromGame) {
                Time.timeScale = 1f;
                if (Player.Instance != null) {
                    Player.Instance.EnableMovement();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else {
                MenuManager.Instance.OpenStartSubMenu();
            }
        }
    }
}


