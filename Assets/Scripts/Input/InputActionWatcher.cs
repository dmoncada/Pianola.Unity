using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Pianola.Input
{
    public class InputActionWatcher : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _inputAction = null;

        [SerializeField]
        private UnityEvent _onActionPerformed = new();

        private void Update()
        {
            if (_inputAction.action.WasPerformedThisFrame())
            {
                Debug.LogFormat("Action: {0} performed.", _inputAction.action.name, this);

                _onActionPerformed.Invoke();
            }
        }
    }
}
