using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuManager : MonoBehaviour {
    public static MenuManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject startSubMenuPanel;
    public GameObject previousStagesPanel;
    public GameObject statisticsPanel;
    public GameObject optionsMenuPanel;
    public GameObject quitConfirmationPanel;
    public GameObject pauseMenuPanel;
    public SaveMenuUI saveMenuUI;

    [Header("State")]
    public bool isInGame = false;
    private GameObject currentPanel;
    private int selectedIndex = 0;
    private bool inputLocked = false;
    private bool allowQuitInput = false;

    [Header("UI Navigation")]
    public Button[] mainButtons;
    public Button[] startSubButtons;
    public Button[] pauseButtons;
    public Button[] stageButtons;

    [Header("Quit Buttons")]
    public Button quitYesButton;
    public Button quitNoButton;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        string scene = SceneManager.GetActiveScene().name;
        isInGame = (scene == "GameScene");

        if (!isInGame) OpenMainMenu();
        else if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    private void Start() {
        if (SceneController.pendingAction.HasValue && saveMenuUI != null) {
            StartCoroutine(DeferredOpen(SceneController.pendingAction.Value));
            SceneController.pendingAction = null;
        }

        if (quitYesButton != null)
            quitYesButton.onClick.AddListener(() => SceneController.QuitGame());

        if (quitNoButton != null)
            quitNoButton.onClick.AddListener(() => CloseQuitConfirmation());
    }

    private void Update() {
        if (inputLocked) return;

        if (saveMenuUI != null && saveMenuUI.gameObject.activeSelf) {
            GameObject namePanel = saveMenuUI.GetEnterNamePanel();
            bool isNameEntryOpen = namePanel != null && namePanel.activeSelf;

            Vector2 nav = VRInputManager.Instance.GetUINavigationDelta();

            if (nav.y > 0.5f && !isNameEntryOpen)
                saveMenuUI.NavigateSlots(-1);
            else if (nav.y < -0.5f && !isNameEntryOpen)
                saveMenuUI.NavigateSlots(1);
            else if (VRInputManager.Instance.GetUIClick()) {
                if (isNameEntryOpen)
                    saveMenuUI.ConfirmPlayerName();
                else
                    saveMenuUI.ConfirmSlotSelection();
            }
            else if (VRInputManager.Instance.GetUIBack())
                saveMenuUI.CloseMenu();

            return;
        }

        Button[] currentButtons = GetCurrentButtons();
        if (currentButtons.Length > 0) {
            Vector2 nav = VRInputManager.Instance.GetUINavigationDelta();

            if (currentPanel == quitConfirmationPanel) {
                if (nav.x > 0.5f) ChangeSelection(1);
                else if (nav.x < -0.5f) ChangeSelection(-1);
            }
            else {
                if (nav.y > 0.5f) ChangeSelection(-1);
                else if (nav.y < -0.5f) ChangeSelection(1);
            }

            if (VRInputManager.Instance.GetUIClick()) {
                if (currentPanel == quitConfirmationPanel && !allowQuitInput) return;
                ActivateCurrentSelection();
            }
        }

        if (VRInputManager.Instance.GetUIBack() || VRInputManager.Instance.GetOpenMenu()) {
            if (statisticsPanel != null && statisticsPanel.activeSelf) {
                statisticsPanel.SetActive(false);
                pauseMenuPanel.SetActive(true);
                currentPanel = pauseMenuPanel;
                selectedIndex = 0;

                if (pauseButtons != null && pauseButtons.Length > 0)
                    pauseButtons[selectedIndex].Select();

                return;
            }

            if (isInGame) TogglePauseMenu();
            else if (currentPanel == startSubMenuPanel || currentPanel == optionsMenuPanel || currentPanel == previousStagesPanel)
                OpenMainMenu();
            else if (currentPanel == quitConfirmationPanel)
                CloseQuitConfirmation();
        }

        if (quitConfirmationPanel != null && quitConfirmationPanel.activeSelf && allowQuitInput) {
            if (Keyboard.current.yKey.wasPressedThisFrame) SceneController.QuitGame();
            if (Keyboard.current.nKey.wasPressedThisFrame) CloseQuitConfirmation();
        }
    }

    private void ChangeSelection(int dir) {
        Button[] current = GetCurrentButtons();
        if (current.Length == 0) return;

        selectedIndex = Mathf.Clamp(selectedIndex + dir, 0, current.Length - 1);
        current[selectedIndex].Select();
    }

    private void ActivateCurrentSelection() {
        Button[] current = GetCurrentButtons();
        if (current.Length == 0) return;

        if (selectedIndex < 0 || selectedIndex >= current.Length) return;

        Button selectedButton = current[selectedIndex];
        if (selectedButton == null) return;

        selectedButton.onClick.Invoke();
    }

    private Button[] GetCurrentButtons() {
        if (mainMenuPanel != null && mainMenuPanel.activeSelf) return mainButtons ?? new Button[0];
        if (startSubMenuPanel != null && startSubMenuPanel.activeSelf) return startSubButtons ?? new Button[0];
        if (pauseMenuPanel != null && pauseMenuPanel.activeSelf) return pauseButtons ?? new Button[0];
        if (previousStagesPanel != null && previousStagesPanel.activeSelf) return stageButtons ?? new Button[0];
        if (quitConfirmationPanel != null && quitConfirmationPanel.activeSelf)
            return new Button[] { quitYesButton, quitNoButton };
        return new Button[0];
    }

    public void OpenMainMenu() {
        isInGame = false;
        StartCoroutine(ActivatePanelWithDelay(mainMenuPanel, mainButtons));
    }

    public void OpenStartSubMenu() {
        StartCoroutine(ActivatePanelWithDelay(startSubMenuPanel, startSubButtons));
    }

    public void OpenOptionsMenu() {
        currentPanel = optionsMenuPanel;
        ShowOnlyPanel(optionsMenuPanel);
        selectedIndex = 0;
    }

    public void OpenPreviousStagesPanel() {
        UpdateStageButtonStates();
        StartCoroutine(ActivatePanelWithDelay(previousStagesPanel, stageButtons));
    }

    public void OpenQuitConfirmation() {
        ShowOnlyPanel(quitConfirmationPanel);
        selectedIndex = 0;
        allowQuitInput = false;
        Invoke(nameof(EnableQuitInput), 0.3f);
    }

    private void EnableQuitInput() {
        allowQuitInput = true;
    }

    public void ConfirmQuitGame() {
        SceneController.QuitGame();
    }

    public void CloseQuitConfirmation() {
        if (isInGame) TogglePauseMenu();
        else OpenMainMenu();
    }

    public void TogglePauseMenu() {
        if (pauseMenuPanel == null) return;

        bool showPause = !pauseMenuPanel.activeSelf;

        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (quitConfirmationPanel != null) quitConfirmationPanel.SetActive(false);
        if (previousStagesPanel != null) previousStagesPanel.SetActive(false);
        if (statisticsPanel != null) statisticsPanel.SetActive(false);

        pauseMenuPanel.SetActive(showPause);
        currentPanel = showPause ? pauseMenuPanel : null;
        Time.timeScale = showPause ? 0f : 1f;

        if (Player.Instance != null) {
            if (showPause) {
                Player.Instance.DisableMovement();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else {
                Player.Instance.EnableMovement();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (showPause && pauseButtons != null && pauseButtons.Length > 0) {
            selectedIndex = 0;
            pauseButtons[selectedIndex].Select();
        }
    }

    public void LoadGameSceneFromButton() {
        isInGame = true;
        SceneController.LoadGameScene();
    }

    public void LoadMainMenuFromButton() {
        SceneController.LoadMainMenu();
    }

    public void OpenSaveMenu() {
        saveMenuUI?.Open(SaveActionType.Save);
    }

    public void OpenLoadMenu() {
        if (isInGame)
            SceneController.LoadMainMenuForSave(SaveActionType.Load);
        else
            saveMenuUI?.Open(SaveActionType.Load);
    }

    public void OpenDeleteMenu() {
        if (isInGame)
            SceneController.LoadMainMenuForSave(SaveActionType.Delete);
        else
            saveMenuUI?.Open(SaveActionType.Delete);
    }

    public void OpenStatisticsPanel() {
        ShowOnlyPanel(statisticsPanel);
        StatisticsManager.Instance.DisplayStatisticsInUI();
    }

    public void ShowOnlyPanel(GameObject activePanel) {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (startSubMenuPanel != null) startSubMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (quitConfirmationPanel != null) quitConfirmationPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (previousStagesPanel != null) previousStagesPanel.SetActive(false);
        if (statisticsPanel != null) statisticsPanel.SetActive(false);

        if (activePanel != null) activePanel.SetActive(true);
        currentPanel = activePanel;
    }

    private IEnumerator ActivatePanelWithDelay(GameObject panel, Button[] buttons) {
        inputLocked = true;
        currentPanel = panel;
        ShowOnlyPanel(panel);
        yield return null;
        selectedIndex = 0;
        if (buttons != null && buttons.Length > 0) {
            buttons[selectedIndex].Select();
        }
        inputLocked = false;
    }

    private IEnumerator DeferredOpen(SaveActionType action) {
        yield return new WaitForEndOfFrame();
        saveMenuUI?.Open(action);
    }

    public void OnStageReplayButton(int stageNumber) {
        GameData.ShowIntro = false;

        if (GameData.MaxStageReached >= stageNumber) {
            GameData.ReplayStage = stageNumber;

            if (StatisticsManager.Instance != null) {
                GameData.CachedStatistics = StatisticsManager.Instance.GetAllStats();
            }

            StatisticsManager.Instance.BeginSession(stageNumber);
            SceneController.LoadGameScene();
        }
    }

    private void UpdateStageButtonStates() {
        if (stageButtons == null || stageButtons.Length == 0) return;

        for (int i = 0; i < stageButtons.Length; i++) {
            int stageNumber = i + 1;
            bool isAvailable = GameData.MaxStageReached >= stageNumber;

            stageButtons[i].interactable = isAvailable;

            ColorBlock colors = stageButtons[i].colors;
            if (!isAvailable) {
                colors.normalColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            }

            stageButtons[i].colors = colors;
        }
    }
}


