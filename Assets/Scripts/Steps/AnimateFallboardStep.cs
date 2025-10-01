using System.Collections;
using UnityEngine;

public class AnimateFallboardStep : MonoBehaviour
{
    [SerializeField]
    private Transform _fallboardTransform = null;

    [SerializeField]
    private float _animationDuration = 1f;

    private void Start()
    {
        StartCoroutine(AnimateFallboard(_animationDuration));
    }

    private IEnumerator AnimateFallboard(float duration)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var angle = Mathf.Lerp(-90f, 0f, elapsed / duration);
            _fallboardTransform.rotation = Quaternion.Euler(angle, 0f, 0f);
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false); // Done, disable self.
    }
}
