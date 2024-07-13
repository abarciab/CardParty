using MyBox;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyAdventurerUI : MonoBehaviour
{
    [SerializeField] private Image _portrait;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Slider _hpSlider;

    private OverworldPartyController _controller;
    private AdventurerData _data;

    public void Initialize(AdventurerData data, OverworldPartyController controller)
    {
        _controller = controller;
        _data = data;

        _hpSlider.value = data.Stats.HealthPercent;
        _portrait.sprite = data.portrait;
        _nameText.text = data.Name;
        gameObject.SetActive(true);
    }

    public void Select()
    {
        _controller.SwitchAdventurer(_data);
    }
}
