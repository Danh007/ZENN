using UnityEngine;
using UnityEngine.InputSystem;
using ZENN.Core;

namespace ZENN.Systems
{
    public abstract class BaseTouchInputHandler : MonoBehaviour
    {
        [Header("Screen Split Settings")]
        [Range(0.1f, 0.9f)]
        [SerializeField] protected float splitThresholdY = 0.5f;

        public bool IsDragging { get; protected set; }
        public Vector2 InputVector { get; protected set; }

        protected abstract bool IsInMyZone(float normalizedY);

        protected virtual void Update()
        {
            IsDragging = false;
            InputVector = Vector2.zero;

            Touchscreen currentTouchscreen = Touchscreen.current;
            if (currentTouchscreen == null) return;

            foreach (var touch in currentTouchscreen.touches)
            {
                var phase = touch.phase.ReadValue();
                if (phase == UnityEngine.InputSystem.TouchPhase.Began ||
                    phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    Vector2 touchPos = touch.position.ReadValue();
                    float normalizedY = touchPos.y / Screen.height;

                    if (IsInMyZone(normalizedY))
                    {
                        IsDragging = true;
                        InputVector = touch.delta.ReadValue();
                        break;
                    }
                }
            }
        }
    }
}