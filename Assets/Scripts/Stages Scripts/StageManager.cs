using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    [SerializeField] private int currentStage = 1;

    [Header("Skybox Materials")]
    [SerializeField] private Material initialSkybox;
    [SerializeField] private Material stage3Skybox;
    [SerializeField] private Material stage4Skybox;

    private List<CleaningTarget> cleaningTargets = new List<CleaningTarget>();
    private int cleanedTargets = 0;

    private Dictionary<int, List<string>> stainsToRemoveByReplayStage = new Dictionary<int, List<string>>() {
        { 2, new List<string> { "Stain_1", "Stain_2" } },
        { 3, new List<string> { "Stain_1", "Stain_2", "Stain_3", "Stain_4", "Stain_5" } },
        { 4, new List<string> { "Stain_1", "Stain_2", "Stain_3", "Stain_4", "Stain_5", "Stain_6", "Stain_7", "Stain_8" } }
    };

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void OnTargetCleaned(CleaningTarget target) {
        if (target.GetStageNumber() != currentStage) return;

        cleanedTargets++;
        Debug.Log($"Cleaned {cleanedTargets} of {cleaningTargets.Count}");

        if (cleanedTargets >= cleaningTargets.Count) {
            ShowStageCompleteMessage();
        }
    }

    private void ShowStageCompleteMessage() {
        Debug.Log("Triggering StageFeedbackUI");
        StageFeedbackUI.Instance.ShowStageCompleteMessage(currentStage);
    }

    public int GetCurrentStage() => currentStage;

    public void SetStage(int newStage) {
        currentStage = newStage;
        cleanedTargets = 0;
        cleaningTargets.Clear();

        CleanerPlatform.Instance.UpdateMaxHeight();

        foreach (var target in Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None)) {
            RegisterCleaningTarget(target);
        }

        RemoveUnwantedStainsIfReplaying();
        UpdateToolVisibility();
        UpdateEnvironmentVisuals();

        GameData.Level = currentStage;

        if (currentStage > GameData.MaxStageReached) {
            GameData.MaxStageReached = currentStage;
        }

        Debug.Log($"Stage {newStage} initialized with {cleaningTargets.Count} targets.");
    }

    private void RemoveUnwantedStainsIfReplaying() {
        int replayStage = GameData.ReplayStage;
        if (replayStage < 1 || !stainsToRemoveByReplayStage.ContainsKey(replayStage)) return;

        List<string> idsToRemove = stainsToRemoveByReplayStage[replayStage];
        foreach (var target in Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None)) {
            if (idsToRemove.Contains(target.GetID())) {
                Destroy(target.gameObject);
            }
        }
    }

    private void UpdateToolVisibility() {
        CleaningToolBase[] allTools = Resources.FindObjectsOfTypeAll<CleaningToolBase>();

        foreach (var tool in allTools) {
            int minStage = tool.GetToolData().minStageAllowed;
            int maxStage = tool.GetToolData().maxStageAllowed;

            bool shouldBeActive = currentStage >= minStage && currentStage <= maxStage;
            tool.gameObject.SetActive(shouldBeActive);
        }
    }

    private void UpdateEnvironmentVisuals() {
        if (currentStage == 3) {
            RenderSettings.skybox = stage3Skybox;
            RenderSettings.fogColor = new Color32(152, 162, 168, 255);
        }
        else if (currentStage == 4) {
            RenderSettings.skybox = stage4Skybox;
            RenderSettings.fogColor = new Color32(142, 142, 142, 255);
        }
    }

    public void ReadyToDescend() {
        StartCoroutine(WaitForGroundLevel());
    }

    private IEnumerator WaitForGroundLevel() {
        while (CleanerPlatform.Instance.transform.position.y > 8.81f) {
            yield return null;
        }

        Player.Instance.DisableMovement();

        // Handle replay resume stage
        if (GameData.StartedFromReplay && GameData.ResumeStageAfterReplay > 0) {
            currentStage = GameData.ResumeStageAfterReplay;
            GameData.Level = currentStage;
            GameData.MaxStageReached = Mathf.Max(GameData.MaxStageReached, currentStage);
            GameData.StartedFromReplay = false;
            GameData.ResumeStageAfterReplay = -1;
        }
        else {
            currentStage++;
            GameData.Level = currentStage;
            GameData.MaxStageReached = Mathf.Max(GameData.MaxStageReached, currentStage);
        }

        if (currentStage > 4) {
            ShowEndGameMessage();
            yield break;
        }

        SetStage(currentStage);

        string toolInfo = "";
        string weatherInfo = "";

        if (currentStage == 3) {
            toolInfo = "The cleaning tool now available to you is a mop instead of a sponge.\n\n";
            weatherInfo = "Please note: The weather is changing to cloudy, you may feel light winds blowing.\n\n";
        }
        else if (currentStage == 4) {
            weatherInfo = "Please note: During the stage you are going to experience weather accompanied by heavy cloud cover and stronger winds.\n\n";
        }

        string message =
            $"Welcome to stage {currentStage},\n" +
            "Clean all stains using the right tools.\n\n" +
            toolInfo +
            weatherInfo +
            "Press Enter to continue.";

        IntroManager.Instance.ShowNewStageMessage(message);
    }


    private void ShowEndGameMessage() {
        Player.Instance.DisableMovement();

        RenderSettings.skybox = initialSkybox;
        RenderSettings.fogColor = new Color32(124, 177, 207, 255);

        string finalMessage =
            "Congratulations! You have completed all the stages in VertiClean.\n" +
            "You faced your fear of heights and overcame challenging weather conditions.\n" +
            "There's no doubt — you're a true cleaning professional.\n" +
            "Thank you for playing.\n\n" +
            "Press Enter to continue.";

        IntroManager.Instance.ShowNewStageMessage(finalMessage, () => {
            Player.Instance.EnableMovement();
        });
    }

    public void RegisterCleaningTarget(CleaningTarget target) {
        if (!cleaningTargets.Contains(target) && target.GetStageNumber() == currentStage) {
            cleaningTargets.Add(target);

            // Detect pre-cleaned stains (restored via GameRestorer)
            if (SaveManager.Instance != null && SaveManager.Instance.HasCleanedID(target.GetID())) {
                cleanedTargets++;
            }
        }
    }

    public bool TryGetCleanedIDsForReplayStage(int stage, out List<string> ids) {
        return stainsToRemoveByReplayStage.TryGetValue(stage, out ids);
    }

}
