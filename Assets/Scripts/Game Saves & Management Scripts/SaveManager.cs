using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    private static string folderPath => Path.Combine(Application.persistentDataPath, "Saves");
    private List<string> currentCleanedIDs = new List<string>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
    }

    private string GetSlotPath(int index) {
        return Path.Combine(folderPath, $"save_slot_{index}.json");
    }

    public void SaveToSlot(int index) {
        StartCoroutine(DelayedSave(index));
    }

    private IEnumerator DelayedSave(int index) {
        yield return new WaitForSeconds(1f);

        FullSaveData full = new FullSaveData();
        full.playerData = SaveData.FromGameData();

        full.stageNumber = StageManager.Instance != null ? StageManager.Instance.GetCurrentStage() : 1;
        full.cleanerPlatformY = CleanerPlatform.Instance != null ? CleanerPlatform.Instance.transform.position.y : 0f;

        full.cleanedTargetIDs = new List<string>(currentCleanedIDs); // Use collected IDs

        var allTargets = Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);
        foreach (var target in allTargets) {
            if (target == null) continue;
            string id = target.GetID();
            if (!string.IsNullOrEmpty(id) && target.GetProgress() >= 0.99f) {
                if (!full.cleanedTargetIDs.Contains(id))
                    full.cleanedTargetIDs.Add(id);
            }
        }

        // Save stage stats
        full.stageStatistics = new List<StageStatsEntry>();
        foreach (var kvp in StatisticsManager.Instance.GetAllStats()) {
            full.stageStatistics.Add(new StageStatsEntry {
                stageNumber = kvp.Key,
                stats = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(full, true);
        File.WriteAllText(GetSlotPath(index), json);
        Debug.Log($"[SaveManager] Saved to slot {index}");
    }

    public bool HasCleanedID(string id) {
        return currentCleanedIDs.Contains(id);
    }

    public void LoadFromSlot(int index) {
        string path = GetSlotPath(index);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        FullSaveData full = JsonUtility.FromJson<FullSaveData>(json);
        if (full == null) return;

        GameData.LoadFromFullSave(full);

        Dictionary<int, StageStats> statsDict = new Dictionary<int, StageStats>();
        foreach (var entry in full.stageStatistics) {
            statsDict[entry.stageNumber] = entry.stats;
        }

        StatisticsManager.Instance.LoadStatsFromExternal(statsDict);
        GameRestorer.Instance?.ApplyLoadedData(full);

        GameData.ShowIntro = false;

        Debug.Log($"[SaveManager] Loaded slot {index}");
    }

    public SaveData LoadSlotHeader(int index) {
        string path = GetSlotPath(index);
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        FullSaveData full = JsonUtility.FromJson<FullSaveData>(json);
        return full?.playerData;
    }

    public void DeleteSlot(int index) {
        string path = GetSlotPath(index);
        if (File.Exists(path)) File.Delete(path);
    }

    public bool SlotExists(int index) {
        return File.Exists(GetSlotPath(index));
    }

    public void ForceAddCleanedID(string id) {
        if (string.IsNullOrEmpty(id)) return;
        if (!currentCleanedIDs.Contains(id))
            currentCleanedIDs.Add(id);
    }

    public List<FullSaveData> GetAllSavedData() {
        List<FullSaveData> allData = new List<FullSaveData>();

        if (!Directory.Exists(folderPath))
            return allData;

        string[] files = Directory.GetFiles(folderPath, "*.json");
        foreach (var file in files) {
            try {
                string json = File.ReadAllText(file);
                FullSaveData data = JsonUtility.FromJson<FullSaveData>(json);
                if (data != null) {
                    allData.Add(data);
                }
            }
            catch (System.Exception ex) {
                Debug.LogWarning($"[SaveManager] Failed to load save from {file}: {ex.Message}");
            }
        }

        return allData;
    }

    public void ResetCleanedIDs() {
        currentCleanedIDs.Clear();
    }

}

