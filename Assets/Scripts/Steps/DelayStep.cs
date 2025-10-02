using UnityEngine;

namespace Pianola
{
    public class DelayStep : MonoBehaviour
    {
        [SerializeField]
        private float _delaySeconds = 1f;

        private void OnEnable()
        {
            Invoke(nameof(DisableSelf), _delaySeconds);
        }

        private void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
