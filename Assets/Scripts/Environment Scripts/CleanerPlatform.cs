using UnityEngine;

public class CleanerPlatform : MonoBehaviour {
    public static CleanerPlatform Instance { get; private set; }

    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float groundLevel = 8.8f;
    [SerializeField] private float maxHeight = 17.3f;

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update() {
        float vertical = 0f;
        bool liftingUp = VRInputManager.Instance.GetLiftUp() && transform.position.y < maxHeight;
        bool liftingDown = VRInputManager.Instance.GetLiftDown() && transform.position.y > groundLevel;

        if (liftingUp) vertical = 1f;
        if (liftingDown) vertical = -1f;

        Vector3 move = new Vector3(0f, vertical * moveSpeed * Time.deltaTime, 0f);
        transform.Translate(move);
    }
}
