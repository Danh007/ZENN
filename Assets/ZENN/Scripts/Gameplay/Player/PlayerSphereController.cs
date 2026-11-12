using UnityEngine;
using MyGame.Core;
using MyGame.Systems; // Needed to reference MobileMovementInput

namespace MyGame.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerSphereController : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float moveSpeed = 15f;

        // Clean interface reference — zero manual inspector assignment needeed!
        private IMovementInputProvider movementProvider;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // Automatically find the active movement input provider in the scene.
            // When you swap to PC later, you just swap the component on the InputManager, 
            // and this automatically grabs the new one!
            movementProvider = FindAnyObjectByType<StaticVirtualJoystickInput>();

            if (movementProvider == null)
            {
                Debug.LogError("No IMovementInputProvider found in the scene!", this);
            }
        }
        private void FixedUpdate()
        {
            if (movementProvider == null || !movementProvider.IsDragging) return;

            Vector2 inputDelta = movementProvider.InputVector;

            // 1. Get the Camera's forward and right vectors
            Transform camTransform = Camera.main.transform;
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;

            // 2. Flatten them onto the XZ plane (ignore Y vertical tilt)
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 3. Transform 2D joystick input into camera-relative 3D world space
            Vector3 moveDirection = (camRight * inputDelta.x + camForward * inputDelta.y).normalized;

            // 4. Apply physical rolling force
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        }
    }
}