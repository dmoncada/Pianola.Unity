using UnityEngine;

namespace Pianola
{
    public class AnimatePianoStep : MonoBehaviour
    {
        private const string AnimationStateName = "AnimatePiano";

        [SerializeField]
        private PlaybackStateAsset _playbackState = null;

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
            var progress = GetProgress(AnimationStateName);

            if (_playbackState != null)
            {
                _playbackState.Opacity = progress;
            }

            if (progress >= 1f)
            {
                gameObject.SetActive(false);
            }
        }

        private float GetProgress(string name)
        {
            var state = _pianoAnimator.GetCurrentAnimatorStateInfo(0);

            return state.IsName(name) ? state.normalizedTime : 0f;
        }
    }
}
