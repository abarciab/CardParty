using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CopySlider : MonoBehaviour
{
    [SerializeField] private Slider _leadSlider;
    [SerializeField] private Slider _followSlider;

    [SerializeField] private bool _autoMode = true;
    [SerializeField] private bool _withDelay;
    [SerializeField, ConditionalField(nameof(_withDelay))] private float _lerpFactor = 3;
    [SerializeField, ConditionalField(nameof(_withDelay))] private float _delayWaitTime = 0.1f;

    private void Start()
    {
        if (_autoMode) {
            if (!_withDelay) _leadSlider.onValueChanged.AddListener(UpdateFollowInstant);
            else _leadSlider.onValueChanged.AddListener(UpdateFollowInstant);
        }
    }

    public void SetFollower(float value) => _followSlider.value = value;

    private void UpdateFollowInstant(float value)
    {
        _followSlider.value = value;
    }

    public async void UpdateFollowWithDelay(float targetValue)
    {
        if (_leadSlider.value > _followSlider.value) {
            UpdateFollowInstant(targetValue);
            return;
        }

        await Task.Delay((int)(_delayWaitTime * 1000));
        while (Mathf.Abs(targetValue - _followSlider.value) > 0.01f) {
            _followSlider.value = Mathf.Lerp(_followSlider.value, targetValue, _lerpFactor * Time.deltaTime);
            await Task.Delay(1);
        }
        UpdateFollowInstant(targetValue);
    }
}
