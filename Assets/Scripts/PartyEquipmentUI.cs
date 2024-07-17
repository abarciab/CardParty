using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyEquipmentUI : MonoBehaviour
{
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private PlayableCardDisplay _cardDisplay;

    private Equipment _data;
    private OverworldPartyController _controller;

    public void Initialize(Equipment data, OverworldPartyController controller)
    {
        _data = data;
        _controller = controller;

        if (data == null) {
            InitializeEmpty();
            return;
        }

        _itemImg.gameObject.SetActive(true);
        _itemImg.sprite = data.Sprite;
        _nameText.text = data.Name;

        if (data.Cards.Count == 0) {
            _cardDisplay.gameObject.SetActive(false);
            return;
        }
        _cardDisplay.Initialize(data.Cards[0]);
    }

    private void InitializeEmpty()
    {
        gameObject.SetActive(true);
        _cardDisplay.gameObject.SetActive(false);
        _nameText.text = "";
        _itemImg.gameObject.SetActive(false);
    }
}
