using System.Collections.Generic;

[System.Serializable]
public class FullSaveData {
    public SaveData playerData = new SaveData();
    public float cleanerPlatformY = 0f;
    public List<string> cleanedTargetIDs = new List<string>();
    public int stageNumber = 1;
}