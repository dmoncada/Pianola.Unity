using UnityEngine;

namespace Pianola
{
    public class AnimatePianoStep : MonoBehaviour
    {
        private const string AnimationStateName = "AnimatePiano";

        [SerializeField]
        private Animator _pianoAnimator = null;

        [SerializeField]
        private float _animationDuration = 1f;

        private void Start()
        {
            _pianoAnimator.speed = 1f / _animationDuration;

            _pianoAnimator.Play(AnimationStateName);
        }

        private void Update()
        {
            if (IsFinished(AnimationStateName))
            {
                gameObject.SetActive(false);
            }
        }

        private bool IsFinished(string name)
        {
            var state = _pianoAnimator.GetCurrentAnimatorStateInfo(0);

            return state.IsName(name) && state.normalizedTime >= 1f;
        }
    }
}
