using UnityEngine;

[System.Serializable]
public class SaveData {
    public string playerName;
    public string saveTime;
    public int level;
    public int maxStageReached;
    public float posX, posY, posZ;

    public Vector3 position {
        get => new Vector3(posX, posY, posZ);
        set {
            posX = value.x;
            posY = value.y;
            posZ = value.z;
        }
    }

    public static SaveData FromGameData() {
        return new SaveData {
            playerName = GameData.PlayerName,
            saveTime = System.DateTime.Now.ToString("dd/MM/yy, HH:mm"),
            level = GameData.Level,
            maxStageReached = GameData.MaxStageReached,
            position = GameData.Position
        };
    }
}

