using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

namespace Pianola.Input
{
    [Serializable]
    public class InputActionButton
    {
        private const TrickleDown UseTrickleDown = TrickleDown.TrickleDown;

        public InputActionReference Action;
        public OnScreenButton Button;

        private Button _uiToolkitButton = null;
        public Button UIToolkitButton
        {
            get => _uiToolkitButton;
            set
            {
                UIToolkit_UnregisterCallbacks(_uiToolkitButton);

                _uiToolkitButton = value;

                UIToolkit_RegisterCallbacks(_uiToolkitButton);
            }
        }

        private Action _callback = null;

        public void Bind(Action callback)
        {
            if (_callback == null)
            {
                _callback = callback;
                Action.action.Enable();
                Action.action.performed += OnActionPerformed;
                Button.SetControlPath(GetFirstBindingPath(Action));
            }
        }

        public void Unbind()
        {
            if (_callback != null)
            {
                _callback = null;
                Action.action.Disable();
                Action.action.performed -= OnActionPerformed;
                Button.SetControlPath(string.Empty);
            }
        }

        private void OnActionPerformed(CallbackContext _)
        {
            _callback.Invoke();
        }

        private void UIToolkit_RegisterCallbacks(Button button)
        {
            if (button != null)
            {
                button.RegisterCallback<PointerDownEvent>(Press, UseTrickleDown);
                button.RegisterCallback<PointerUpEvent>(Release, UseTrickleDown);
            }
        }

        private void UIToolkit_UnregisterCallbacks(Button button)
        {
            if (button != null)
            {
                button.UnregisterCallback<PointerDownEvent>(Press, UseTrickleDown);
                button.UnregisterCallback<PointerUpEvent>(Release, UseTrickleDown);
            }
        }

        private void Press(PointerDownEvent _)
        {
            Button.OnPointerDown(null);
        }

        private void Release(PointerUpEvent _)
        {
            Button.OnPointerUp(null);
        }

        private string GetFirstBindingPath(InputAction action)
        {
            foreach (var binding in action.bindings)
            {
                var path = binding.effectivePath;
                if (path.StartsWith("<Keyboard>"))
                {
                    return path;
                }
            }

            throw new Exception(); // TODO(dmoncada): add descriptive message.
        }
    }

    public static class OnScreenControlExtensions
    {
        public static void SetControlPath(this OnScreenControl control, string controlPath)
        {
            control.enabled = false;
            control.controlPath = controlPath;
            control.enabled = true;
        }
    }
}
