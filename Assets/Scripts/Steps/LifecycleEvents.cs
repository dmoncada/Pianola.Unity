using UnityEngine;
using UnityEngine.Events;

namespace Pianola
{
    public class LifecycleEvents : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onEnable = new();

        [SerializeField]
        private UnityEvent _onDisable = new();

        private void OnEnable()
        {
            _onEnable.Invoke();
        }

        private void OnDisable()
        {
            _onDisable.Invoke();
        }
    }
}
