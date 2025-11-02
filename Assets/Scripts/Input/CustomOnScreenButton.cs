using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Pianola.Input
{
    public class CustomOnScreenButton : OnScreenControl
    {
        [SerializeField]
        [InputControl(layout = "Button")]
        private string _controlPath = null;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        public void Press()
        {
            if (string.IsNullOrWhiteSpace(_controlPath) == false)
            {
                SendValueToControl(1f);
            }
        }

        public void Release()
        {
            if (string.IsNullOrWhiteSpace(_controlPath) == false)
            {
                SendValueToControl(0f);
            }
        }
    }
}
