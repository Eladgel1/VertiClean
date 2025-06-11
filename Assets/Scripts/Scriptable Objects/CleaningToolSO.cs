using UnityEngine;

[CreateAssetMenu(fileName = "New Cleaning Tool", menuName = "CleaningTool")]
public class CleaningToolSO : ScriptableObject {
    public string toolName;
    public Sprite icon;
    public ToolType toolType;
    public float cleaningPower;
    public int minStageAllowed;
    public int maxStageAllowed;
}

public enum ToolType {
    Spray,
    Sponge,
    Mop
}
