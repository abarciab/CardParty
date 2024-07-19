using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardGameUIManager : UIManager
{
    public new static CardGameUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _instructionsText;
    [SerializeField] private GameObject _instructionsParent;

    [SerializeField] private Deck _deck;

    public void AddToDiscardPile(CardData data) => _deck.AddToDiscard(data);


    private void Update()
    {
        //TEMP
        UpdateInstructions();
    }

    private void UpdateInstructions()
    {
        if (_instructionsText.text.Length > 1 && !_instructionsParent.activeInHierarchy) {
            _instructionsParent.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_instructionsParent.GetComponent<RectTransform>());
        }
        _instructionsParent.SetActive(_instructionsText.text.Length > 0);
    }
}
