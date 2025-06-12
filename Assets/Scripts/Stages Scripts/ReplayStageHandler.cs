using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles stage replay logic for previously completed stages.
/// </summary>
public class ReplayStageHandler : MonoBehaviour {

    private void Awake() {
        if (GameData.ReplayStage > 0) {
            GameData.StartedFromReplay = true;
        }
    }

    private void Start() {
        int stage = GameData.ReplayStage;
        if (stage < 1 || stage > 4) return;

        GameData.Level = stage;

        GameData.ShowIntro = false;

        if (stage > GameData.MaxStageReached) {
            GameData.MaxStageReached = stage;
        }

        GameData.ResumeStageAfterReplay = stage + 1;

        // Step 1: Inject cleaned IDs BEFORE scene objects are destroyed
        InjectCleanedIDsUpToStage(stage);

        // Step 2: Set stage (which includes object destruction)
        StageManager.Instance?.SetStage(stage);

        IntroManager.Instance?.EndIntro();

        if (stage > 1) {
            string toolInfo = "";
            string weatherInfo = "";

            if (stage == 3) {
                toolInfo = "The cleaning tool now available to you is a mop instead of a sponge.\n\n";
                weatherInfo = "Please note: The weather is changing to cloudy, you may feel light winds blowing.\n\n";
            }
            else if (stage == 4) {
                weatherInfo = "Please note: During the stage you are going to experience weather accompanied by heavy cloud cover and stronger winds.\n\n";
            }

            string message =
                $"Welcome to stage {stage},\n" +
                "Clean all stains using the right tools.\n\n" +
                toolInfo +
                weatherInfo +
                "Press Enter to continue.";

            IntroManager.Instance?.ShowNewStageMessage(message);
        }

        GameData.ReplayStage = -1;
    }

    private void InjectCleanedIDsUpToStage(int replayStage) {
        if (SaveManager.Instance == null || StageManager.Instance == null) return;

        List<string> idsToAdd = new List<string>();

        if (replayStage == 1) {
            SaveManager.Instance.ResetCleanedIDs(); // stage 1 --> empty
            Debug.Log("[ReplayStageHandler] Cleaned IDs reset for stage 1.");
            return;
        }

        for (int stage = 1; stage < replayStage; stage++) {
            if (StageManager.Instance.TryGetCleanedIDsForReplayStage(stage + 1, out var ids)) {
                foreach (var id in ids) {
                    if (!idsToAdd.Contains(id))
                        idsToAdd.Add(id);
                }
            }
        }

        foreach (string id in idsToAdd) {
            SaveManager.Instance.ForceAddCleanedID(id);
        }

        Debug.Log($"[ReplayStageHandler] Injected {idsToAdd.Count} cleaned IDs before stage {replayStage}.");
    }
}
