using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cardListParent;

    [SerializeField] private GameObject _playableCardPrefab;
    [SerializeField] private List<CardObject> _cards = new List<CardObject>();
    [SerializeField] private int _maxHandSize = 6;
    [SerializeField] private Deck _deck;

    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private ScrollRect _scrollRect;

    private void Start() {
        CardGameManager.i.OnStartPlayerTurn.AddListener(EnableCards);
    }

    public void StartHoverCard(CardObject cardObj)
    {
        CardGameManager.i.StartWiggle(cardObj.CardInstance.Owner);
        PutCardInCenterOfHand(cardObj.transform);
    }

    private void PutCardInCenterOfHand(Transform card)
    {
        Vector3[] itemCorners = new Vector3[4];
        (card as RectTransform).GetWorldCorners(itemCorners);

        Vector3[] contentCorners = new Vector3[4];
        _cardListParent.GetComponent<RectTransform>().GetWorldCorners(contentCorners);

        float itemCenterX = (itemCorners[0].x + itemCorners[2].x) / 2;
        float contentWidth = contentCorners[2].x - contentCorners[0].x;
        float viewportWidth = _scrollRect.viewport.rect.width;

        float normalizedPosition = (itemCenterX - contentCorners[0].x - viewportWidth / 2) / (contentWidth - viewportWidth);

        normalizedPosition = Mathf.Clamp01(normalizedPosition);
        LerpScrollRect(normalizedPosition, 0.1f);
    }

    private async void LerpScrollRect(float targetValue, float duration)
    {
        float startValue = _scrollRect.horizontalNormalizedPosition;
        float timePassed = 0;
        while (timePassed < duration) {
            _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startValue, targetValue, timePassed / duration);
            timePassed += Time.deltaTime;
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
        }
        _scrollRect.horizontalNormalizedPosition = targetValue;
    }

    public void AddCards(List<CardInstance> newCards) {
        List<GameObject> placeHolderCards = new List<GameObject>();
        foreach(CardInstance card in newCards) {
            GameObject newCardCoord = Instantiate(_playableCardPrefab, _cardListParent);
            int siblingIndex = _cardListParent.childCount - 2;
            newCardCoord.transform.SetSiblingIndex(siblingIndex);

            var cardController = newCardCoord.GetComponent<CardObject>();
            cardController.Initialize(card, this);
            _cards.Add(cardController);

            placeHolderCards.Add(newCardCoord);
        }

        DisableCardInteractionForSeconds(Utilities.OBJECT_LERP_TIME + Utilities.OBJECT_SHAKE_TIME * 1.2f);
    }

    public void EnableCards() => SetCardsEnabled(true);

    public void StopPlayingCards() => SetCardsEnabled(false);

    private void SetCardsEnabled(bool state) {
        foreach (var cardObj in _cards) cardObj.SetEnabled(state);
    }

    public void AddCard(CardObject cardObject) {
        if (_cards.Contains(cardObject)) return;
        _cards.Add(cardObject);
    }

    public void DrawUntilFull() {
        if (_maxHandSize > _cards.Count) _deck.Draw(_maxHandSize - _cards.Count);
    }

    public void Discard(int count = 1) {
        if (count == -1) count = _cards.Count;

        for (int i = 0; i < count; i++) {
            CardObject card = _cards[Random.Range(0, _cards.Count)];
            CardGameUIManager.i.AddToDiscardPile(card.CardInstance);
            _cards.Remove(card);
            Destroy(card.gameObject);
        }
    }

    public void RemoveCard(CardObject cardObject) {
        _cards.Remove(cardObject);
    }

    public void MoveFromDisplay(CardObject cardObject) {
        cardObject.transform.SetParent(transform);
        AddCard(cardObject);
    }

    private void DisableCardInteractionForSeconds(float duration) { }

}