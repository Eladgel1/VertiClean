using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController {
    private const string gameSceneString = "GameScene";
    private const string mainMenuSceneString = "MainMenuScene";
    public static SaveActionType? pendingAction = null;

    public static void LoadGameScene() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneString);
    }

    public static void LoadMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneString);
    }

    public static void LoadMainMenuForSave(SaveActionType action) {
        if (action == SaveActionType.Save) {
            if (Player.Instance != null) {
                Player.Instance.ExportToGameData();
                Debug.Log($"[SceneController] Exported player data before saving: {GameData.Position}");
            }
            else {
                Debug.LogWarning("[SceneController] Player.Instance is null. Cannot export player data.");
            }
        }

        pendingAction = action;
        LoadMainMenu();
    }

    public static void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}