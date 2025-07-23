using UnityEngine;
using System.Collections;

public class GameRestorer : MonoBehaviour {
    public static GameRestorer Instance { get; private set; }

    private FullSaveData loadedData;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyLoadedData(FullSaveData data) {
        loadedData = data;
        StartCoroutine(WaitAndApply());
    }

    private IEnumerator WaitAndApply() {
        yield return new WaitUntil(() =>
            Player.Instance != null &&
            CleanerPlatform.Instance != null &&
            StageManager.Instance != null
        );

        Debug.Log("[GameRestorer] Applying loaded game state...");

        GameData.SetFromFullSave(loadedData);
        ApplyPlayerPosition();
        StageManager.Instance.SetStage(loadedData.stageNumber);
        RestorePlatformPosition();
        RemoveCleanedTargets();

        GameData.LoadedFromSave = false;
        GameData.ShowIntro = false;

        Debug.Log("[GameRestorer] All game state applied successfully.");
    }

    private void ApplyPlayerPosition() {
        if (Player.Instance != null) {
            Player.Instance.transform.position = GameData.Position;
            Debug.Log($"[GameRestorer] Player position restored to {GameData.Position}");
        }
    }

    private void RestorePlatformPosition() {
        if (CleanerPlatform.Instance != null) {
            Vector3 pos = CleanerPlatform.Instance.transform.position;
            pos.y = loadedData.cleanerPlatformY;
            CleanerPlatform.Instance.transform.position = pos;
            Debug.Log($"[GameRestorer] Platform Y restored to {pos.y}");
        }
    }

    private void RemoveCleanedTargets() {
        CleaningTarget[] allTargets = Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);
        int removed = 0;

        foreach (CleaningTarget target in allTargets) {
            if (target == null) continue;

            if (loadedData.cleanedTargetIDs.Contains(target.GetID())) {
                StageManager.Instance?.OnTargetCleaned(target); // Update stage progression
                SaveManager.Instance?.ForceAddCleanedID(target.GetID()); // Retain for next Save
                Destroy(target.gameObject);
                removed++;
            }
        }

        Debug.Log($"[GameRestorer] Removed {removed} cleaned targets.");
    }
}
