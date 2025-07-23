using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        if (GameData.ReplayStage > 0) {
            Debug.Log($"[GameManager] Replaying stage {GameData.ReplayStage}...");
            StageManager.Instance?.SetStage(GameData.ReplayStage);
            GameData.Level = GameData.ReplayStage;
            GameData.ReplayStage = -1;
        }

        if (LoadBuffer.HasPendingLoad) {
            StartCoroutine(WaitThenLoadSlot());
        }
        else if (GameData.LoadedFromSave) {
            Debug.Log("[GameManager] Waiting for Player to be ready...");
            PlayerSpawnNotifier.OnPlayerReady += OnPlayerReady;
        }
    }

    private IEnumerator WaitThenLoadSlot() {
        yield return new WaitUntil(() =>
            Player.Instance != null &&
            StageManager.Instance != null &&
            GameRestorer.Instance != null
        );

        int slot = LoadBuffer.pendingSlotIndex.Value;
        Debug.Log($"[GameManager] Detected pending load for slot {slot}. Loading...");
        SaveManager.Instance?.LoadFromSlot(slot);
        LoadBuffer.Clear();
    }

    private void OnPlayerReady() {
        PlayerSpawnNotifier.OnPlayerReady -= OnPlayerReady;
        StartCoroutine(DelayedRestore());
    }

    private IEnumerator DelayedRestore() {
        yield return new WaitUntil(() => Player.Instance != null);

        Debug.Log("[GameManager] Restoring Player from GameData...");
        Player.Instance.ApplyGameData();
        // No longer resetting LoadedFromSave here!
        Debug.Log("[GameManager] Player restored.");
    }
}

