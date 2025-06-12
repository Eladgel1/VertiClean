using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour {
    public int slotIndex;
    public TextMeshProUGUI slotText;
    public Button button;

    public void Initialize(int index, SaveData data, System.Action<int> onClick) {
        slotIndex = index;

        if (data == null)
            slotText.text = $"Empty Slot No. {index + 1}";
        else
            slotText.text = $"{data.playerName} - {data.saveTime}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(slotIndex));
    }
}
