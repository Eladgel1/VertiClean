using UnityEngine;

public class VehicleMovement : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Movement speed
    [SerializeField] private bool isReversed = false; // Should the car move in the reverse direction?

    [Header("Teleport Settings")]
    [SerializeField] private float teleportZThreshold = 280f; // Z position to teleport at
    [SerializeField] private float teleportZReset = -15f; // Z position to reset to after teleport

    private void Update() {
        MoveForward();

        CheckTeleport();
    }

    private void MoveForward() {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void CheckTeleport() {
        float currentZ = transform.position.z;

        if (!isReversed) {
            if (currentZ >= teleportZThreshold) {
                Vector3 resetPosition = new Vector3(transform.position.x, transform.position.y, teleportZReset);
                transform.position = resetPosition;
            }
        }
        else {
            if (currentZ <= teleportZReset) {
                Vector3 resetPosition = new Vector3(transform.position.x, transform.position.y, teleportZThreshold);
                transform.position = resetPosition;
            }
        }
    }
}
