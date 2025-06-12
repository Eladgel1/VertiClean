using UnityEngine;
using System.Collections.Generic;

public static class GameData {
    public static string PlayerName = "Player";
    public static int Level = 1;
    public static int MaxStageReached = 1;
    public static Vector3 Position = Vector3.zero;
    public static bool LoadedFromSave = false;

    public static int ReplayStage = -1;
    public static bool StartedFromReplay = false;
    public static int ResumeStageAfterReplay = -1;
    public static bool ShowIntro = true;

    public static void LoadFromFullSave(FullSaveData full) {
        if (full == null || full.playerData == null) return;

        PlayerName = full.playerData.playerName;
        Position = full.playerData.position;
        Level = full.stageNumber;
        MaxStageReached = full.playerData.maxStageReached;

        LoadedFromSave = true;
        StartedFromReplay = false;
        ResumeStageAfterReplay = -1;
    }

    public static void SetFromFullSave(FullSaveData full) {
        if (full == null || full.playerData == null) return;

        PlayerName = full.playerData.playerName;
        Position = full.playerData.position;
        Level = full.stageNumber;
        MaxStageReached = full.playerData.maxStageReached;
    }

    public static SaveData CreateSaveData() => SaveData.FromGameData();
}

