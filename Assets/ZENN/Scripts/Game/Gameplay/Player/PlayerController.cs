using System;
using UnityEngine;
using ZENN.Systems;
using ZENN.Core;

namespace ZENN.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private SphereBody initialBody;

        private IMovementInputProvider movementProvider;

        public SphereBody ActiveBody { get; private set; }
        public event Action<SphereBody> OnActiveBodyChanged;

        private void Awake()
        {
            movementProvider = FindAnyObjectByType<StaticVirtualJoystickInput>();

            if (movementProvider == null)
            {
                Debug.LogError("No IMovementInputProvider found in the scene!", this);
            }
        }

        private void Start()
        {
            SphereBody target = initialBody != null ? initialBody : FindAnyObjectByType<SphereBody>();
            if (target != null)
            {
                Possess(target);
            }
        }

        public void Possess(SphereBody newBody)
        {
            if (ActiveBody == newBody) return;

            if (ActiveBody != null)
            {
                ActiveBody.SetPossessed(false);
            }

            ActiveBody = newBody;

            if (ActiveBody != null)
            {
                ActiveBody.SetPossessed(true);
            }

            OnActiveBodyChanged?.Invoke(ActiveBody);
        }

        private void FixedUpdate()
        {
            if (ActiveBody == null) return;

            if (movementProvider == null || !movementProvider.IsDragging)
            {
                ActiveBody.SetMoveInput(Vector2.zero);
                return;
            }

            ActiveBody.SetMoveInput(movementProvider.InputVector);
        }
    }
}