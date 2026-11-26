using UnityEngine;
using UnityEngine.EventSystems;
using ZENN.Core;

namespace ZENN.Systems
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

        [Tooltip("Normalized distance (0 to 1) from the center where input is ignored.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float deadZone = 0.1f;

        [Tooltip("Maps raw distance from center (X: 0 to 1) to output strength (Y: 0 to 1).")]
        [SerializeField] private AnimationCurve responseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public bool IsDragging { get; private set; }
        public Vector2 InputVector { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDragging = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || handle == null || handleRange <= 0f) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            // Clamp visual handle within handleRange
            Vector2 clampedOffset = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = clampedOffset;

            // Normalized distance from 0 (center) to 1 (edge)
            float normalizedDistance = clampedOffset.magnitude / handleRange;

            // 1. Deadzone check: ignore tiny accidental touches near dead center
            if (normalizedDistance < deadZone)
            {
                InputVector = Vector2.zero;
                return;
            }

            // 2. Rescale distance smoothly from [deadZone..1] to [0..1] so there's no sudden jump
            float remappedMagnitude = Mathf.InverseLerp(deadZone, 1f, normalizedDistance);

            // 3. Evaluate your custom curve for fine control
            float curvedMagnitude = responseCurve.Evaluate(remappedMagnitude);

            // Apply the curved magnitude to the direction
            InputVector = clampedOffset.normalized * curvedMagnitude;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
            InputVector = Vector2.zero;

            if (handle != null)
            {
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }
}
