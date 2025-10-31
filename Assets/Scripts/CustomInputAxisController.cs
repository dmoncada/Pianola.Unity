using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pianola
{
    public class CustomInputAxisController
        : InputAxisControllerBase<CustomInputAxisController.Reader>
    {
        [SerializeField]
        private MouseOverVisualElement _hoverDetector = null;

        private bool _isDragging = false;

        private void Update()
        {
            CheckDragging();
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                UpdateControllers();
            }
        }

        private void CheckDragging()
        {
            var mouseClick = Mouse.current.leftButton;

            if (_isDragging == false && mouseClick.wasPressedThisFrame)
            {
                _isDragging = true;

                SetControllerState(_hoverDetector.Hovered == false);
            }
            else if (_isDragging && mouseClick.wasReleasedThisFrame)
            {
                _isDragging = false;

                SetControllerState(true);
            }
        }

        private void SetControllerState(bool enabled)
        {
            foreach (var controller in Controllers)
            {
                controller.Enabled = enabled;
            }
        }

        [Serializable]
        public class Reader : IInputAxisReader
        {
            public InputActionReference InputAction;

            public float Gain = 1f;

            public float GetValue(
                UnityEngine.Object context,
                IInputAxisOwner.AxisDescriptor.Hints hint
            )
            {
                var vector = InputAction.action.ReadValue<Vector2>();
                var value = hint == IInputAxisOwner.AxisDescriptor.Hints.Y ? vector.y : vector.x;
                return value * Gain;
            }
        }
    }
}
