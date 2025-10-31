using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pianola
{
    public class CustomInputAxisController : CinemachineInputAxisController
    {
        [SerializeField]
        private MouseOverVisualElement _hoverDetector = null;

        private void Update()
        {
            if (Application.isPlaying)
            {
                CheckDragging();

                UpdateControllers();
            }
        }

        private void CheckDragging()
        {
            var mouseClick = Mouse.current.leftButton;
            if (mouseClick.wasPressedThisFrame)
            {
                SetControllerState(_hoverDetector.HoveredOverTarget == false);
            }
            if (mouseClick.wasReleasedThisFrame)
            {
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
    }
}
