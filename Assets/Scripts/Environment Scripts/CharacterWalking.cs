using UnityEngine;

public class CharacterWalking : MonoBehaviour {
    [Header("Waypoints")]
    [SerializeField] private Transform pointA; // Start point
    [SerializeField] private Transform pointB; // End point

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.5f; // Walking speed
    [SerializeField] private float thresholdDistance = 0.5f; // Distance to switch direction

    private Transform target; // Current target to walk to

    private void Start() {
        target = pointB; // Set initial target
        FaceTarget();
    }

    private void Update() {
        // Move towards the current target
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Check if close enough to switch direction
        if (Vector3.Distance(transform.position, target.position) <= thresholdDistance) {
            target = (target == pointA) ? pointB : pointA;
            FaceTarget();
        }
    }

    private void FaceTarget() {
        // Rotate to face the current target
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
