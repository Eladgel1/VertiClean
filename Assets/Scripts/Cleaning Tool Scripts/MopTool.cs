using UnityEngine;

public class MopTool : SpongeTool {
    private void OnEnable() {
        int stage = StageManager.Instance != null ? StageManager.Instance.GetCurrentStage() : 0;
        bool shouldBeActive = stage >= 3 && stage <= 4;

        if (!shouldBeActive) {
            gameObject.SetActive(false);
        }
    }
}
