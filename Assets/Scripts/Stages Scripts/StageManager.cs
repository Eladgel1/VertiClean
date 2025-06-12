using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {
    public static StageManager Instance { get; private set; }

    [SerializeField] private int currentStage = 1;

    private List<CleaningTarget> cleaningTargets = new List<CleaningTarget>();
    private int cleanedTargets = 0;

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

        UpdateToolVisibility();

        Debug.Log($"Stage {newStage} initialized with {cleaningTargets.Count} targets.");

        string toolInfo = "";

        if (currentStage == 3) {
            toolInfo = "The cleaning tool now available to you is a mop instead of a sponge.\n\n";
        }

        string message =
            $"Welcome to stage {currentStage},\n" +
            "Clean all stains using the right tools.\n\n" +
            toolInfo +
            "Press Enter to continue.";

        IntroManager.Instance.ShowNewStageMessage(message);
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

    public void ReadyToDescend() {
        StartCoroutine(WaitForGroundLevel());
    }

    private IEnumerator WaitForGroundLevel() {
        while (CleanerPlatform.Instance.transform.position.y > 8.81f) {
            yield return null;
        }

        Player.Instance.DisableMovement();

        currentStage++;
        if (currentStage > 4) {
            ShowEndGameMessage();
            yield break;
        }

        SetStage(currentStage);
    }

    private void ShowEndGameMessage() {
        Player.Instance.DisableMovement();

        string finalMessage =
            "Congratulations! You have completed all the stages in VertiClean.\n" +
            "You faced your fear of heights and mastered every cleaning challenge.\n" +
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
        }
    }
}
