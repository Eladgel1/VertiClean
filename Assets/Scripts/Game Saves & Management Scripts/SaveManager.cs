/*using UnityEngine;
using System.IO;
using System.Collections;

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    private static string folderPath => Path.Combine(Application.persistentDataPath, "Saves");

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
        yield return new WaitForSeconds(1f); // Let the scene fully initialize

        FullSaveData full = new FullSaveData();
        full.playerData = SaveData.FromGameData();

        // Save stage number from StageManager, not from GameData
        if (StageManager.Instance != null) {
            full.stageNumber = StageManager.Instance.GetCurrentStage();
            Debug.Log($"[SaveManager] Stage saved = {full.stageNumber}");
        }
        else {
            Debug.LogWarning("[SaveManager] StageManager not found. Saving stage = 1.");
            full.stageNumber = 1;
        }

        // Save platform Y position
        if (CleanerPlatform.Instance != null) {
            full.cleanerPlatformY = CleanerPlatform.Instance.transform.position.y;
            Debug.Log($"[SaveManager] Platform Y = {full.cleanerPlatformY}");
        }
        else {
            Debug.LogWarning("[SaveManager] CleanerPlatform not found. Saving Y = 0.");
            full.cleanerPlatformY = 0f;
        }

        // Save cleaned targets
        var allTargets = Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var target in allTargets) {
            if (target == null) continue;

            string id = target.GetID();
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError($"[SaveManager] CleaningTarget '{target.name}' has no ID!");
                continue;
            }

            if (target.GetProgress() >= 1f) {
                full.cleanedTargetIDs.Add(id);
                count++;
            }
        }

        Debug.Log($"[SaveManager] Total cleaned targets saved: {count}");

        string json = JsonUtility.ToJson(full, true);
        File.WriteAllText(GetSlotPath(index), json);

        Debug.Log($"[SaveManager] Slot {index} saved to path: {GetSlotPath(index)}");
    }

    public void LoadFromSlot(int index) {
        string path = GetSlotPath(index);
        if (!File.Exists(path)) {
            Debug.LogWarning($"[SaveManager] File not found at slot {index}");
            return;
        }

        string json = File.ReadAllText(path);
        FullSaveData full = JsonUtility.FromJson<FullSaveData>(json);
        if (full == null) {
            Debug.LogError($"[SaveManager] Failed to parse FullSaveData at slot {index}");
            return;
        }

        Debug.Log($"[SaveManager] Loaded slot {index}: {full.playerData.playerName}, Pos: {full.playerData.position}, Level: {full.playerData.level}");

        GameData.LoadFromFullSave(full);
        GameRestorer.Instance?.ApplyLoadedData(full);
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
}*/


/*using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    private static string folderPath => Path.Combine(Application.persistentDataPath, "Saves");

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

        var allTargets = Object.FindObjectsByType<CleaningTarget>(FindObjectsSortMode.None);
        foreach (var target in allTargets) {
            if (target == null) continue;
            string id = target.GetID();
            if (!string.IsNullOrEmpty(id) && target.GetProgress() >= 1f) {
                full.cleanedTargetIDs.Add(id);
            }
        }

        // Convert dictionary to list
        full.stageStatistics = new List<StageStatsEntry>();
        foreach (var kvp in StatisticsManager.Instance.GetAllStats()) {
            full.stageStatistics.Add(new StageStatsEntry {
                stageNumber = kvp.Key,
                stats = kvp.Value
            });
        }

        string json = JsonUtility.ToJson(full, true);
        File.WriteAllText(GetSlotPath(index), json);
    }

    public void LoadFromSlot(int index) {
        string path = GetSlotPath(index);
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        FullSaveData full = JsonUtility.FromJson<FullSaveData>(json);
        if (full == null) return;

        GameData.LoadFromFullSave(full);

        // Convert list back to dictionary
        Dictionary<int, StageStats> statsDict = new Dictionary<int, StageStats>();
        foreach (var entry in full.stageStatistics) {
            statsDict[entry.stageNumber] = entry.stats;
        }

        StatisticsManager.Instance.LoadStatsFromExternal(statsDict);
        GameRestorer.Instance?.ApplyLoadedData(full);
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
}*/

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

