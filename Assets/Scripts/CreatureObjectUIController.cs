using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureObjectUIController : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _blockSlider;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _blockText;
    [SerializeField] private GameObject _labelParent;

    private CreatureObject _creatureObj;

    public void SetLabelVisible(bool visible) => _labelParent.SetActive(visible);

    public void Initialize(CreatureObject creatureObj)
    {
        _creatureObj = creatureObj;

        _hpSlider.value = 0.5f;
        _blockSlider.value = 0;
        _creatureObj.OnHealthPercentChanged.AddListener(UpdateHealth);
        _creatureObj.OnBlockPercentChanged.AddListener(UpdateBlock);

        _nameText.text = creatureObj.GetName();
    }

    public void UpdateHealth(float percent)
    {
        _hpSlider.value = percent * 0.5f;
        _hpText.text = _creatureObj.GetHealth().ToString();
    }

    public void UpdateBlock(float percent)
    {
        _blockSlider.value = percent * 0.5f;
        _blockText.text = _creatureObj.GetBlock().ToString();
    }
}
