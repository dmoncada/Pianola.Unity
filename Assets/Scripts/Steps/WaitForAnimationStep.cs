using UnityEngine;

namespace Pianola
{
    public class WaitForAnimationStep : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private string _targetStateName = string.Empty;

        private void Update()
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName(_targetStateName) && state.normalizedTime > 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
