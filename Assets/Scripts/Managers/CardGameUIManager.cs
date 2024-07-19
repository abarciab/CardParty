using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardGameUIManager : UIManager
{
    public new static CardGameUIManager i;
    protected override void Awake() { base.Awake(); i = this; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _instructionsText;
    [SerializeField] private TextMeshProUGUI _actionsCounter;
    [SerializeField] private TextMeshProUGUI _actionsLabel;

    [SerializeField] private Deck _deck;

    public void AddToDiscardPile(CardData data) => _deck.AddToDiscard(data);
}
