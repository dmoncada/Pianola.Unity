using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

namespace Pianola.Input
{
    [Serializable]
    public struct InputActionButton
    {
        private const TrickleDown UseTrickleDown = TrickleDown.TrickleDown;

        public InputActionReference Action;
        public CustomOnScreenButton Button;

        private Button _uiToolkitButton;
        private Action _callback;

        public void Set(Button uiToolkitButton)
        {
            _uiToolkitButton = uiToolkitButton;
        }

        public void Bind(Action callback)
        {
            if (_callback == null)
            {
                _callback = callback;
                Action.action.Enable();
                Action.action.performed += OnActionPerformed;
                _uiToolkitButton?.RegisterCallback<PointerUpEvent>(Press, UseTrickleDown);
                _uiToolkitButton?.RegisterCallback<PointerDownEvent>(Release, UseTrickleDown);
                BindInputActionToOnScreenButton(Action, Button);
            }
        }

        public void Unbind()
        {
            if (_callback != null)
            {
                _callback = null;
                Action.action.Disable();
                Action.action.performed -= OnActionPerformed;
                _uiToolkitButton?.UnregisterCallback<PointerUpEvent>(Press, UseTrickleDown);
                _uiToolkitButton?.UnregisterCallback<PointerDownEvent>(Release, UseTrickleDown);
                Button.controlPath = string.Empty;
            }
        }

        private void OnActionPerformed(CallbackContext _)
        {
            _callback.Invoke();
        }

        private void Press(PointerUpEvent _)
        {
            Button.Press();
        }

        private void Release(PointerDownEvent _)
        {
            Button.Release();
        }

        private void BindInputActionToOnScreenButton(
            InputAction action,
            CustomOnScreenButton button
        )
        {
            foreach (var binding in action.bindings)
            {
                var path = binding.effectivePath;
                if (path.StartsWith("<Keyboard>/"))
                {
                    button.controlPath = path;
                    return;
                }
            }

            throw new Exception(); // TODO(dmoncada): add descriptive message.
        }
    }
}
