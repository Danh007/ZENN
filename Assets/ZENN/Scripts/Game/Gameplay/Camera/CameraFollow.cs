using UnityEngine;

namespace ZENN.Gameplay
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Leave empty to auto-find the player in the scene.")]
        [SerializeField] private Transform playerTransform;

        [Header("Leash / Rope Settings")]
        [Tooltip("The physical offset from the player where the rope is anchored (e.g., higher up above the ball).")]
        [SerializeField] private Vector3 ropeAnchorOffset = new Vector3(0f, 3f, 0f);
        [Tooltip("Maximum distance the camera can get from the anchor before the rope goes taut and pulls it along.")]
        [SerializeField] private float maxLeashLength = 10f;
        [Tooltip("How smoothly the camera is dragged along when the rope is taut.")]
        [SerializeField] private float dragSmoothTime = 0.15f;

        [Header("Rotation Dynamics")]
        [Tooltip("Speed at which the camera rotates to look at the player.")]
        [SerializeField] private float rotationSmoothSpeed = 15f;

        private Vector3 currentVelocity;

        private void Start()
        {
            if (playerTransform == null)
            {
                var playerController = FindAnyObjectByType<PlayerSphereController>();
                if (playerController != null)
                {
                    playerTransform = playerController.transform;
                }
            }

            // Initialize camera starting position at a safe distance from the anchor
            if (playerTransform != null && transform.position == Vector3.zero)
            {
                Vector3 initialAnchor = playerTransform.position + ropeAnchorOffset;
                transform.position = initialAnchor + new Vector3(0f, 0f, -maxLeashLength);
            }
        }

        private void FixedUpdate()
        {
            if (playerTransform == null) return;

            // 1. Separate the two distinct points:
            // - Rope Anchor: Where the physical leash is tied (can be offset higher)
            // - Look Target: The exact center of the player sphere (always center-screen)
            Vector3 ropeAnchor = playerTransform.position + ropeAnchorOffset;
            Vector3 lookTarget = playerTransform.position;

            Vector3 currentCamPos = transform.position;

            // 2. Leash calculation relative to the rope anchor
            Vector3 anchorToCamera = currentCamPos - ropeAnchor;
            float currentDistance = anchorToCamera.magnitude;

            if (currentDistance > maxLeashLength)
            {
                // Pull camera back to the edge of the leash sphere
                Vector3 tautPosition = ropeAnchor + anchorToCamera.normalized * maxLeashLength;

                transform.position = Vector3.SmoothDamp(
                    currentCamPos,
                    tautPosition,
                    ref currentVelocity,
                    dragSmoothTime,
                    float.MaxValue,
                    Time.fixedDeltaTime
                );
            }
            else
            {
                // Slack zone: camera stays put, clear velocity
                currentVelocity = Vector3.zero;
            }

            // 3. Rotation looks strictly at the player's true center (independent of rope height)
            Vector3 directionToPlayer = lookTarget - transform.position;
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmoothSpeed * Time.fixedDeltaTime
                );
            }
        }
    }
}