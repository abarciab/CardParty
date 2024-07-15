using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class OverworldDeckController : MonoBehaviour
{
    [SerializeField] private GameObject _horizontalListPrefab;
    [SerializeField] private Transform _listParent;
    [SerializeField] private List<GameObject> _spawnedRows = new List<GameObject>();
    [SerializeField] private Transform _endSpacer;
    [SerializeField] private GameObject _selectedCardParent;
    [SerializeField] private PlayableCardDisplay _selectedCardDisplay;

    private const int _cardsPerRow = 5;

    public void Open()
    {
        Clear();
        ShowCards();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SelectCard(CardData toSelect)
    {
        _selectedCardParent.SetActive(true);
        _selectedCardDisplay.Initialize(toSelect, toSelect.Owner.Name);
    }

    private void Clear()
    {
        foreach (var row in _spawnedRows) Destroy(row.gameObject);
        _spawnedRows.Clear();
    }

    private void ShowCards()
    {
        var deck = PlayerInfo.Party.GetDeck();
        int numRows = Mathf.CeilToInt(deck.Count / (float) _cardsPerRow);
        for (int i = 0; i < numRows; i++) {
            SpawnRow(deck, i * _cardsPerRow);
        }
        _endSpacer.SetAsLastSibling();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_listParent.GetComponent<RectTransform>());
    }

    private void SpawnRow(List<CardData> deck, int startIndex)
    {
        var newRow = Instantiate(_horizontalListPrefab, _listParent).transform;

        for (int i = 0; i < _cardsPerRow; i++) {
            if (startIndex + i >= deck.Count) continue;
            var card = deck[startIndex + i];
            var owner = card.Owner.Name;
            var playableCard = newRow.GetChild(i).GetComponent<PlayableCardDisplay>();
            playableCard.Initialize(card, owner);
            playableCard.AddOnClick(() => SelectCard(card));
        }

        _spawnedRows.Add(newRow.gameObject);
    }
}
