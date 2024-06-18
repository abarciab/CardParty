using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] private Gradient _fadeGradient;
    [SerializeField] private bool _manualAssignImg;
    [SerializeField, ConditionalField(nameof(_manualAssignImg))] private Image _img;

    public float FadeTime = 0.5f;

    private void OnEnable()
    {
        if (!_img) _img = GetComponent<Image>();
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        _img.raycastTarget = true;
        StopAllCoroutines();
        AnimateFade(false);
    }

    public void Disappear()
    {
        gameObject.SetActive(true);
        _img.raycastTarget = false;
        StopAllCoroutines();
        AnimateFade(true);
    }

    private async void AnimateFade(bool reverse)
    {
        float timePassed = 0;
        while (timePassed < FadeTime) {
            float progress = timePassed / FadeTime;
            if (reverse) progress = 1 - progress;

            _img.color = _fadeGradient.Evaluate(progress);

            timePassed += Time.deltaTime;
            await Task.Yield();
        }
        _img.color = _fadeGradient.Evaluate(reverse ? 0 : 1);
        if (reverse) gameObject.SetActive(false);
    }
}
