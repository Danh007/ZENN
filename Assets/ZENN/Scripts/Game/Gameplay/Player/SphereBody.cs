using UnityEngine;

namespace ZENN.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class SphereBody : MonoBehaviour
    {
        [Header("Movement Configuration")]
        [SerializeField] private float moveSpeed = 15f;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private float groundCheckOffset = 0.05f;
        [SerializeField] private bool showGroundGizmo = true;

        private Rigidbody rb;
        private Vector2 pendingMoveInput;

        public bool IsPossessed { get; private set; }

        public bool IsGrounded
        {
            get
            {
                Vector3 checkOrigin = GetGroundCheckOrigin();
                return Physics.CheckSphere(checkOrigin, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void SetPossessed(bool possessed)
        {
            IsPossessed = possessed;
            if (!possessed)
            {
                pendingMoveInput = Vector2.zero;
            }
        }

        public void SetMoveInput(Vector2 inputDelta)
        {
            pendingMoveInput = IsPossessed ? inputDelta : Vector2.zero;
        }

        public void TeleportTo(Vector3 worldPosition)
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = worldPosition;
            transform.position = worldPosition;
            pendingMoveInput = Vector2.zero;
        }

        private void FixedUpdate()
        {
            if (!IsPossessed || pendingMoveInput.sqrMagnitude <= 0.0001f) return;

            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            Transform camTransform = mainCam.transform;
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 moveDirection = camRight * pendingMoveInput.x + camForward * pendingMoveInput.y;
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        }

        private Vector3 GetGroundCheckOrigin()
        {
            float sphereRadius = transform.localScale.y * 0.5f;
            return transform.position + Vector3.down * (sphereRadius + groundCheckOffset);
        }

        private void OnDrawGizmos()
        {
            if (!showGroundGizmo) return;

            bool grounded = Application.isPlaying && rb != null && IsGrounded;
            Gizmos.color = grounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(GetGroundCheckOrigin(), groundCheckRadius);
        }
    }
}