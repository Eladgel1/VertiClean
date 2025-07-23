using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatisticsManager : MonoBehaviour {
    public static StatisticsManager Instance { get; private set; }

    private Dictionary<int, StageStats> statsPerStage = new Dictionary<int, StageStats>();
    private float currentSessionStartTime = 0f;
    private int currentStage = -1;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI[] totalTimeTexts; // index 0 = stage 1
    [SerializeField] private TextMeshProUGUI[] minTimeTexts;
    [SerializeField] private TextMeshProUGUI[] maxTimeTexts;
    [SerializeField] private TextMeshProUGUI[] ratingTexts;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        // Restore cached statistics from GameData if this is a replay
        if (GameData.CachedStatistics != null) {
            LoadStatsFromExternal(GameData.CachedStatistics);
            GameData.CachedStatistics = null; // Clear cache after use
        }
    }

    public void BeginSession(int stage) {
        currentStage = stage;
        currentSessionStartTime = Time.time;

        if (!statsPerStage.ContainsKey(stage)) {
            statsPerStage[stage] = new StageStats();
        }
    }

    public void EndSession(string rating) {
        if (currentStage == -1) return;

        float duration = Time.time - currentSessionStartTime;
        statsPerStage[currentStage].UpdateSession(duration, rating);
        currentStage = -1;
    }

    public void DisplayStatisticsInUI() {
        for (int i = 1; i <= 4; i++) {
            StageStats stats = GetStatsForStage(i);
            int index = i - 1;

            if (stats != null) {
                totalTimeTexts[index].text = stats.GetTotalTimeFormatted();
                minTimeTexts[index].text = stats.minSessionSeconds != float.MaxValue ? stats.GetMinTimeFormatted() : "-";
                maxTimeTexts[index].text = stats.maxSessionSeconds > 0f ? stats.GetMaxTimeFormatted() : "-";
                string translated = TranslateRating(stats.lastRating);
                ratingTexts[index].text = !string.IsNullOrEmpty(translated) ? translated : "-";
            }
            else {
                totalTimeTexts[index].text = "00:00:00";
                minTimeTexts[index].text = "-";
                maxTimeTexts[index].text = "-";
                ratingTexts[index].text = "-";
            }
        }
    }

    private string TranslateRating(string rating) {
        switch (rating) {
            case "1": return "Very Low";
            case "2": return "Low";
            case "3": return "Medium";
            case "4": return "High";
            case "5": return "Very High";
            default: return rating;
        }
    }

    public Dictionary<int, StageStats> GetAllStats() => statsPerStage;

    public void LoadStatsFromExternal(Dictionary<int, StageStats> data) {
        statsPerStage = data ?? new Dictionary<int, StageStats>();
    }

    public void ResetStatistics() {
        statsPerStage.Clear();
    }

    public StageStats GetStatsForStage(int stage) {
        return statsPerStage.ContainsKey(stage) ? statsPerStage[stage] : null;
    }
}


