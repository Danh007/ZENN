using UnityEngine;
using UnityEngine.EventSystems;
using MyGame.Core;

namespace MyGame.Systems
{
    public class StaticVirtualJoystickInput : MonoBehaviour, IMovementInputProvider, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("UI References")]
        [Tooltip("The background circle of the joystick.")]
        [SerializeField] private RectTransform background;
        [Tooltip("The inner handle/thumbstick that moves.")]
        [SerializeField] private RectTransform handle;

        [Header("Joystick Settings")]
        [Tooltip("Maximum pixel distance the handle can move from the center.")]
        [SerializeField] private float handleRange = 100f;

        // IMovementInputProvider contract implementation
        public bool IsDragging { get; private set; }
        public Vector2 InputVector { get; private set; }

        private Vector2 centerPosition;

        private void Start()
        {
            if (background != null)
            {
                centerPosition = background.anchoredPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Instantly register touch/click on the joystick area
            IsDragging = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || handle == null) return;

            // Convert screen position of the touch/mouse to local canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            // Calculate vector from center to touch point
            Vector2 direction = localPoint;
            float distance = direction.magnitude;

            // Clamp the handle within the maximum range
            if (distance > handleRange)
            {
                direction = direction.normalized * handleRange;
            }

            // Move the visual handle
            handle.anchoredPosition = direction;

            // Output normalized direction vector (-1 to 1) for the player controller
            InputVector = (handleRange > 0f) ? (direction / handleRange) : Vector2.zero;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Reset everything when the user lifts their finger/mouse button
            IsDragging = false;
            InputVector = Vector2.zero;

            if (handle != null)
            {
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }
}