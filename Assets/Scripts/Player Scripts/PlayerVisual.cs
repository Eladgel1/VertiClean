using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerVisual : MonoBehaviour {
    private Rigidbody rigidBody;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        // This is where realistic physics behavior can be added if needed
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Tool") || collision.gameObject.CompareTag("CleanerPlatform")) {
            // Handle collision with tools or platform - for future effects/logic
            Debug.Log("Player collided with an interactable object.");
        }
    }
}

