using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System;

public class Stage3PerformanceLogger : MonoBehaviour {
    private float minFps = float.MaxValue;
    private float maxFps = float.MinValue;
    private float maxMemory = 0f;
    private long initialGC = 0;
    private float graphicsMemoryMB = 0f;
    private Stopwatch stopwatch;

    private bool hasStarted = false;

    void Update() {
        if (!hasStarted && GameData.Level == 3) {
            UnityEngine.Debug.Log("[PERF] GameData.Level == 3 detected in Update() — starting logger");
            hasStarted = true;

            stopwatch = Stopwatch.StartNew();
            initialGC = GC.GetTotalMemory(false);

            StartCoroutine(LogPerformanceData());
        }
    }

    IEnumerator LogPerformanceData() {
        while (GameData.Level == 3) {
            float fps = 1.0f / Time.deltaTime;

            if (fps < minFps) minFps = fps;
            if (fps > maxFps) maxFps = fps;

            float currentMemoryMB = GC.GetTotalMemory(false) / (1024f * 1024f);
            if (currentMemoryMB > maxMemory) maxMemory = currentMemoryMB;

            graphicsMemoryMB = UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver() / (1024f * 1024f);

            yield return new WaitForSeconds(1f);
        }

        stopwatch.Stop();
        long finalGC = GC.GetTotalMemory(false);
        float totalSeconds = stopwatch.ElapsedMilliseconds / 1000f;
        float gcDeltaMB = (finalGC - initialGC) / (1024f * 1024f);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string report =
            "=== Stage 3 Performance Report ===\n" +
            $"Date & Time: {timestamp}\n" +
            $"Total Duration: {totalSeconds:F1} seconds\n" +
            $"Min FPS: {minFps:F2}\n" +
            $"Max FPS: {maxFps:F2}\n" +
            $"Max Memory Usage: {maxMemory:F1} MB\n" +
            $"GC Delta: {gcDeltaMB:F1} MB\n" +
            $"Graphics Driver Memory: {graphicsMemoryMB:F1} MB\n" +
            "=================================\n";

        string filePath = Path.Combine(Application.persistentDataPath, "Stage3_PerfReport.txt");

        try {
            File.WriteAllText(filePath, report);
            UnityEngine.Debug.Log($"[PERF] Report successfully written to:\n{filePath}\n{report}");
        }
        catch (Exception ex) {
            UnityEngine.Debug.LogError($"[PERF] Failed to write report: {ex.Message}");
        }
    }
}
