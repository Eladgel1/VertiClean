using System;

[Serializable]
public class StageStats {
    public float totalTimeSeconds = 0f;
    public float minSessionSeconds = -1f;
    public float maxSessionSeconds = 0f;
    public string lastRating = "None";

    public void UpdateSession(float sessionTimeInSeconds, string rating) {
        totalTimeSeconds += sessionTimeInSeconds;

        if (minSessionSeconds < 0f || sessionTimeInSeconds < minSessionSeconds)
            minSessionSeconds = sessionTimeInSeconds;

        if (sessionTimeInSeconds > maxSessionSeconds)
            maxSessionSeconds = sessionTimeInSeconds;

        lastRating = rating;
    }

    public string GetTotalTimeFormatted() => FormatTime(totalTimeSeconds);

    public string GetMinTimeFormatted() =>
        minSessionSeconds >= 0f ? FormatTime(minSessionSeconds) : "-";

    public string GetMaxTimeFormatted() =>
        maxSessionSeconds > 0f ? FormatTime(maxSessionSeconds) : "-";

    private string FormatTime(float seconds) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}

