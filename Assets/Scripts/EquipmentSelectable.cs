using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectable : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _currentlyWornText;
    [SerializeField] private PlayableCardDisplay _playableCard;
    private Equipment _data;

    public void Initialize(Equipment data, AdventurerData currentUser)
    {
        _data = data;
        _image.sprite = data.Sprite;
        _name.text = data.Name;

        if (currentUser) _currentlyWornText.text = "currently " + data.WornByString + " by " + currentUser.Name;
        else _currentlyWornText.text = "";

        if (data.Cards.Count == 0) {
            _playableCard.gameObject.SetActive(false);
            return;
        }
        _playableCard.Initialize(data.Cards[0]);
    }

    public void Select()
    {
        GetComponentInParent<EquipmentSelector>().SelectEquipment(_data);
    }
}
