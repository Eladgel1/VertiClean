using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController {
    private const string gameSceneString = "GameScene";
    private const string mainMenuSceneString = "MainMenuScene";

    public static void LoadGameScene() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneString);
    }

    public static void LoadMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneString);
    }

    public static void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
