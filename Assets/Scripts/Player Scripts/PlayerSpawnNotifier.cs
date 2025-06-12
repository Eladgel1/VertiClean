using UnityEngine;
using System;

public class PlayerSpawnNotifier : MonoBehaviour {
    public static event Action OnPlayerReady;

    private void Start() {
        if (Player.Instance != null) {
            Debug.Log("[PlayerSpawnNotifier] Player is ready!");
            OnPlayerReady?.Invoke();
        }
        else {
            Debug.LogWarning("[PlayerSpawnNotifier] Player.Instance == null on Start");
        }
    }
}

