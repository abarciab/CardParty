using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureObjectUIController : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _blockSlider;
    private Creature _creatureObj;

    public void Initialize(Creature creatureObj)
    {
        _creatureObj = creatureObj;

        _hpSlider.value = 1;
        _blockSlider.value = 0;
        _creatureObj.OnHealthPercentChanged.AddListener((float newValue) => _hpSlider.value = newValue);
        _creatureObj.OnBlockPercentChanged.AddListener((float newValue) => _blockSlider.value = newValue);
        _blockSlider.onValueChanged.AddListener((float value) => UpdateVisuals());

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        _blockSlider.gameObject.SetActive(_blockSlider.value > 0);
    }
}
