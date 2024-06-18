using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopySlider : MonoBehaviour
{
    [SerializeField] private Slider _leadSlider;
    [SerializeField] private Slider _followSlider;

    private void Start()
    {
        _leadSlider.onValueChanged.AddListener(UpdateFollow);
    }

    private void UpdateFollow(float value)
    {
        _followSlider.value = value;
    }
}
